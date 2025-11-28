using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/bank/validate-account")]
public class BankValidateController : ControllerBase
{
    private readonly BankValidateService _service;

    public BankValidateController(BankValidateService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> VerificarCuenta([FromBody] IbanAccount ibanAccount)
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

            var consulta = await _service.VerificarCuenta(ibanAccount.iban);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            if (!consulta.exists)
                return NotFound("La cuenta bancaria no existe.");

            if (consulta.info == null)
                return NotFound("No se encontraron detalles del propietario de la cuenta.");
            
            if (string.IsNullOrEmpty(consulta.info.name) || string.IsNullOrEmpty(consulta.info.identification))
                return NotFound("Faltan detalles del propietario de la cuenta.");

            return Ok(consulta);
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}