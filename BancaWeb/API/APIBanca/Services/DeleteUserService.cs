using APIBanca.Models;

namespace APIBanca.Services
{
    public class DeleteUserService
    {
        private readonly DeleteUserRepository _repository;

        public DeleteUserService(DeleteUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> EliminarUsuario(string idUsuario)
        {
            return await _repository.EliminarUsuario(idUsuario);
        }

    }
}
