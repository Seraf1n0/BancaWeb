using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class CreateAccountService
    {
        private readonly CreateAccountRepository _repository;
        public CreateAccountService(CreateAccountRepository repository)
        {
            _repository = repository;
        }

        // Método para crear una cuenta asíncrono
        // Usa el modelo CreateCuenta
        public async Task<Guid> CrearCuentaAsync(CreateCuenta cuenta)
        {
            return await _repository.CreateAccountAsync(cuenta);
        }
    }
}