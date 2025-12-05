using APIBanca.Repositories;
using APIBanca.Models;
using APIBanca.Handlers;
using System.Text.RegularExpressions;

namespace APIBanca.Services
{
    public class InterbankTransferService
    {
        private readonly InterbankTransferRepository _repo;
        private readonly BankSocketHandler _socket;
        private readonly ILogger<InterbankTransferService> _logger;

        public InterbankTransferService(
            InterbankTransferRepository repo,
            BankSocketHandler socket,
            ILogger<InterbankTransferService> logger)
        {
            _repo = repo;
            _socket = socket;
            _logger = logger;
        }
    

        private static bool validarIban(string iban)
        {
            if (string.IsNullOrEmpty(iban))
                return false;

            iban = iban.Trim().ToUpperInvariant();

            // Letras mayúsculas y dígitos
            if (!Regex.IsMatch(iban, @"^[A-Z0-9]+$"))
                return false;

            // Patrón: CR + 2 dígitos + código banco (B00-B08) + 20 dígitos
            if (!Regex.IsMatch(iban, @"^CR\d{2}B0[0-8]\d{20}$"))
                return false;
            
            return true;
        }

        public async Task<InterbankTransferResponse> TransferAsync(
            string fromIban,
            string toIban,
            decimal amount,
            string currency,
            string? descripcion)
        {
            if (fromIban == toIban)
                throw new InvalidOperationException("No puedes transferir a la misma cuenta");

            if (currency != "CRC" && currency != "USD")
                throw new InvalidOperationException("Moneda debe ser CRC o USD");

            var transferId = $"TX{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";

            _logger.LogInformation($"Transferencia {transferId}");
            _logger.LogInformation($"   {fromIban} → {toIban}");
            _logger.LogInformation($"   {amount} {currency}");

            // esto es para supabase
            var dbResult = await _repo.TransferInterbankAsync(
                fromIban,
                toIban,
                amount,
                currency,
                descripcion
            );

            // Aqui donde lo envio
            await _socket.SendInterbankTransfer(new 
            {
                type = "transfer.intent",
                data = new
                {
                    id = transferId,
                    from = fromIban,
                    to = toIban,
                    amount = amount,
                    currency = currency
                }
            });


            _logger.LogInformation("Intent enviado al Banco Central");

            return new InterbankTransferResponse
            {
                id = transferId,
                status = "PENDING",
                receipt_number = dbResult.receipt_number
            };
        }
    }
}
