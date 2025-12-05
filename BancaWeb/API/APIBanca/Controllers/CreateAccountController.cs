using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIBanca.Services;
using APIBanca.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


[ApiController]
[Route("api/v1/accounts")]
public class CreateAccountController : ControllerBase
{
    private readonly CreateAccountService _service;
    public CreateAccountController(CreateAccountService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCuenta cuenta)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var rol = User.FindFirstValue(ClaimTypes.Role) ?? "";
        var isAdmin = string.Equals(rol, "1", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(rol, "admin", StringComparison.OrdinalIgnoreCase);

        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue("userId");

        if (!Guid.TryParse(sub, out var callerId))
            return Unauthorized(new { message = "Token inválido" });

        if (!isAdmin)
            cuenta.usuario_id = callerId;

        var id = await _service.CrearCuentaAsync(cuenta);
        return Created($"/api/v1/accounts/{id}", new { account_id = id });
    }
}