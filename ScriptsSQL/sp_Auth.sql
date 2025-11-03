-- SPs de Auth y seguridad
CREATE OR REPLACE FUNCTION sp_auth_user_get_by_username_or_email(
    p_username_or_email VARCHAR
)
RETURNS TABLE (
    user_id UUID,
    contrasena_hash VARCHAR,
    rol int
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.id,
        u.contrasena_hash::varchar,
        u.rol::int
    FROM usuario u
    WHERE u.usuario = p_username_or_email 
       OR u.correo = p_username_or_email;
END;
$$;


CREATE OR REPLACE FUNCTION sp_api_key_is_active(
    p_api_key_hash VARCHAR
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
DECLARE
    v_is_active BOOLEAN;
BEGIN
    SELECT activa INTO v_is_active
    FROM apiKey
    WHERE clave_hash = p_api_key_hash;
    
    RETURN COALESCE(v_is_active, FALSE);
END;
$$;

CREATE OR REPLACE FUNCTION sp_otp_create(
    p_user_id UUID,
    p_proposito text,
    p_expires_in_seconds INTEGER,
    p_codigo_hash text,
    OUT otp_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO "Otps" (
        usuario_id,
        codigo_hash,
        proposito,
        fecha_expiracion,
        fecha_creacion
    ) VALUES (
        p_user_id,
        p_codigo_hash,
        p_proposito,
        NOW() + (p_expires_in_seconds || ' seconds')::INTERVAL,
        NOW()
    )
    RETURNING id INTO otp_id;
END;
$$;


CREATE OR REPLACE FUNCTION sp_otp_consume(
    p_user_id UUID,
    p_proposito text,
    p_codigo_hash text,
    OUT consumed BOOLEAN
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_otp_id UUID;
    v_is_expired BOOLEAN;
BEGIN
    consumed := FALSE;
    
    -- Buscar OTP válido y no consumido
    SELECT id, (fecha_expiracion < NOW()) 
    INTO v_otp_id, v_is_expired
    FROM "Otps"
    WHERE usuario_id = p_user_id
      AND proposito = p_proposito
      AND codigo_hash = p_codigo_hash
      AND fecha_consumido IS NULL;
    
    -- En caso de que el otp exista y aun no se haya expirado
    IF v_otp_id IS NOT NULL AND NOT v_is_expired THEN
        UPDATE "Otps" 
        SET fecha_consumido = NOW()
        WHERE id = v_otp_id;
        
        consumed := TRUE;
    END IF;
END;
$$;


