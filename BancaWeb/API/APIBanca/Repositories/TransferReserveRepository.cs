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

        
        public async Task<(string fromIban, decimal amount)> GetLastMovementAsync()
        {
            try
            {
                var rpcUrl = $"{_supabaseUrl}/rest/v1/rpc/sp_ultimo_mov";

                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync(rpcUrl, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error consultando último movimiento: {httpResponse.StatusCode}");
                    return ("", 0);
                }

                var response = await httpResponse.Content.ReadAsStringAsync();

                var json = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response);

                if (json == null || json.Count == 0)
                    return ("", 0);

                string from = json[0]["from_iban"].GetString()!;
                decimal amount = json[0]["amount"].GetDecimal();

                return (from, amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo último movimiento");
                return ("", 0);
            }
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

                var json = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response);

                if (json == null || json.Count == 0)
                    return (false, "INVALID_RESPONSE");

                bool ok = json[0]["ok"].GetBoolean();
                string? reason =
                    json[0].ContainsKey("reason") &&
                    json[0]["reason"].ValueKind != JsonValueKind.Null
                        ? json[0]["reason"].GetString()
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
