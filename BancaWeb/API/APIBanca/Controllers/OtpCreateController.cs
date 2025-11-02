using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/cards/{card_id}/otp")]
public class OtpCreateController : ControllerBase
{
    private readonly OtpCreateService _service;
    private readonly EncryptionProtect _encryptionProtect;

    public OtpCreateController(OtpCreateService service, EncryptionProtect encryptionProtect)
    {
        _service = service;
        _encryptionProtect = encryptionProtect;
    }

    [HttpPost]
    public async Task<IActionResult> OtpCreate([FromRoute]string? card_id, [FromBody] OtpCreateM otp)
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
            var otpOriginal = otp.codigo_hash;
            var contrasenaEncrypt =  _encryptionProtect.Encrypt(otp.codigo_hash);
            Console.WriteLine($"Código original: {otpOriginal}");
            Console.WriteLine($"Código encriptado: {contrasenaEncrypt}");
            otp.codigo_hash = contrasenaEncrypt;
            otp.usuario_id = card_id;

            Console.WriteLine($"card ID RECIBIDO: {card_id}");
            var consulta = await _service.OtpCreate(otp);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok($"Otp ID: {consulta}, Este es su otp: {otpOriginal}");
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}