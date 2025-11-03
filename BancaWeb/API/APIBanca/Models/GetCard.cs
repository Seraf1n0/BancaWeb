namespace APIBanca.Models
{
    public class GetCard
    {
        public Guid id { get; set; }
        public Guid usuarioID { get; set; }
        public Guid tipo { get; set; } 
        public string nombreTarjeta { get; set; }
        public string numeroEnmascarado { get; set; }
        public string fechaExpiracion { get; set; }
        public Guid moneda { get; set; }
        public string nombreMoneda { get; set; }
        public decimal limiteCredito { get; set; }
        public decimal saldoActual { get; set; }
        public decimal saldoDisponible { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }
}