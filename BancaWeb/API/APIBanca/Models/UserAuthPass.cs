namespace APIBanca.Models
{
    public class UserAuthResponse
    {
        public Guid UserId { get; set; }
        public string ContrasenaHash { get; set; } = string.Empty;
        public int Rol { get; set; }
    }
}