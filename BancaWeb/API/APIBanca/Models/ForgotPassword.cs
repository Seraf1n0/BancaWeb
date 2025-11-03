namespace APIBanca.Models
{
    public class ForgotPasswordM
    {
        public string user_id { get; set; }
        public string proposito { get; set; }
        public int expiresInt { get; set; }
        public string codigo_hash { get; set; }
    }
}