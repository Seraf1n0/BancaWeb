using Microsoft.AspNetCore.Mvc;
using APIBanca.Services;
using APIBanca.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]  
[ApiController]
[Route("api/v1/transfers")]
public class TransferController : ControllerBase
{
    private readonly InterbankTransferService _service;

    public TransferController(InterbankTransferService service)
    {
        _service = service;
    }

    [HttpPost("interbank")]
    public async Task<IActionResult> CreateInterbank([FromBody] InterbankTransferRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState });

        try
        {
            var result = await _service.TransferAsync(
                dto.from,
                dto.to,
                dto.amount,
                dto.currency,
                dto.description
            );

            return Ok(new
            {
                success = true,
                message = "Transferencia enviada al Banco Central",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Error procesando transferencia", error = ex.Message });
        }
    }
}