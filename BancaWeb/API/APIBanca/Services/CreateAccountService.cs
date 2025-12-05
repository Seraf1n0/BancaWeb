using APIBanca.Models;
using APIBanca.Repositories;

namespace APIBanca.Services
{
    public class CreateAccountService
    {
        private readonly CreateAccountRepository _repository;
        public CreateAccountService(CreateAccountRepository repository)
        {
            _repository = repository;
        }

        public bool ValidarIban(string iban)
        {
            if (string.IsNullOrEmpty(iban))
                return false;

            iban = iban.Trim().ToUpperInvariant();

            // Letras mayusculas y dígitos
            if (!System.Text.RegularExpressions.Regex.IsMatch(iban, @"^[A-Z0-9]+$"))
                return false;
            // Patron y longitud: CR + 2 numeros, B01(banco prometedores) + 12 digitos
            if (!System.Text.RegularExpressions.Regex.IsMatch(iban, @"^CR\d{2}B01\d{12}$"))
                return false;

            return true;
        }

        public async Task<Guid> CrearCuentaAsync(CreateCuenta cuenta)
        {
            try {
                if (!ValidarIban(cuenta.iban))
                    throw new ArgumentException("El IBAN proporcionado no es válido.");
                return await _repository.CreateAccountAsync(cuenta);
            } catch (Exception ex) {
                throw new Exception("Error al crear la cuenta: " + ex.Message, ex);
            }
        }
    }
}