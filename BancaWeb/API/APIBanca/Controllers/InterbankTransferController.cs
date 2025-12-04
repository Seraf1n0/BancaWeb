using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize]
[ApiController]
[Route("/api/v1/transfers/interbank")]
public class InterbankTransferController : ControllerBase
{
    private readonly InterbankTransferService _service;

    public InterbankTransferController(InterbankTransferService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] InterbankTransferRequest body)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue("userId");
        if (!Guid.TryParse(sub, out var userId)) return Unauthorized(new { message = "Token inválido" });

        try
        {
            var result = await _service.TransferInterbank(body, userId);
            return Ok(result);
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
