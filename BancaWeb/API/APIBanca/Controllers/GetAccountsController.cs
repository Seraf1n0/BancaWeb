using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

    // GET /api/v1/accounts?userId={ownerId?}&accountId={accountId?}
    [HttpGet]
    public async Task<ActionResult<List<Cuenta>>> GetAccounts(
        [FromQuery(Name = "userId")] string? ownerId,
        [FromQuery] string? accountId)
    {
        try
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var isAdmin = string.Equals(role, "1", StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);

            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue("userId");

            if (!Guid.TryParse(sub, out var callerId))
                return Unauthorized(new { message = "Token inválido" });

            if (!isAdmin)
                ownerId = callerId.ToString();

            var cuentas = await _service.GetAccounts(ownerId, accountId);

            if (!isAdmin && !string.IsNullOrWhiteSpace(accountId))
            {
                if (cuentas.Count == 0 || cuentas.Any(c => c.usuario_id != callerId))
                    return StatusCode(403, new { message = "Forbidden" });
            }

            return Ok(cuentas);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { error = $"Error Supabase: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}