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
public class DeleteUserController : ControllerBase
{
    private readonly DeleteUserService _service;

    public DeleteUserController(DeleteUserService service)
    {
        _service = service;
    }

    [HttpDelete]
    public async Task<IActionResult> EliminarUsuario([FromRoute] string idUsuario)
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
            if (jwtRol != "1")
            {
                return Forbid();
            }
            var resultado = await _service.EliminarUsuario(idUsuario);

            Console.WriteLine($"Se elimino o no: {resultado}");

            if (!resultado)
                return BadRequest("No se pudo eliminar el usuario.");

            return Ok("Usuario eliminado correctamente.");



        } catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return StatusCode(500, "Error interno del servidor.");
        }




    }
}