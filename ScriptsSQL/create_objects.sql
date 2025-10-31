--- ESTE SCRIPT ES PARA LOS OBJETOS DE LA BASE DE DATOS, NO ES 100% EJECUTABLE 
CREATE TABLE public.Auditoria (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL UNIQUE,
  usuario_id uuid NOT NULL,
  accion text NOT NULL,
  detalles json NOT NULL,
  fecha timestamp without time zone NOT NULL,
  CONSTRAINT Auditoria_pkey PRIMARY KEY (id),
  CONSTRAINT Auditoria_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuario(id)
);
CREATE TABLE public.Otps (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  codigo_hash text NOT NULL,
  proposito text NOT NULL,
  fecha_expiracion timestamp without time zone NOT NULL,
  fecha_consumido timestamp without time zone,
  fecha_creacion timestamp without time zone NOT NULL DEFAULT now(),
  CONSTRAINT Otps_pkey PRIMARY KEY (id),
  CONSTRAINT Otps_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuario(id)
);
CREATE TABLE public.apiKey (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  clave_hash text NOT NULL,
  etiqueta text NOT NULL,
  activa boolean NOT NULL,
  fecha_creacion timestamp without time zone NOT NULL DEFAULT now(),
  user_id uuid,
  CONSTRAINT apiKey_pkey PRIMARY KEY (id),
  CONSTRAINT apiKey_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.usuario(id)
);
CREATE TABLE public.cuenta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  usuario_id uuid NOT NULL,
  iban text NOT NULL,
  alias text NOT NULL,
  tipoCuenta uuid NOT NULL,
  moneda uuid NOT NULL,
  saldo double precision NOT NULL DEFAULT '0'::double precision,
  estado uuid NOT NULL,
  fecha_creacion text,
  fecha_actualizacion text,
  CONSTRAINT cuenta_pkey PRIMARY KEY (id),
  CONSTRAINT cuenta_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuario(id),
  CONSTRAINT cuenta_tipoCuenta_fkey FOREIGN KEY (tipoCuenta) REFERENCES public.tipoCuenta(id),
  CONSTRAINT cuenta_moneda_fkey FOREIGN KEY (moneda) REFERENCES public.moneda(id),
  CONSTRAINT cuenta_estado_fkey FOREIGN KEY (estado) REFERENCES public.estadoCuenta(id)
);
CREATE TABLE public.estadoCuenta (
  id uuid NOT NULL DEFAULT gen_random_uuid() UNIQUE,
  nombre text NOT NULL,
  descripcion text NOT NULL,
  CONSTRAINT estadoCuenta_pkey PRIMARY KEY (id)
);
CREATE TABLE public.moneda (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  nombre text NOT NULL,
  iso text NOT NULL,
  CONSTRAINT moneda_pkey PRIMARY KEY (id)
);
CREATE TABLE public.movimientoCuenta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  cuenta_id uuid NOT NULL,
  fecha timestamp without time zone,
  tipo uuid NOT NULL,
  descripcion text,
  moneda uuid NOT NULL,
  monto double precision NOT NULL,
  CONSTRAINT movimientoCuenta_pkey PRIMARY KEY (id),
  CONSTRAINT movimientoCuenta_cuenta_id_fkey FOREIGN KEY (cuenta_id) REFERENCES public.cuenta(id),
  CONSTRAINT movimientoCuenta_tipo_fkey FOREIGN KEY (tipo) REFERENCES public.tipoMovimientoCuenta(id),
  CONSTRAINT movimientoCuenta_moneda_fkey FOREIGN KEY (moneda) REFERENCES public.moneda(id)
);
CREATE TABLE public.movimientoTarjeta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  cuenta_id uuid NOT NULL,
  fecha timestamp without time zone,
  tipo uuid NOT NULL,
  descripcion text,
  moneda uuid NOT NULL,
  monto double precision NOT NULL,
  CONSTRAINT movimientoTarjeta_pkey PRIMARY KEY (id),
  CONSTRAINT movimientoTarjeta_cuenta_id_fkey FOREIGN KEY (cuenta_id) REFERENCES public.cuenta(id),
  CONSTRAINT movimientoTarjeta_tipo_fkey FOREIGN KEY (tipo) REFERENCES public.tipoMovimientoTarjeta(id),
  CONSTRAINT movimientoTarjeta_moneda_fkey FOREIGN KEY (moneda) REFERENCES public.moneda(id)
);
CREATE TABLE public.rol (
  nombre character varying NOT NULL,
  descripcion character varying NOT NULL,
  Rol smallint GENERATED ALWAYS AS IDENTITY NOT NULL,
  CONSTRAINT rol_pkey PRIMARY KEY (Rol)
);
CREATE TABLE public.tarjeta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  usuario_id uuid,
  tipo uuid NOT NULL,
  numero_enmascarado text NOT NULL,
  fecha_expiracion text NOT NULL,
  cvv_hash text NOT NULL,
  pin_hash text NOT NULL,
  moneda uuid NOT NULL,
  limite_credito double precision NOT NULL,
  saldo_actual double precision,
  fecha_creacion timestamp without time zone,
  fecha_actualizacion timestamp without time zone,
  CONSTRAINT tarjeta_pkey PRIMARY KEY (id),
  CONSTRAINT tarjeta_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuario(id),
  CONSTRAINT tarjeta_tipo_fkey FOREIGN KEY (tipo) REFERENCES public.tipoTarjeta(id),
  CONSTRAINT tarjeta_moneda_fkey FOREIGN KEY (moneda) REFERENCES public.moneda(id)
);
CREATE TABLE public.tipoCuenta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  nombre text NOT NULL,
  descripcion text NOT NULL,
  CONSTRAINT tipoCuenta_pkey PRIMARY KEY (id)
);
CREATE TABLE public.tipoIdentificacion (
  nombre text NOT NULL,
  descripcion text NOT NULL,
  id smallint NOT NULL,
  CONSTRAINT tipoIdentificacion_pkey PRIMARY KEY (id)
);
CREATE TABLE public.tipoMovimientoCuenta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  nombre text NOT NULL,
  descripcion text NOT NULL,
  CONSTRAINT tipoMovimientoCuenta_pkey PRIMARY KEY (id)
);
CREATE TABLE public.tipoMovimientoTarjeta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  nombre text NOT NULL,
  descripcion text NOT NULL,
  CONSTRAINT tipoMovimientoTarjeta_pkey PRIMARY KEY (id)
);
CREATE TABLE public.tipoTarjeta (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  nombre text NOT NULL,
  descripcion text NOT NULL,
  CONSTRAINT tipoTarjeta_pkey PRIMARY KEY (id)
);
CREATE TABLE public.usuario (
  id uuid NOT NULL DEFAULT gen_random_uuid(),
  identificacion text NOT NULL,
  nombre text NOT NULL,
  apellido text NOT NULL,
  correo text,
  telefono text NOT NULL,
  usuario text,
  contrasena_hash text,
  fecha_creacion timestamp without time zone,
  fecha_actualizacion timestamp without time zone,
  rol smallint,
  tipo_identificacion smallint NOT NULL,
  CONSTRAINT usuario_pkey PRIMARY KEY (id),
  CONSTRAINT usuario_rol_fkey FOREIGN KEY (rol) REFERENCES public.rol(Rol),
  CONSTRAINT usuario_tipo_identificacion_fkey FOREIGN KEY (tipo_identificacion) REFERENCES public.tipoIdentificacion(id)
);