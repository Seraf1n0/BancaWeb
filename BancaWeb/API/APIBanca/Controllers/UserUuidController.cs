using Microsoft.AspNetCore.Mvc;
using APIBanca.Repositories;

[ApiController]
[Route("api/v1/users")]
public class UserUuidController : ControllerBase
{
    private readonly UserUuidRepository _repository;

    public UserUuidController(UserUuidRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("uuid")]
    public async Task<IActionResult> GetUserIdByUsername([FromQuery] string username)
    {
        var userId = await _repository.GetUserIdByUsernameAsync(username);
        if (userId == null)
            return NotFound("Usuario no encontrado");
        return Ok(new { userId });
    }
}