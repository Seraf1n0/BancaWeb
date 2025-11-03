using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("/api/v1/auth/reset-password")]
public class ResetPasswordController : ControllerBase
{
    private readonly ResetPasswordService _service;
    private readonly EncryptionProtect _encryptionProtect;

    public ResetPasswordController(ResetPasswordService service, EncryptionProtect encryptionProtect)
    {
        _service = service;
        _encryptionProtect = encryptionProtect;
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] OtpConsumeM otpConsumeM)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null) 
            {
                return Unauthorized();
            }


            var otpOriginal = otpConsumeM.codigo_hash;
            var contrasenaEncrypt =  _encryptionProtect.Encrypt(otpConsumeM.codigo_hash);
            otpConsumeM.codigo_hash = contrasenaEncrypt;
            Console.WriteLine($"Código OTP original: {otpOriginal}");
            Console.WriteLine($"Código OTP encriptado: {contrasenaEncrypt}");
            var consulta = await _service.ResetPassword(otpConsumeM);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok($"El otp fue consumido: {consulta}");
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}