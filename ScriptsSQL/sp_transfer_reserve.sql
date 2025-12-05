CREATE OR REPLACE FUNCTION sp_transfer_create_interbank(
    p_from_iban VARCHAR(28),
    p_to_iban VARCHAR(28),
    p_amount NUMERIC(15,2),
    p_currency VARCHAR(10),
    p_descripcion TEXT
)
RETURNS TABLE(
    transfer_id UUID,
    receipt_number TEXT,
    status TEXT
)
LANGUAGE plpgsql
AS $$

DECLARE
    v_from_account_id UUID;
    v_user_id UUID;
    v_currency_id UUID;
    v_movement_id UUID;
    v_current_balance NUMERIC(15,2);
    v_receipt TEXT;
BEGIN
    -- 1. Obtener cuenta y usuario AUTOMÁTICAMENTE
    SELECT id, usuario_id, saldo
    INTO v_from_account_id, v_user_id, v_current_balance
    FROM cuenta
    WHERE iban = p_from_iban;

    IF v_from_account_id IS NULL THEN
        RAISE EXCEPTION 'La cuenta origen no existe';
    END IF;

    -- 2. Validar fondos
    IF v_current_balance < p_amount THEN
        RAISE EXCEPTION 'Fondos insuficientes (saldo %) para enviar %', v_current_balance, p_amount;
    END IF;

    -- 3. Obtener moneda
    SELECT id INTO v_currency_id
    FROM moneda
    WHERE iso = p_currency;

    IF v_currency_id IS NULL THEN
        RAISE EXCEPTION 'Código ISO de moneda inválido (%).', p_currency;
    END IF;

    -- 4. Generar identificador del movimiento
    v_movement_id := gen_random_uuid();
    v_receipt := 'IB-' || UPPER(SUBSTRING(v_movement_id::TEXT, 1, 8));

    -- 5. Insertar movimiento en movimientoCuenta
    INSERT INTO "movimientoCuenta" (
        id,
        cuenta_id,
        fecha,
        tipo,
        descripcion,
        moneda,
        monto,
        toaccount,
        estado
    )
    VALUES (
        v_movement_id,
        v_from_account_id,
        NOW(),
        (SELECT id FROM "tipoMovimientoCuenta" WHERE nombre = 'Transferencia Interbancaria' LIMIT 1),
        COALESCE(p_descripcion, 'Transferencia interbancaria a ' || p_to_iban),
        v_currency_id,
        p_amount,
        p_to_iban,
        'pending'
    );

    -- 6. Retorno final
    RETURN QUERY
    SELECT 
        v_movement_id,
        v_receipt,
        'pending';

END;
$$;