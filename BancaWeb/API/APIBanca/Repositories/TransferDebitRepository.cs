using System.Text;
using System.Text.Json;

namespace APIBanca.Repositories
{
    public class TransferDebitRepository
    {
        private readonly HttpClient _http;
        private readonly ILogger<TransferDebitRepository> _logger;

        public TransferDebitRepository(HttpClient http, ILogger<TransferDebitRepository> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<(bool ok, string? reason)> DebitAsync(string iban)
        {
            var payload = new
            {
                p_account_iban = iban
            };

            var json = JsonSerializer.Serialize(payload);
            _logger.LogInformation("➡ Enviando a sp_transfer_debit: " + json);

            var response = await _http.PostAsync(
                "rpc/sp_transfer_debit",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error HTTP Debit: " + body);
                return (false, "HTTP_ERROR");
            }

            var result = JsonSerializer.Deserialize<List<DebitResult>>(body);

            var row = result?.FirstOrDefault();

            return (row?.ok ?? false, row?.reason);
        }

        private class DebitResult
        {
            public bool ok { get; set; }
            public string? reason { get; set; }
        }
    }
}
