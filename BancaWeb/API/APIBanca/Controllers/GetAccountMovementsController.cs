using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization;
using APIBanca.Models;          
using APIBanca.Services;        
using System;                           
using System.Threading.Tasks;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/accounts/{account_id}/movements")]
public class GetAccountMovementsController : ControllerBase
{
    private readonly GetAccountMovementsService _service;

    public GetAccountMovementsController(GetAccountMovementsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> AccountMovements([FromRoute] string? account_id, [FromQuery] DateTime? start_date, [FromQuery] DateTime? to_date, [FromQuery] string type,
    [FromQuery] string? q, [FromQuery] int page, [FromQuery] int pageSize)
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

            Console.WriteLine($"account ID RECIBIDO: {account_id}");
            var consulta = await _service.AccountMovements(account_id, start_date, to_date, type, q, page, pageSize);
            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok(consulta);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}