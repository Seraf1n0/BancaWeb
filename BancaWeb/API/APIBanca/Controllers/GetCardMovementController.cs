using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/cards/{card_id}/movements")]
public class GetCardMovementController : ControllerBase
{
    private readonly GetCardMovementService _service;

    public GetCardMovementController(GetCardMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> CardMovement([FromRoute]string? card_id, [FromQuery] DateTime? start_date, [FromQuery] DateTime? to_date, [FromQuery] string? type,
    [FromQuery] string? q, [FromQuery] int page, [FromQuery] int pageSize)
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


            Console.WriteLine($"card ID RECIBIDO: {card_id}");
            var consulta = await _service.CardMovement(card_id, start_date, to_date, type, q, page, pageSize);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok(consulta);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}