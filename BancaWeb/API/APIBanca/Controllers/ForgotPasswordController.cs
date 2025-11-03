using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


[Authorize]
[ApiController]
[Route("/api/v1/auth/forgot-password")]
public class ForgotPasswordController : ControllerBase
{
    private readonly ForgotPasswordService _service;
    private readonly EncryptionProtect _encryptionProtect;

    public ForgotPasswordController(ForgotPasswordService service, EncryptionProtect encryptionProtect)
    {
        _service = service;
        _encryptionProtect = encryptionProtect;
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordM forgotPassword) 
    {
        try {
            var otpOriginal = forgotPassword.codigo_hash;
            var contrasenaEncrypt =  _encryptionProtect.Encrypt(forgotPassword.codigo_hash);
            forgotPassword.codigo_hash = contrasenaEncrypt;
            Console.WriteLine($"Código OTP original: {otpOriginal}");
            Console.WriteLine($"Código OTP encriptado: {contrasenaEncrypt}");
            var consulta = await _service.ForgotPassword(forgotPassword);

            if (consulta == null)
                return BadRequest("No se pudo obtener la consulta.");

            return Ok($"Esto es tu id del otp creado: {consulta}, este es tu otp creado: {otpOriginal}");
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}