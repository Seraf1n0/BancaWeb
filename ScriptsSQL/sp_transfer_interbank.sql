-- Stored Procedure para crear transferencia interbancaria
-- Se ejecuta cuando el backend recibe la solicitud del frontend
-- Guarda el movimiento en la BD con estado 'pending' hasta que el Banco Central confirme

CREATE OR REPLACE FUNCTION sp_transfer_create_interbank(
    p_from_iban VARCHAR(28),
    p_to_iban VARCHAR(28),
    p_amount NUMERIC(15,2),
    p_currency VARCHAR(10),
    p_descripcion TEXT,
    p_user_id UUID
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
    v_currency_id UUID;
    v_movement_id UUID;
    v_current_balance NUMERIC(15,2);
    v_receipt TEXT;
BEGIN
    -- 1. Obtener el ID de la cuenta origen usando el IBAN
    SELECT id, saldo INTO v_from_account_id, v_current_balance
    FROM cuenta
    WHERE iban = p_from_iban AND usuario_id = p_user_id AND estado = 'activa';

    IF v_from_account_id IS NULL THEN
        RAISE EXCEPTION 'La cuenta origen no existe o no pertenece al usuario';
    END IF;

    -- 2. Validar que haya fondos suficientes
    IF v_current_balance < p_amount THEN
        RAISE EXCEPTION 'Fondos insuficientes. Saldo actual: %, Requerido: %', v_current_balance, p_amount;
    END IF;

    -- 3. Obtener el ID de la moneda
    SELECT id INTO v_currency_id
    FROM moneda
    WHERE codigo = p_currency;

    IF v_currency_id IS NULL THEN
        RAISE EXCEPTION 'Moneda no válida: %', p_currency;
    END IF;

    -- 4. Generar ID único para el movimiento
    v_movement_id := gen_random_uuid();
    v_receipt := 'IB-' || UPPER(SUBSTRING(v_movement_id::TEXT, 1, 8));

    -- 5. Insertar movimiento en movimiento_cuenta con estado 'pending'
    --    y guardando el IBAN destino en la columna toaccount
    INSERT INTO movimiento_cuenta (
        id,
        cuenta_id,
        tipo_movimiento_id,
        monto,
        saldo_anterior,
        saldo_nuevo,
        descripcion,
        fecha_movimiento,
        numero_comprobante,
        toaccount,
        estado
    )
    VALUES (
        v_movement_id,
        v_from_account_id,
        (SELECT id FROM tipo_movimiento WHERE nombre = 'Transferencia Interbancaria' LIMIT 1),
        p_amount,
        v_current_balance,
        v_current_balance, -- No debitamos aún, esperamos confirmación del Banco Central
        COALESCE(p_descripcion, 'Transferencia interbancaria a ' || p_to_iban),
        NOW(),
        v_receipt,
        p_to_iban, -- Guardamos el IBAN destino
        'pending' -- Estado inicial, cambiará cuando el Banco Central confirme
    );

    -- 6. Retornar resultado
    RETURN QUERY
    SELECT 
        v_movement_id,
        v_receipt,
        'pending'::TEXT;
END;
$$;
