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

        public async Task<BankValidate> VerificarCuenta(string iban) 
        {
            return await _repository.VerificarCuenta(iban);
        }
    }
}