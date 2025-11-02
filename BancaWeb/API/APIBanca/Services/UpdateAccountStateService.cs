using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class UpdateAccountStateService
    {
        private readonly UpdateAccountStateRepository _repository;

        public UpdateAccountStateService(UpdateAccountStateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ActualizarEstadoCuenta(UpdateEstadoCuenta updateEstadoCuenta)
        {
            return await _repository.ActualizarEstadoCuenta(updateEstadoCuenta);
        }
    }
}
