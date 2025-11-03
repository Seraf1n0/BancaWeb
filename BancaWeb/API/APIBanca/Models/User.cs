namespace APIBanca.Models
{
    public class User
    {
        public int p_tipo_identificacion { get; set; }
        public string? p_identificacion { get; set; }
        public string? p_nombre { get; set; }
        public string? p_apellido { get; set; }
        public string? p_correo { get; set; }
        public string? p_usuario { get; set; }
        public string? p_telefono { get; set; }
        public string? p_contrasena_hash { get; set; }
        public int p_rol { get; set; }
    }  
}
