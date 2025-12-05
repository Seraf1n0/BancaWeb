using System.ComponentModel.DataAnnotations;

namespace APIBanca.Models
{
    public class TransferInterbankRequest
    {
        [Required]
        public Guid from_account_id { get; set; }  
        
        [Required]
        public string to_iban { get; set; }  
        
        [Required]
        public decimal amount { get; set; }
        
        
        public Guid currency { get; set; }
        
        public string? descripcion { get; set; }
    }
}