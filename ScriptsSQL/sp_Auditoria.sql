CREATE OR REPLACE FUNCTION sp_audit_log(
  p_accion    VARCHAR,
  p_entidad   VARCHAR,
  p_actor_user_id UUID DEFAULT NULL,
  p_entidad_id   UUID DEFAULT NULL,
  p_detalles     JSON DEFAULT NULL,
  OUT audit_id   INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
  INSERT INTO Auditoria (
    usuario_id,
    accion,
    detalles,
    fecha
  ) VALUES (
    p_actor_user_id,
    p_accion,
    json_build_object(
      'entidad', p_entidad,
      'entidad_id', p_entidad_id,
      'detalles_adicionales', p_detalles
    ),
    NOW()
  )
  RETURNING id INTO audit_id;
END;
$$;

CREATE OR REPLACE FUNCTION sp_audit_list_by_user(
    p_user_id UUID
)
RETURNS TABLE (
    id INTEGER,
    accion VARCHAR,
    detalles JSON,
    fecha TIMESTAMP,
    nombre_usuario VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        a.id,
        a.accion,
        a.detalles,
        a.fecha,
        (u.nombre || ' ' || u.apellido)::VARCHAR as nombre_usuario
    FROM Auditoria a
    LEFT JOIN usuario u ON a.usuario_id = u.id
    WHERE a.usuario_id = p_user_id
    ORDER BY a.fecha DESC;
END;
$$;