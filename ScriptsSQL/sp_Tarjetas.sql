-- SPs para tarjetas
CREATE OR REPLACE FUNCTION sp_cards_create(
    p_usuario_id UUID,
    p_tipo UUID,
    p_numero_enmascarado text,
    p_fecha_expiracion text,
    p_cvv_encriptado text,
    p_pin_encriptado text,
    p_moneda UUID,
    p_limite_credito DECIMAL(18,2),
    p_saldo_actual DECIMAL(18,2) DEFAULT 0,
    OUT card_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    RAISE NOTICE 'Usuario: %, TipoTarjeta: %, Moneda: %', p_usuario_id, p_tipo, p_moneda;

    IF NOT EXISTS (SELECT 1 FROM "usuario" WHERE id = p_usuario_id) THEN
        RAISE EXCEPTION 'Usuario no encontrado';
    END IF;
    

    

    
    IF p_limite_credito <= 0 THEN
        RAISE EXCEPTION 'El límite de crédito debe ser mayor a cero';
    END IF;
    
    IF p_saldo_actual > p_limite_credito THEN
        RAISE EXCEPTION 'El saldo actual no puede exceder el límite de crédito';
    END IF;
    
    IF NOT p_fecha_expiracion ~ '^(0[1-9]|1[0-2])/[0-9]{2}$' THEN
        RAISE EXCEPTION 'Formato de fecha de expiración inválido. Use MM/YY';
    END IF;
    
    INSERT INTO tarjeta (
        usuario_id,
        tipo,
        numero_enmascarado,
        fecha_expiracion,
        cvv_hash,
        pin_hash,
        moneda,
        limite_credito,
        saldo_actual,
        fecha_creacion,
        fecha_actualizacion
    ) VALUES (
        p_usuario_id,
        p_tipo,
        p_numero_enmascarado,
        p_fecha_expiracion,
        p_cvv_encriptado,
        p_pin_encriptado,
        p_moneda,
        p_limite_credito,
        p_saldo_actual,
        NOW(),
        NOW()
    )
    RETURNING id INTO card_id;
END;
$$;


CREATE OR REPLACE FUNCTION sp_cards_get(
    p_owner_id UUID DEFAULT NULL,
    p_card_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id UUID,
    usuario_id UUID,
    tipo uuid,
    nombre_tipo_tarjeta text,
    numero_enmascarado text,
    fecha_expiracion text,
    moneda uuid,
    nombre_moneda text,
    limite_credito DECIMAL(18,2),
    saldo_actual DECIMAL(18,2),
    disponible DECIMAL(18,2),
    fecha_creacion TIMESTAMP,
    fecha_actualizacion TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        t.id,
        t.usuario_id,
        t.tipo,
        tt.nombre as nombre_tipo_tarjeta,
        t.numero_enmascarado,
        t.fecha_expiracion,
        t.moneda,
        m.nombre as nombre_moneda,
        t.limite_credito::numeric,
        t.saldo_actual::numeric,
        (t.limite_credito - t.saldo_actual)::numeric as disponible,
        t.fecha_creacion,
        t.fecha_actualizacion
    FROM "tarjeta" t
    LEFT JOIN "tipoTarjeta" tt ON t.tipo = tt.id
    LEFT JOIN "moneda" m ON t.moneda = m.id
    WHERE (p_owner_id IS NOT NULL AND t.usuario_id = p_owner_id)
       OR (p_card_id IS NOT NULL AND t.id = p_card_id);
END;
$$;

CREATE OR REPLACE FUNCTION sp_card_movements_list(
    p_card_id UUID,
    p_from_date TIMESTAMP DEFAULT NULL,
    p_to_date TIMESTAMP DEFAULT NULL,
    p_type UUID DEFAULT NULL,
    p_q text DEFAULT NULL,
    p_page INTEGER DEFAULT 1,
    p_page_size INTEGER DEFAULT 20,
    OUT items JSON,
    OUT total INTEGER,
    OUT page INTEGER,
    OUT page_size INTEGER
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_offset INTEGER;
BEGIN
    page := GREATEST(1, p_page);
    page_size := GREATEST(1, LEAST(100, p_page_size));
    v_offset := (page - 1) * page_size;
    
    IF NOT EXISTS (SELECT 1 FROM "tarjeta" WHERE id = p_card_id) THEN
        RAISE EXCEPTION 'Tarjeta no encontrada';
    END IF;
    
    SELECT COUNT(*) INTO total
    FROM "movimientoTarjeta" mt
    WHERE mt.cuenta_id = p_card_id
      AND (p_from_date IS NULL OR mt.fecha >= p_from_date)
      AND (p_to_date IS NULL OR mt.fecha <= p_to_date)
      AND (p_type IS NULL OR mt.tipo = p_type)
      AND (p_q IS NULL OR mt.descripcion ILIKE '%' || p_q || '%');
    
    SELECT json_agg(row_to_json(t))
    INTO items
    FROM (
        SELECT 
            mt.id,
            mt.fecha,
            mt.tipo,
            tmt.nombre AS nombre_tipo,
            mt.descripcion,
            mt.moneda,
            m.nombre AS nombre_moneda,
            mt.monto
        FROM "movimientoTarjeta" mt
        LEFT JOIN "tipoMovimientoTarjeta" tmt ON mt.tipo = tmt.id
        LEFT JOIN "moneda" m ON mt.moneda = m.id
        WHERE mt.cuenta_id = p_card_id
          AND (p_from_date IS NULL OR mt.fecha >= p_from_date)
          AND (p_to_date IS NULL OR mt.fecha <= p_to_date)
          AND (p_type IS NULL OR mt.tipo = p_type)
          AND (p_q IS NULL OR mt.descripcion ILIKE '%' || p_q || '%')
        ORDER BY mt.fecha DESC
        LIMIT page_size
        OFFSET v_offset
    ) t;

    IF items IS NULL THEN
        items := '[]';
    END IF;
END;
$$;


CREATE OR REPLACE FUNCTION sp_card_movement_add(
    p_card_id UUID,
    p_fecha TIMESTAMP,
    p_tipo uuid,
    p_descripcion text,
    p_moneda uuid,
    p_monto DECIMAL(18,2),
    OUT movement_id UUID,
    OUT nuevo_saldo_tarjeta DECIMAL(18,2)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_tarjeta_record RECORD;
    v_nombre_tipo_movimiento text;
    v_saldo_anterior DECIMAL(18,2);
BEGIN
    SELECT saldo_actual, limite_credito, moneda 
    INTO v_tarjeta_record
    FROM "tarjeta" 
    WHERE id = p_card_id;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Tarjeta no encontrada';
    END IF;
    
    SELECT nombre INTO v_nombre_tipo_movimiento
    FROM "tipoMovimientoTarjeta" 
    WHERE id = p_tipo;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Tipo de movimiento no válido';
    END IF;
    
    IF v_tarjeta_record.moneda != p_moneda THEN
        RAISE EXCEPTION 'La moneda del movimiento no coincide con la moneda de la tarjeta';
    END IF;
    
    v_saldo_anterior := v_tarjeta_record.saldo_actual;
    
    CASE v_nombre_tipo_movimiento
        WHEN 'Compra' THEN
            nuevo_saldo_tarjeta := v_saldo_anterior + p_monto;
            
            IF nuevo_saldo_tarjeta > v_tarjeta_record.limite_credito THEN
                RAISE EXCEPTION 'Límite de crédito excedido. Saldo actual: %, Límite: %, Monto intentado: %', 
                                v_saldo_anterior, v_tarjeta_record.limite_credito, p_monto;
            END IF;
            
        WHEN 'Pago' THEN
            nuevo_saldo_tarjeta := v_saldo_anterior - p_monto;
            
            IF nuevo_saldo_tarjeta < 0 THEN
                RAISE EXCEPTION 'El pago no puede exceder el saldo actual. Saldo actual: %, Monto intentado: %', 
                                v_saldo_anterior, p_monto;
            END IF;
            
        ELSE
            RAISE EXCEPTION 'Tipo de movimiento no soportado: %', v_nombre_tipo_movimiento;
    END CASE;
    
    INSERT INTO "movimientoTarjeta" (
        cuenta_id,
        fecha,
        tipo,
        descripcion,
        moneda,
        monto
    ) VALUES (
        p_card_id,
        COALESCE(p_fecha, NOW()),
        p_tipo,
        p_descripcion,
        p_moneda,
        p_monto
    )
    RETURNING id INTO movement_id;
    
    UPDATE "tarjeta" 
    SET 
        saldo_actual = nuevo_saldo_tarjeta,
        fecha_actualizacion = NOW()
    WHERE id = p_card_id;
 
END;
$$;


