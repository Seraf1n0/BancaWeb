using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]  
[Route("api/v1/cards/{cardId}/movements")]
public class CardMovementController : ControllerBase
{
    private readonly CardMovementService _service;

    public CardMovementController(CardMovementService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CardMovement([FromRoute] string cardId, [FromBody] CardMovement card)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null)
            {
                Unauthorized();
            }
            var jwtRol = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"ROL DEL JWT: {jwtRol}");


            Console.WriteLine($"CARD ID RECIBIDO: {cardId}");
            card.CardID = Guid.Parse(cardId);
            card.FechaMovimiento = DateTime.UtcNow;

            var resultado = await _service.CardMovement(card);
            //Estos los devuevlo así por ser tuplas
            return Ok(new {movement_id = resultado.Item1, nuevo_saldo_tarjeta = resultado.Item2});
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
 
}