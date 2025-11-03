using APIBanca.Models;

namespace APIBanca.Services
{
    public class UsuarioService
    {
        private readonly AuthUsuarioRepository _usuarioRepository;

        public UsuarioService(AuthUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UserAuthResponse?> ValidarUsuario(string usernameOrEmail, string password)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorUsernameOEmailAsync(usernameOrEmail);
            
            if (usuario == null)
            {
                return null; 
            }


            bool passwordValida = password == usuario.ContrasenaHash;

            if (!passwordValida)
            {
                return null; 
            }

            return usuario;
        }
    }
}