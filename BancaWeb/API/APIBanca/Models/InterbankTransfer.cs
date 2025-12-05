using System.ComponentModel.DataAnnotations;

namespace APIBanca.Models
{
    public class InterbankTransferRequest
    {
        [Required] 
        public string from { get; set; } = string.Empty;

        [Required] 
        public string to { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)] 
        public decimal amount { get; set; }

        [Required]
        public string currency { get; set; } = string.Empty;

        public string? description { get; set; }
    }

    public class InterbankTransferResponse
    {
        public string id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty; 
        public string? reason { get; set; }
        public string? receipt_number { get; set; }
    }

    public class WebSocketMessage
    {
        public string type { get; set; } = string.Empty;
        public object data { get; set; } = new();
    }

    public class TransferIntentData
    {
        public string id { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public decimal amount { get; set; }
        public string currency { get; set; } = string.Empty;
    }

    public class TransferResultData
    {
        public string id { get; set; } = string.Empty;
        public bool ok { get; set; }
        public string? reason { get; set; }
    }

    public class TransferCommitData
    {
        public string id { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public decimal amount { get; set; }
        public string currency { get; set; } = string.Empty;
    }

    public class TransferRejectData
    {
        public string id { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
    }
}
