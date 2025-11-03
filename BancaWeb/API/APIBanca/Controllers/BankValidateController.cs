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

            return Ok(new { mensaje = $"La cuenta existe: {consulta.existeCuenta}, Propietario: {consulta.propietarioCuenta}, ID Propietario: {consulta.idPropietario}" });
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}