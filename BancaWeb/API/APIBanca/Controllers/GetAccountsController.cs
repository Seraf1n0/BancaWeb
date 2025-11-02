using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/accounts")]
public class GetAccountsController : ControllerBase
{
    private readonly GetAccountService _service;

    public GetAccountsController(GetAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Cuenta>>> GetAccounts([FromQuery] string? userId, [FromQuery] string? accountId)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null || jwtUserId != userId) 
            {
                return Unauthorized();
            }
            var jwtRol = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"ROL DEL JWT: {jwtRol}");
            if (jwtRol != "1")
            {
                return Forbid();
            }

            Console.WriteLine($"USER ID RECIBIDO: {userId}");
            Console.WriteLine($"ACCOUNT ID RECIBIDO: {accountId}");
            var cuentas = await _service.GetAccounts(userId, accountId);

            if (cuentas == null)
                return BadRequest("No se pudieron obtener las cuentas.");

            return Ok(cuentas);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}