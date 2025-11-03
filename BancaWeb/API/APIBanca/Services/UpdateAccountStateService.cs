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

        public async Task<bool> SetStatus(Guid accountId, string nuevoEstadoInput)
        {
            var resolved = await _repository.GetEstadoIdByNombreAsync(nuevoEstadoInput);
            if (resolved == null)
                throw new ArgumentException("Estado no válido");

            return await _repository.SetStatusAsync(accountId, resolved.Value);
        }
    }
}
