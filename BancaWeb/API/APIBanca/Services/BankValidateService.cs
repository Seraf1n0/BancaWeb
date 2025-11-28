using APIBanca.Models; 

namespace APIBanca.Services
{
    public class BankValidateService
    {
        private readonly BankValidateRepository _repository;

        public BankValidateService(BankValidateRepository repository)
        {
            _repository = repository;
        }

        // Logica para verificar la cuenta bancaria
        public async Task<BankValidate> VerificarCuenta(string iban) 
        {
            var resultado = await _repository.VerificarCuenta(iban);
            if (resultado == null)
            {
                throw new Exception("No se pudo verificar la cuenta bancaria, " + iban + ".");
            }

            return resultado;
        }
    }
}