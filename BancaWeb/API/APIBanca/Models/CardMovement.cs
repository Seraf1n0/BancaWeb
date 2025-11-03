namespace APIBanca.Models
{
    public class CardMovement{
        public Guid CardID { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public Guid tipo { get; set; }
        public string descripcion { get; set; }
        public Guid moneda { get; set; }
        public decimal monto { get; set; }
    }
}