namespace APIBanca.Models
{
    public class TransferIntent
    {
        public String id { get; set; }
        public String from { get; set;}
        public String to { get; set; }
        public Decimal amount { get; set; }
        public String currency { get; set; }
        
    }
}