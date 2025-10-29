using System.Text.Json;

namespace APIBanca.Services
{
    public class UsuarioService
    {
        private readonly AuthUsuarioRepository _usuarioRepository;

        public UsuarioService(AuthUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> ValidarUsuario(string username, string password)
        {
            return await _usuarioRepository.ValidarCredencialesAsync(username, password);
        }
    }
}