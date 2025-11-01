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
[Route("api/v1/cards")]
public class CreatedCardController : ControllerBase
{
    private readonly CreateCardService _service;


    public CreatedCardController(CreateCardService service)
    {
        _service = service;
    }


    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCard card)
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
            if (jwtRol != "1")
            {
                return Forbid();
            }

            var cardId = await _service.CrearTarjetaAsync(card);

            return Ok(new { CardId = cardId });

        }   
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"ERROR SUPABASE: {ex.Message}");
        return StatusCode(502, new { error = ex.Message });
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"ERROR JSON: {ex.Message}");
        return StatusCode(500, new { error = "Error procesando respuesta de Supabase" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
        return StatusCode(500, "Error interno del servidor.");
    }
    }
}