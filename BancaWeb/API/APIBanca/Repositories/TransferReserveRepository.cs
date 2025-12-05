using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace APIBanca.Repositories
{
    public class TransferReserveRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly ILogger<TransferReserveRepository> _logger;

        public TransferReserveRepository(HttpClient httpClient, IConfiguration configuration, ILogger<TransferReserveRepository> logger)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"]!;
            _supabaseKey = configuration["Supabase:ServiceRoleKey"]!;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        public async Task<(bool ok, string? reason)> ReserveAsync(string accountIban, decimal amount, string transferId)
        {
            try
            {
                var payload = new
                {
                    p_account_iban = accountIban,
                    p_amount = amount,
                    p_transfer_id = transferId
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var rpcUrl = $"{_supabaseUrl}/rest/v1/rpc/sp_transfer_reserve";
                var httpResponse = await _httpClient.PostAsync(rpcUrl, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"Error HTTP {httpResponse.StatusCode}: {errorContent}");
                    return (false, "HTTP_ERROR");
                }

                var response = await httpResponse.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(response) || response == "[]")
                {
                    _logger.LogError("Respuesta vacía del stored procedure");
                    return (false, "DB_ERROR");
                }

                var json = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, System.Text.Json.JsonElement>>>(response);
                
                if (json == null || json.Count == 0)
                {
                    return (false, "INVALID_RESPONSE");
                }

                var firstRow = json[0];
                bool ok = firstRow["ok"].GetBoolean();
                string? reason = firstRow.ContainsKey("reason") && firstRow["reason"].ValueKind != System.Text.Json.JsonValueKind.Null
                    ? firstRow["reason"].GetString()
                    : null;

                return (ok, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReserveAsync");
                return (false, "EXCEPTION");
            }
        }
    }
}
