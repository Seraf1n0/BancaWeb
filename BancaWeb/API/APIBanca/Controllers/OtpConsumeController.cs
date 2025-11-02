using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/cards/{card_id}/view-details")]
public class OtpConsumeController : ControllerBase
{
    private readonly OtpConsumeService _service;

    public OtpConsumeController(OtpConsumeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> OtpConsume([FromRoute]string? card_id, [FromBody] OtpConsumeM otp)
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
            otp.user_id = card_id;

            Console.WriteLine($"card ID RECIBIDO: {card_id}");
            var consulta = await _service.OtpConsume(otp);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok(consulta);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}