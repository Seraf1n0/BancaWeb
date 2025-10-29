using Microsoft.AspNetCore.Mvc;
using APIBanca.Services;
using APIBanca.Models;

namespace APIBanca.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthUsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public AuthUsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostAuthUser([FromBody] AuthUser authUser)
        {
            try
            {
                if (string.IsNullOrEmpty(authUser.p_userName) || string.IsNullOrEmpty(authUser.p_password))
                {
                    return BadRequest(new { success = false, message = "Username y password requeridos" });
                }

                var isValid = await _usuarioService.ValidarUsuario(authUser.p_userName, authUser.p_password);

                if (isValid)
                {
                    return Ok(new { success = true, message = "Autenticado correctamente" });
                }
                else
                {
                    return Unauthorized(new { success = false, message = "Credenciales incorrectas" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

}