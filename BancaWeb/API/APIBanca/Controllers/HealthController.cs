using Microsoft.AspNetCore.Mvc;

namespace APIBanca.Controllers
{
    // Controller para control de salud de la API
    [ApiController]
    [Route("api/v1/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "ok" });

        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { pong = true, serverTime = DateTime.UtcNow });
    }
}