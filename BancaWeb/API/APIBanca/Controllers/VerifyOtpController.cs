using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("api/v1/auth/verify-otp")]
public class VerifyOtpController : ControllerBase
{
    private readonly VerifyOtpService _service;
    private readonly EncryptionProtect _encryptionProtect;

    public VerifyOtpController(VerifyOtpService service, EncryptionProtect encryptionProtect)
    {
        _service = service;
        _encryptionProtect = encryptionProtect;
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpConsumeM otp)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null) 
            {
                return Unauthorized();
            }


            var otpOriginal = otp.codigo_hash;
            var contrasenaEncrypt =  _encryptionProtect.Encrypt(otp.codigo_hash);
            otp.codigo_hash = contrasenaEncrypt;
            Console.WriteLine($"Código OTP original: {otpOriginal}");
            Console.WriteLine($"Código OTP encriptado: {contrasenaEncrypt}");
            var consulta = await _service.VerifyOtp(otp);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok($"El otp fue consumido: {consulta}");
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}