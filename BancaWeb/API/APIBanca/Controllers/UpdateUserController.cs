using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/users/{idUsuario}")]
public class UpdateUserController : ControllerBase
{
    private readonly UpdateUserService _service;

    public UpdateUserController( UpdateUserService service)
    {
        _service = service;
    }

    [HttpPut]
    public async Task<IActionResult> ActualizarUsuario([FromRoute] string idUsuario, [FromBody] UpdateUser model)
    {
        try {
            var jwtUserId = User.FindFirst("userId")?.Value;
            Console.WriteLine($"ID DEL JWT: {jwtUserId}");
            if (jwtUserId == null) 
            {
                return Unauthorized();
            }
            var jwtRol = User.FindFirst(ClaimTypes.Role)?.Value;
;
            Console.WriteLine($"ROL DEL JWT: {jwtRol}");
            if (jwtRol != "1")
            {
                return Forbid();
            }
            var resultado = await _service.ActualizarUsuario(idUsuario, model.Nombre, model.Apellido, model.Correo,model.Usuario,
                model.Rol);

            if (!resultado)
                return BadRequest("No se pudo actualizar el usuario.");

            return Ok("Usuario actualizado correctamente.");



        } catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return StatusCode(500, "Error interno del servidor.");
        }




    }
}