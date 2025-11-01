namespace APIBanca.Models
{
    public class UpdateUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Usuario { get; set; }
        public int? Rol { get; set; }
    }
}
