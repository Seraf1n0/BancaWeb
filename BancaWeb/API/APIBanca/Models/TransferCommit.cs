namespace APIBanca.Models
{
    public class TransferCommit
    {
        public String Id { get; set; }
        public String From { get; set; }
        public String To { get; set; }
        public Decimal Amount { get; set; }
        public String Currency { get; set; }
      
    }
}