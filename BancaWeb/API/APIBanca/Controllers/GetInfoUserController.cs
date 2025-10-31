using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                  
using System;                           
using System.Threading.Tasks;          



[Authorize]
[ApiController]
[Route("api/v1/users/{identification}")]
public class GetInfoUserController : ControllerBase
{
    private readonly GetInfoUserService _service;
    private readonly UserRolIDService _userRolIDService;

    public GetInfoUserController(GetInfoUserService service, UserRolIDService userRolIDService)
    {
        _service = service;
        _userRolIDService = userRolIDService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerUsuarioID([FromRoute] string identification)
    {
        try
        {
            // Esto es para ver si mi jwt esta bie 
            var jwtUserId = User.FindFirst("userId")?.Value;

            if (jwtUserId == null) 
            {
                return Unauthorized();
            }



            var datosToken = await _userRolIDService.obtenerRoleID(Guid.Parse(jwtUserId));

            if (datosToken == null) 
            {
                return Unauthorized();
            }

            var tipoIdentificacion = datosToken.Value.tipoIdentificacion;
            var idToken = datosToken.Value.identificacion;


            if (tipoIdentificacion != identification && idToken != "Administrador")
            {
                return Forbid();
            }

            var usuario = await _service.ObtenerUsuarioID(identification);

            if (usuario == null)
                return NotFound(new { success = false, message = "Usuario no encontrado" });

            return Ok(new { success = true, usuario });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
