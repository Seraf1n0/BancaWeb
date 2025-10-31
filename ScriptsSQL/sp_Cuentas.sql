-- sps para cuentas
CREATE OR REPLACE FUNCTION sp_accounts_create(
    p_usuario_id UUID,
    p_iban VARCHAR,
    p_alias VARCHAR,
    p_tipo int,
    p_moneda int,
    p_saldo_inicial DECIMAL(18,2),
    p_estado UUID DEFAULT NULL,
    OUT account_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificaciones de datos sensibles (no nulos ni repetidos)
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE id = p_usuario_id) THEN
        RAISE EXCEPTION 'Usuario no encontrado';
    END IF;
    
    IF EXISTS (SELECT 1 FROM cuenta WHERE iban = p_iban) THEN
        RAISE EXCEPTION 'El IBAN ya está registrado';
    END IF;
    
    IF p_saldo_inicial < 0 THEN
        RAISE EXCEPTION 'El saldo inicial no puede ser negativo';
    END IF;
    
    -- Estado: activo
    DECLARE
        v_estado_final UUID := COALESCE(p_estado, (SELECT id FROM estadoCuenta WHERE nombre = 'Activa'));
    BEGIN
        INSERT INTO cuenta (
            usuario_id,
            iban,
            alias,
            tipoCuenta,
            moneda,
            saldo,
            estado,
            fecha_creacion,
            fecha_actualizacion
        ) VALUES (
            p_usuario_id,
            p_iban,
            p_alias,
            p_tipo,
            p_moneda,
            p_saldo_inicial,
            v_estado_final,
            NOW(),
            NOW()
        )
        RETURNING id INTO account_id;
        
        -- movimiento de apertura
        IF p_saldo_inicial > 0 THEN
            INSERT INTO movimientoCuenta (
                cuenta_id,
                fecha,
                tipo,
                descripcion,
                moneda,
                monto
            ) VALUES (
                account_id,
                NOW(),
                (SELECT id FROM tipoMovimientoCuenta WHERE nombre = 'Crédito'),
                'Apertura de cuenta - Saldo inicial',
                p_moneda,
                p_saldo_inicial
            );
        END IF;
    END;
END;
$$;

CREATE OR REPLACE FUNCTION sp_accounts_get(
    p_owner_id UUID DEFAULT NULL,
    p_account_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id UUID,
    usuario_id UUID,
    iban VARCHAR,
    alias VARCHAR,
    tipoCuenta int,
    moneda int,
    saldo DECIMAL(18,2),
    estado int,
    fecha_creacion TIMESTAMP,
    fecha_actualizacion TIMESTAMP,
    nombre_tipo_cuenta VARCHAR,
    nombre_moneda VARCHAR,
    nombre_estado VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id,
        c.usuario_id,
        c.iban,
        c.alias,
        c.tipoCuenta,
        c.moneda,
        c.saldo,
        c.estado,
        c.fecha_creacion,
        c.fecha_actualizacion,
        tc.nombre as nombre_tipo_cuenta,
        m.nombre as nombre_moneda,
        ec.nombre as nombre_estado
    FROM cuenta c
    LEFT JOIN tipoCuenta tc ON c.tipoCuenta = tc.id
    LEFT JOIN moneda m ON c.moneda = m.id
    LEFT JOIN estadoCuenta ec ON c.estado = ec.id
    WHERE (p_owner_id IS NOT NULL AND c.usuario_id = p_owner_id)
       OR (p_account_id IS NOT NULL AND c.id = p_account_id);
END;
$$;

CREATE OR REPLACE FUNCTION sp_accounts_set_status(
    p_account_id UUID,
    p_nuevo_estado int,
    OUT updated BOOLEAN
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_saldo_actual DECIMAL(18,2);
    v_estado_actual int;
    v_nombre_estado VARCHAR;
BEGIN
    updated := FALSE;
    
    -- se toma la info general de la cuenta
    SELECT saldo, estado INTO v_saldo_actual, v_estado_actual
    FROM cuenta 
    WHERE id = p_account_id;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Cuenta no encontrada';
    END IF;
    
    -- Nombre para el nuevo estado
    SELECT nombre INTO v_nombre_estado
    FROM estadoCuenta 
    WHERE id = p_nuevo_estado;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Estado de cuenta no válido';
    END IF;
    
    CASE v_nombre_estado
        WHEN 'Cerrada' THEN
            IF v_saldo_actual != 0 THEN
                RAISE EXCEPTION 'No se puede cerrar una cuenta con saldo diferente de cero';
            END IF;
            
        WHEN 'Bloqueada' THEN
            NULL;
            
        WHEN 'Activa' THEN
            IF v_estado_actual != (SELECT id FROM estadoCuenta WHERE nombre = 'Bloqueada') THEN
                RAISE EXCEPTION 'Solo se puede reactivar una cuenta bloqueada';
            END IF;
            
        ELSE
            RAISE EXCEPTION 'Operación de cambio de estado no permitida';
    END CASE;
    
    UPDATE cuenta 
    SET 
        estado = p_nuevo_estado,
        fecha_actualizacion = NOW()
    WHERE id = p_account_id;
    
    updated := TRUE;
END;
$$;

CREATE OR REPLACE FUNCTION sp_account_movements_list(
    p_account_id UUID,
    p_from_date TIMESTAMP DEFAULT NULL,
    p_to_date TIMESTAMP DEFAULT NULL,
    p_type UUID DEFAULT NULL,
    p_q VARCHAR DEFAULT NULL,
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
    page_size := GREATEST(1, LEAST(100, p_page_size)); -- Máximo 100 por página
    v_offset := (page - 1) * page_size;
    
    IF NOT EXISTS (SELECT 1 FROM cuenta WHERE id = p_account_id) THEN
        RAISE EXCEPTION 'Cuenta no encontrada';
    END IF;
    
    SELECT COUNT(*) INTO total
    FROM movimientoCuenta mc
    WHERE mc.cuenta_id = p_account_id
      AND (p_from_date IS NULL OR mc.fecha >= p_from_date)
      AND (p_to_date IS NULL OR mc.fecha <= p_to_date)
      AND (p_type IS NULL OR mc.tipo = p_type)
      AND (p_q IS NULL OR mc.descripcion ILIKE '%' || p_q || '%');
    
    SELECT json_agg(
        json_build_object(
            'id', mc.id,
            'fecha', mc.fecha,
            'tipo', mc.tipo,
            'nombre_tipo', tmc.nombre,
            'descripcion', mc.descripcion,
            'moneda', mc.moneda,
            'nombre_moneda', m.nombre,
            'monto', mc.monto
        )
    ) INTO items
    FROM movimientoCuenta mc
    LEFT JOIN tipoMovimientoCuenta tmc ON mc.tipo = tmc.id
    LEFT JOIN moneda m ON mc.moneda = m.id
    WHERE mc.cuenta_id = p_account_id
      AND (p_from_date IS NULL OR mc.fecha >= p_from_date)
      AND (p_to_date IS NULL OR mc.fecha <= p_to_date)
      AND (p_type IS NULL OR mc.tipo = p_type)
      AND (p_q IS NULL OR mc.descripcion ILIKE '%' || p_q || '%')
    ORDER BY mc.fecha DESC
    LIMIT page_size
    OFFSET v_offset;
    
    IF items IS NULL THEN
        items := '[]';
    END IF;
END;
$$;

