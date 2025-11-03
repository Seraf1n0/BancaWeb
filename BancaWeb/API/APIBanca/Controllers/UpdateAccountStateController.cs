using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/accounts")]
public class UpdateAccountStateController : ControllerBase
{
    private readonly UpdateAccountStateService _service;

    public UpdateAccountStateController(UpdateAccountStateService service)
    {
        _service = service;
    }

    [HttpPost("{accountId:guid}/status")]
    public async Task<IActionResult> SetStatus([FromRoute] Guid accountId, [FromBody] UpdateAccountStatusRequest body)
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
        if (role != "1") return Forbid();

        if (body == null || string.IsNullOrWhiteSpace(body.nuevo_estado))
            return BadRequest(new { message = "nuevo_estado requerido" });

        var ok = await _service.SetStatus(accountId, body.nuevo_estado);
        return Ok(new { updated = ok });
    }
}