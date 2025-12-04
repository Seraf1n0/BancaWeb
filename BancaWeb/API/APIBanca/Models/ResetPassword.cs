namespace APIBanca.Models
{
    public class ResetPasswordM{
        public string? user_id { get; set; }
        public string proposito { get; set; }
        public string codigo_hash { get; set; }
        public string nueva_contrasena { get; set; }
    }
}
