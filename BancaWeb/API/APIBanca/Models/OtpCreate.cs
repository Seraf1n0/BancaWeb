namespace APIBanca.Models

{
    public class OtpCreateM
    {
        public string? usuario_id { get; set; }
        public string codigo_hash { get; set; }
        public string proposito { get; set; }
        public int tiempoExpiracionSegundos { get; set; }
    }
}