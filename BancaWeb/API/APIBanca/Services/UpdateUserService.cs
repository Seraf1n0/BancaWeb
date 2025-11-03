using APIBanca.Models;

namespace APIBanca.Services
{
    public class UpdateUserService
    {
        private readonly UpdateUserRepository _repository;

        public UpdateUserService(UpdateUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ActualizarUsuario(string idUsuario, string? nombre, string? apellido, string? correo,
        string? usuario, int? rol)
        {
            return await _repository.ActualizarUsuario(idUsuario, nombre, apellido, correo, usuario, rol);
        }
    }
}
