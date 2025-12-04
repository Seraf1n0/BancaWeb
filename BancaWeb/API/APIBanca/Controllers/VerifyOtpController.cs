using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


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
            
            if (string.IsNullOrEmpty(otp.user_id))
            {
                return BadRequest("El user_id es requerido.");
            }


            var otpOriginal = otp.codigo_hash;
            var contrasenaEncrypt =  _encryptionProtect.Encrypt(otp.codigo_hash);
            otp.codigo_hash = contrasenaEncrypt;
            Console.WriteLine($"User ID: {otp.user_id}");
            Console.WriteLine($"Código OTP original: {otpOriginal}");
            Console.WriteLine($"Código OTP encriptado: {contrasenaEncrypt}");
            var consulta = await _service.VerifyOtp(otp);

            if (consulta == null)
                return BadRequest("Código inválido o expirado.");

            return Ok(new { message = "Código verificado correctamente", success = true });
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}