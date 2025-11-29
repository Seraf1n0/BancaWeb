namespace APIBanca.Models
{
    // Response para la validación de cuenta bancaria
    public class BankValidate
    {
        public bool exists { get; set; }
        public Info info { get; set; }
    }
    // Info obj para detalles del propietario
    public class Info
    {
        public string name { get; set; }
        public string identification { get; set; }
        public string currency { get; set; } // Puede ser CRC o USD
        // Flags para saber si tiene débito o crédito (usado en transferencias)
        public bool debit { get; set; }
        public bool credit { get; set; }
    }

    // Formato de un IBAN:
    // Banca Prometedores usa: CR01B01XXXXYYYYzzzzzzzzzzzz
    public class IbanAccount
    {
        public string iban { get; set; }
    }

    // Unauthorized al validar cuenta bancaria
    public class BankValidateUnauthorized
    {
        public string error { get; set; }
        public string message { get; set; }
    }

    // Notfound al validar cuenta bancaria
    public class BankValidateNotFound
    {
        public string error { get; set; }
        public string message { get; set; }
    }

    public class BankValidateServerError
    {
        public string error { get; set; }
        public string message { get; set; }
    }
}