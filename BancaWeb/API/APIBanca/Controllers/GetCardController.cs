using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/cards")]
public class GetCardController : ControllerBase
{
    private readonly GetCardService _service;

    public GetCardController(GetCardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetCard>>> GetCard([FromQuery] string? userId, [FromQuery] string? cardId)
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
            Console.WriteLine($"CARD ID RECIBIDO: {cardId}");
            var tarjeta = await _service.GetCard(userId, cardId);

            if (tarjeta == null)
                return BadRequest("No se pudo obtener la tarjeta.");

            return Ok(tarjeta);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}