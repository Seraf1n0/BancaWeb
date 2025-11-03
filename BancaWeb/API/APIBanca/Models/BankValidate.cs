namespace APIBanca.Models
{
    public class BankValidate
    {
        public bool existeCuenta { get; set; }
        public string propietarioCuenta { get; set; }
        public string idPropietario { get; set; }
    }

    public class IbanAccount
    {
        public string iban { get; set; }
    }
}