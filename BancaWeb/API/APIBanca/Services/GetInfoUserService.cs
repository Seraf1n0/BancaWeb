using APIBanca.Models;

namespace APIBanca.Services
{
    public class GetInfoUserService
    {
        private readonly InfoUserRepository _repository;

        public GetInfoUserService(InfoUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> ObtenerUsuarioID(string identificacion)
        {
            return await _repository.ObtenerUsuarioID(identificacion);
        }
    }
}
