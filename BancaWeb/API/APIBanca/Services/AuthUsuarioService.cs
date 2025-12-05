using APIBanca.Models;
using APIBanca.Repositories;
using System.Threading.Tasks;
using BCrypt.Net;


namespace APIBanca.Services
{
    public class UsuarioService
    {
        private readonly AuthUsuarioRepository _usuarioRepository;

        public UsuarioService(AuthUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UserAuthResponse?> ValidarUsuario(string usernameOrEmail, string passwordPlano)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorUsernameOEmailAsync(usernameOrEmail);

            if (usuario == null)
                return null;

            string hashGuardado = usuario.ContrasenaHash;

            bool valido = BCrypt.Net.BCrypt.Verify(passwordPlano, hashGuardado);

            if (!valido)
                return null;

            return usuario;
        }
    }
}
