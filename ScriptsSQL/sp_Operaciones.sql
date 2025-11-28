-- SPs para operaciones o transferencias bancarias
CREATE OR REPLACE FUNCTION sp_transfer_create_internal(
    p_from_account_id UUID,
    p_to_account_id UUID,
    p_amount DECIMAL(18,2),
    p_currency UUID,
    p_descripcion VARCHAR,
    p_user_id UUID,
    OUT transfer_id UUID,
    OUT receipt_number VARCHAR,
    OUT status VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_cuenta_origen RECORD;
    v_cuenta_destino RECORD;
    v_nuevo_saldo_origen DECIMAL(18,2);
    v_nuevo_saldo_destino DECIMAL(18,2);
    v_movement_id UUID;
    v_tipo_debito UUID;
    v_tipo_credito UUID;
BEGIN
    IF p_from_account_id = p_to_account_id THEN
        RAISE EXCEPTION 'No se puede transferir a la misma cuenta';
    END IF;
    
    IF p_amount <= 0 THEN
        RAISE EXCEPTION 'El monto de transferencia debe ser mayor a cero';
    END IF;
    
    SELECT id, usuario_id, saldo, moneda, estado, iban
    INTO v_cuenta_origen
    FROM cuenta 
    WHERE id = p_from_account_id
    FOR UPDATE;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Cuenta de origen no encontrada';
    END IF;
    
    SELECT id, usuario_id, saldo, moneda, estado, iban
    INTO v_cuenta_destino
    FROM cuenta 
    WHERE id = p_to_account_id
    FOR UPDATE;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Cuenta de destino no encontrada';
    END IF;
    
    IF v_cuenta_origen.usuario_id != p_user_id THEN
        RAISE EXCEPTION 'No tiene permisos para transferir desde esta cuenta';
    END IF;
    
    IF v_cuenta_origen.estado != (SELECT id FROM "estadoCuenta" WHERE nombre = 'activa') THEN
        RAISE EXCEPTION 'La cuenta de origen no está activa';
    END IF;
    
    IF v_cuenta_destino.estado != (SELECT id FROM "estadoCuenta" WHERE nombre = 'activa') THEN
        RAISE EXCEPTION 'La cuenta de destino no está activa';
    END IF;
    
    IF v_cuenta_origen.moneda != p_currency THEN
        RAISE EXCEPTION 'La moneda de la cuenta de origen no coincide con la moneda de transferencia';
    END IF;
    
    IF v_cuenta_destino.moneda != p_currency THEN
        RAISE EXCEPTION 'La moneda de la cuenta de destino no coincide con la moneda de transferencia';
    END IF;
    
    IF v_cuenta_origen.saldo < p_amount THEN
        RAISE EXCEPTION 'Saldo insuficiente. Saldo actual: %, Monto solicitado: %', 
                        v_cuenta_origen.saldo, p_amount;
    END IF;
    
    SELECT id INTO v_tipo_debito FROM "tipoMovimientoCuenta" WHERE nombre = 'Débito';
    SELECT id INTO v_tipo_credito FROM "tipoMovimientoCuenta" WHERE nombre = 'Crédito';
    
    v_nuevo_saldo_origen := v_cuenta_origen.saldo - p_amount;
    v_nuevo_saldo_destino := v_cuenta_destino.saldo + p_amount;
    
    receipt_number := 'TFR-' || to_char(NOW(), 'YYYYMMDD') || '-' || substr(replace(gen_random_uuid()::text, '-', ''), 1, 8);
    
    UPDATE cuenta 
    SET saldo = v_nuevo_saldo_origen, fecha_actualizacion = NOW()
    WHERE id = p_from_account_id;
    
    INSERT INTO "movimientoCuenta" (
        cuenta_id,
        fecha,
        tipo,
        descripcion,
        moneda,
        monto
    ) VALUES (
        p_from_account_id,
        NOW(),
        v_tipo_debito,
        COALESCE(p_descripcion, 'Transferencia a ' || v_cuenta_destino.iban),
        p_currency,
        p_amount
    )
    RETURNING id INTO v_movement_id;
    
    transfer_id := v_movement_id;
    
    UPDATE cuenta 
    SET saldo = v_nuevo_saldo_destino, fecha_actualizacion = NOW()
    WHERE id = p_to_account_id;
    
    INSERT INTO "movimientoCuenta" (
        cuenta_id,
        fecha,
        tipo,
        descripcion,
        moneda,
        monto
    ) VALUES (
        p_to_account_id,
        NOW(),
        v_tipo_credito,
        COALESCE(p_descripcion, 'Transferencia de ' || v_cuenta_origen.iban),
        p_currency,
        p_amount
    );
    
    status := 'COMPLETED';
    
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'auditoria') THEN
        INSERT INTO Auditoria (usuario_id, accion, detalles, fecha)
        VALUES (
            p_user_id,
            'TRANSFERENCIA_INTERNA',
            json_build_object(
                'from_account', p_from_account_id,
                'to_account', p_to_account_id,
                'amount', p_amount,
                'currency', p_currency,
                'receipt_number', receipt_number,
                'previous_balance_origin', v_cuenta_origen.saldo,
                'new_balance_origin', v_nuevo_saldo_origen,
                'previous_balance_dest', v_cuenta_destino.saldo,
                'new_balance_dest', v_nuevo_saldo_destino
            ),
            NOW()
        );
    END IF;
    
END;
$$;

CREATE OR REPLACE FUNCTION sp_bank_validate_account(
    p_iban VARCHAR
)
RETURNS TABLE (
    exists_account BOOLEAN,
    name VARCHAR,
    identification VARCHAR,
    currency VARCHAR,
    debit BOOLEAN,
    credit BOOLEAN
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_name VARCHAR;
    v_identification VARCHAR;
    v_currency VARCHAR;
    v_user_id UUID;
BEGIN
    SELECT (u.nombre || ' ' || u.apellido), u.identificacion, m.nombre, u.id
    INTO v_name, v_identification, v_currency, v_user_id
    FROM cuenta c
    INNER JOIN usuario u ON c.usuario_id = u.id
    INNER JOIN moneda m ON c.moneda = m.id
    WHERE c.iban = p_iban
      AND c.estado = (SELECT id FROM estadoCuenta WHERE nombre = 'activa');

    IF NOT FOUND THEN
    -- si no encontramos retornamos false
        exists_account := FALSE;
        name := NULL;
        identification := NULL;
        currency := NULL;
        debit := FALSE;
        credit := FALSE;
        RETURN NEXT;
        RETURN;
    END IF;
    -- En caso de encontrarla buscamos si tiene tarjeta credito valida:
    -- A este punto deberiamos tener la info y la cuenta deberia aceptar debito
    credit := (
            SELECT EXISTS (
                SELECT 1
                FROM tarjeta t
                WHERE t.usuario_id = v_user_id
            )
        );

    -- Respuesta correcta:
    exists_account := TRUE;
    name := v_name;
    identification := v_identification;
    currency := v_currency;
    debit := TRUE;
    RETURN NEXT;
END;
$$;