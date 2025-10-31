using Microsoft.AspNetCore.Mvc;
using APIBanca.Models;
using APIBanca.Services;


namespace APIBanca.Controllers
{

    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly CreateUserService _service;
        private readonly ApiKeyRepository _apiKeyRepository;
        private readonly ApiKeyGeneratorService _apiKeyGenerator;
        // inyectamos los servicios 
        public UserController(CreateUserService service, ApiKeyRepository apiKeyRepository, ApiKeyGeneratorService apiKeyGenerator)
        {
            _service = service;
            _apiKeyRepository = apiKeyRepository;
            _apiKeyGenerator = apiKeyGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] User usuario)
        {
            try
            {        
                usuario.p_contrasena_hash = BCrypt.Net.BCrypt.HashPassword(usuario.p_contrasena_hash);

                var userId = await _service.CrearUsuarioAsync(usuario);
                Console.WriteLine($"[DEBUG] User ID: {userId}");
                //Aqui ya guardo el api después de crear el usuario
                string apiKey = _apiKeyGenerator.GenerarApiKey();
                await _apiKeyRepository.CrearApiKeyAsync(userId, apiKey);



                
                return Ok(new { success = true, userId,apiKey});
            }
            catch(Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}