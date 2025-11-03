namespace APIBanca.Models
{
    public class CreateCard{
        public Guid usuarioID { get; set; }
        public Guid tipo { get; set; }
        public string numeroEnmascarado { get; set; }
        public string fechaExpiracion { get; set; }
        public string cvv_encriptado { get; set; }
        public string pin_encriptado { get; set; }
        public Guid moneda { get; set; }
        public decimal limiteCredito { get; set; }
        public decimal saldoActual { get; set; }
    }
}