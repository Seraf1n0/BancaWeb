using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;


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
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordM resetPasswordM)
    {
        try {
            
            if (string.IsNullOrEmpty(resetPasswordM.user_id))
            {
                return BadRequest("El user_id es requerido.");
            }

            if (string.IsNullOrEmpty(resetPasswordM.nueva_contrasena))
            {
                return BadRequest("La nueva contraseña es requerida.");
            }

            
            var otpOriginal = resetPasswordM.codigo_hash;
            var otpEncrypt = _encryptionProtect.Encrypt(resetPasswordM.codigo_hash);
            resetPasswordM.codigo_hash = otpEncrypt;
            
            
            var passwordOriginal = resetPasswordM.nueva_contrasena;
            var passwordEncrypt = _encryptionProtect.Encrypt(resetPasswordM.nueva_contrasena);
            resetPasswordM.nueva_contrasena = passwordEncrypt;

            Console.WriteLine($"User ID: {resetPasswordM.user_id}");
            Console.WriteLine($"Código OTP original: {otpOriginal}");
            Console.WriteLine($"Código OTP encriptado: {otpEncrypt}");
            Console.WriteLine($"Nueva contraseña original: {passwordOriginal}");
            Console.WriteLine($"Nueva contraseña encriptada: {passwordEncrypt}");

            var resultado = await _service.ResetPassword(resetPasswordM);

            if (!resultado)
                return BadRequest("No se pudo restablecer la contraseña. Código inválido o expirado.");

            return Ok(new { message = "Contraseña restablecida correctamente", success = true });
        } catch(Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}