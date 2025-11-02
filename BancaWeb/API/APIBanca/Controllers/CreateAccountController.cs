using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIBanca.Services;
using APIBanca.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text.Json;

[Authorize]
[ApiController]
[Route("api/v1/accounts")]
public class CreateAccountController : ControllerBase
{
    private readonly CreateAccountService _service;

    public CreateAccountController(CreateAccountService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateCuenta cuenta)
    {
        try
        {
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

            var accountId = await _service.CrearCuentaAsync(cuenta);

            return Ok(new { AccountId = accountId });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"ERROR SUPABASE: {ex.Message}");
            return StatusCode(502, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR INTERNO: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor." });
        }
    }
}