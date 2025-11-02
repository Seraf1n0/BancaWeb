using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/accounts/status")]
public class UpdateAccountStateController : ControllerBase
{
    private readonly UpdateAccountStateService _service;

    public UpdateAccountStateController( UpdateAccountStateService service)
    {
        _service = service;
    }

    [HttpPut]
    public async Task<IActionResult> ActualizarEstadoCuenta([FromBody] UpdateEstadoCuenta model)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null)
            {
                return Unauthorized();
            }
            var jwtRol = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"ROL DEL JWT: {jwtRol}");
            if (jwtRol != "1")
            {
                return Forbid();
            }
            var resultado = await _service.ActualizarEstadoCuenta(model);
            if (!resultado)
            {
                return BadRequest();
            }
            return Ok();
        } catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return StatusCode(500, "Error interno del servidor.");
        }
    }
}