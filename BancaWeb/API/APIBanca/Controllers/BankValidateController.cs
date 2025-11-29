using Microsoft.AspNetCore.Mvc;        
using Microsoft.AspNetCore.Authorization; 
using APIBanca.Services;                
using APIBanca.Models;                           
using System.Security.Claims;
using System.Text.RegularExpressions;


[ApiController]
[Route("api/v1/bank/validate-account")]
public class BankValidateController : ControllerBase
{
    private readonly BankValidateService _service;
    private readonly string _bankCentralApiToken;

    public BankValidateController(BankValidateService service, IConfiguration configuration)
    {
        _service = service;
        _bankCentralApiToken = configuration["Security:ApiTokenBancoCentral"];
    }

    private static bool validarIban (string iban)
    {
        if (string.IsNullOrEmpty(iban))
            return false;

        iban = iban.Trim().ToUpperInvariant();

        // Letras mayusculas y dígitos
        if (!Regex.IsMatch(iban, @"^[A-Z0-9]+$"))
            return false;
        // Patron y longitud: CR + 2 numeros, B01(banco prometedores) + 23 digitos
        if (!Regex.IsMatch(iban, @"^CR\d{2}B01\d{20}$"))
            return false;
        
        return true;
    }

    [HttpPost]
    public async Task<IActionResult> VerificarCuenta([FromBody] IbanAccount ibanAccount)
    {
        // Validamos el api token del Banco Central
        if (!Request.Headers.TryGetValue("X-API-TOKEN", out var apiTokenExtraido))
        {
            return new UnauthorizedObjectResult(
                new BankValidateUnauthorized
                {
                    error = "MISSING_API_TOKEN",
                    message = "Falta el token de API para el Banco Central."
                }
            );
        }
        if (apiTokenExtraido != _bankCentralApiToken)
        {
            return new UnauthorizedObjectResult(
                new BankValidateUnauthorized
                {
                    error = "INVALID_API_TOKEN",
                    message = "El token de API para el Banco Central es inválido."
                }
            );
        }
        // Bad request si no encontramos el body o el iban
        if (ibanAccount == null || string.IsNullOrEmpty(ibanAccount.iban))
        {
            return new BadRequestObjectResult(
                new BankValidateBadRequest
                {
                    error = "MISSING_ACCOUNT_IBAN",
                    message = "Falta el IBAN de la cuenta bancaria a validar."
                }
            );
        }

        // Validamos formato de iban sino 401
        if (!validarIban(ibanAccount.iban))
        {
            return new UnauthorizedObjectResult(
                new BankValidateUnauthorized
                {
                    error = "INVALID_ACCOUNT_FORMAT",
                    message = "El formato de la cuenta bancaria (IBAN) es inválido."
                }
            );
        }

        try {
            var consulta = await _service.VerificarCuenta(ibanAccount.iban);

            if (consulta == null || !consulta.exists)
                return new NotFoundObjectResult(
                    new BankValidateNotFound
                    {
                        error = "ACCOUNT_NOT_FOUND",
                        message = "No se encontró la cuenta bancaria solicitada."
                    }
                );

            if (consulta.info == null)
                return new NotFoundObjectResult(
                    new BankValidateNotFound
                    {
                        error = "OWNER_DETAILS_NOT_FOUND",
                        message = "No se encontraron detalles del propietario de la cuenta."
                    }
                );
            
            if (string.IsNullOrEmpty(consulta.info.name) || string.IsNullOrEmpty(consulta.info.identification))
                return new NotFoundObjectResult(
                    new BankValidateNotFound
                    {
                        error = "OWNER_DETAILS_INCOMPLETE",
                        message = "Faltan detalles del propietario de la cuenta."
                    }
                );

            return Ok(consulta);
        } catch(Exception ex) {
            return new ObjectResult(
                new BankValidateServerError
                {
                    error = "SERVER_ERROR",
                    message = "Ocurrió un error interno al procesar la solicitud: " + ex.Message
                }
            ) { StatusCode = 500 };
        } 
    }
}