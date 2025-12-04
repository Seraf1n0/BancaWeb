namespace APIBanca.Models
{
    public class TransferInterbankResponse
    {
        public Guid transfer_id { get; set; }
        public string receipt_number { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}