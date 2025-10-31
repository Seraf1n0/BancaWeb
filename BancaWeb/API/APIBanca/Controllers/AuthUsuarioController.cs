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
        private readonly JwtService _jwtService;

        public AuthUsuarioController(UsuarioService usuarioService, JwtService jwtService)
        {
            _usuarioService = usuarioService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostAuthUser([FromBody] AuthUser authUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Datos inválidos" });
            }

            try
            {
                var userAuth = await _usuarioService.ValidarUsuario(authUser.p_userName, authUser.p_password);

                if (userAuth == null){return Unauthorized(new { success = false, message = "Credenciales incorrectas" });}

                var token = _jwtService.GenerateToken(userAuth.UserId.ToString(), userAuth.Rol);
                

                return Ok(new { success = true,  message = "Autenticado correctamente", token = token, userId = userAuth.UserId,
                    rol = userAuth.Rol});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}