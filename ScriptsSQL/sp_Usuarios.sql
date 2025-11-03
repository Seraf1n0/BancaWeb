-- Sps para Usuarios
CREATE OR REPLACE FUNCTION sp_users_create(
    p_tipo_identificacion int,
    p_identificacion VARCHAR,
    p_nombre VARCHAR,
    p_apellido VARCHAR,
    p_correo VARCHAR,
    p_telefono VARCHAR,
    p_usuario VARCHAR,
    p_contrasena_hash VARCHAR,
    p_rol int,
    OUT user_id UUID
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificamos que el identficicacion, correo y nombre de usuario
    IF EXISTS (SELECT 1 FROM usuario WHERE identificacion = p_identificacion) THEN
        RAISE EXCEPTION 'La identificación ya está registrada';
    END IF;
    
    IF EXISTS (SELECT 1 FROM usuario WHERE correo = p_correo) THEN
        RAISE EXCEPTION 'El correo electrónico ya está registrado';
    END IF;
    
    IF EXISTS (SELECT 1 FROM usuario WHERE usuario = p_usuario) THEN
        RAISE EXCEPTION 'El nombre de usuario ya está en uso';
    END IF;
    
    INSERT INTO usuario (
        tipo_identificacion,
        identificacion,
        nombre,
        apellido,
        correo,
        telefono,
        usuario,
        contrasena_hash,
        rol,
        fecha_creacion,
        fecha_actualizacion
    ) VALUES (
        p_tipo_identificacion,
        p_identificacion,
        p_nombre,
        p_apellido,
        p_correo,
        p_telefono,
        p_usuario,
        p_contrasena_hash,
        p_rol,
        NOW(),
        NOW()
    )
    RETURNING id INTO user_id;
END;
$$;

CREATE OR REPLACE FUNCTION sp_users_get_by_identification(
    p_identificacion VARCHAR
)
RETURNS TABLE (
    id UUID,
    nombre VARCHAR,
    apellido VARCHAR,
    correo VARCHAR,
    usuario VARCHAR,
    rol int
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.id,
        u.nombre,
        u.apellido,
        u.correo,
        u.usuario,
        u.rol
    FROM usuario u
    WHERE u.identificacion = p_identificacion;
END;
$$;

CREATE OR REPLACE FUNCTION sp_api_key_create(
    p_api_hash VARCHAR,
    p_label VARCHAR,
    p_active BOOLEAN,
    p_user_id UUID,
    OUT created BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    created := FALSE;
    
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE id = p_user_id) THEN
        RAISE EXCEPTION 'Usuario no encontrado';
    END IF;

    INSERT INTO apiKey (
        clave_hash,
        etiqueta,
        activa,
        fecha_creacion,
        user_id
    ) VALUES (
        p_api_hash,
        p_label,
        p_active,
        NOW(),
        p_user_id
    );

    created := TRUE;
    
EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'La API Key ya existe';
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al crear la API Key: %', SQLERRM;
END;
$$;

CREATE OR REPLACE FUNCTION sp_users_update(
    p_user_id UUID,
    p_nombre VARCHAR DEFAULT NULL,
    p_apellido VARCHAR DEFAULT NULL,
    p_correo VARCHAR DEFAULT NULL,
    p_usuario VARCHAR DEFAULT NULL,
    p_rol int DEFAULT NULL,
    OUT updated BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    updated := FALSE;
    
    -- Verificacion de unicidad de campos de datos personales
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE id = p_user_id) THEN
        RAISE EXCEPTION 'Usuario no encontrado';
    END IF;
    
    IF p_correo IS NOT NULL AND EXISTS (
        SELECT 1 FROM usuario WHERE correo = p_correo AND id != p_user_id
    ) THEN
        RAISE EXCEPTION 'El correo electrónico ya está registrado por otro usuario';
    END IF;
    
    IF p_usuario IS NOT NULL AND EXISTS (
        SELECT 1 FROM usuario WHERE usuario = p_usuario AND id != p_user_id
    ) THEN
        RAISE EXCEPTION 'El nombre de usuario ya está en uso por otro usuario';
    END IF;
    
    UPDATE usuario 
    SET 
        nombre = COALESCE(p_nombre, nombre),
        apellido = COALESCE(p_apellido, apellido),
        correo = COALESCE(p_correo, correo),
        usuario = COALESCE(p_usuario, usuario),
        rol = COALESCE(p_rol, rol),
        fecha_actualizacion = NOW()
    WHERE id = p_user_id;
    
    updated := TRUE;
END;
$$;



CREATE OR REPLACE FUNCTION sp_users_delete(
    p_user_id UUID,
    OUT deleted BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    deleted := FALSE;
    
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE id = p_user_id) THEN
        RAISE EXCEPTION 'Usuario no encontrado';
    END IF;
    
    -- Delete on cascade 
    DELETE FROM Otps WHERE usuario_id = p_user_id;
    
    DELETE FROM movimientoTarjeta 
    WHERE cuenta_id IN (SELECT id FROM tarjeta WHERE usuario_id = p_user_id);
    
    DELETE FROM tarjeta WHERE usuario_id = p_user_id;
    
    DELETE FROM movimientoCuenta 
    WHERE cuenta_id IN (SELECT id FROM cuenta WHERE usuario_id = p_user_id);
    
    DELETE FROM cuenta WHERE usuario_id = p_user_id;
    
    DELETE FROM usuario WHERE id = p_user_id;
    
    deleted := TRUE;
END;
$$;

