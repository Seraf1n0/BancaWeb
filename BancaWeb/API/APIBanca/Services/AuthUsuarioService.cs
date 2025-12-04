using APIBanca.Models;

namespace APIBanca.Services
{
    public class UsuarioService
    {
        private readonly AuthUsuarioRepository _usuarioRepository;
        private readonly EncryptionProtect _encryptionProtect;

        public UsuarioService(AuthUsuarioRepository usuarioRepository, EncryptionProtect encryptionProtect)
        {
            _usuarioRepository = usuarioRepository;
            _encryptionProtect = encryptionProtect;
        }

        public async Task<UserAuthResponse?> ValidarUsuario(string usernameOrEmail, string password)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorUsernameOEmailAsync(usernameOrEmail);
            
            if (usuario == null)
            {
                return null; 
            }

            
            var passwordEncriptada = _encryptionProtect.Encrypt(password);
            Console.WriteLine($"Password ingresada: {password}");
            Console.WriteLine($"Password encriptada: {passwordEncriptada}");
            Console.WriteLine($"Password en BD: {usuario.ContrasenaHash}");

            bool passwordValida = passwordEncriptada == usuario.ContrasenaHash;

            if (!passwordValida)
            {
                return null; 
            }

            return usuario;
        }
    }
}