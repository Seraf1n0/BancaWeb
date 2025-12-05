using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace APIBanca.Repositories
{
    public class TransferCreditRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly ILogger<TransferCreditRepository> _logger;

        public TransferCreditRepository(HttpClient httpClient, IConfiguration configuration, ILogger<TransferCreditRepository> logger)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"]!;
            _supabaseKey = configuration["Supabase:ServiceRoleKey"]!;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        public async Task<(bool ok, string? reason)> CreditAsync(
            Guid transferId,
            string toIban,
            string fromIban,
            decimal amount,
            string currency,
            string? descripcion)
        {
            try
            {
                var payload = new
                {
                    p_transfer_id = transferId,
                    p_to_iban = toIban,
                    p_from_iban = fromIban,
                    p_amount = amount,
                    p_currency = currency,
                    p_descripcion = descripcion
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var rpcUrl = $"{_supabaseUrl}/rest/v1/rpc/sp_transfer_credit";
                var httpResponse = await _httpClient.PostAsync(rpcUrl, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError($"Error HTTP {httpResponse.StatusCode}: {errorContent}");
                    return (false, "HTTP_ERROR");
                }

                var response = await httpResponse.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(response);

                if (results == null || !results.Any())
                    return (false, "NO_RESPONSE");

                var result = results.First();
                var ok = result["ok"].GetBoolean();
                var reason = result.ContainsKey("reason") && result["reason"].ValueKind != JsonValueKind.Null
                    ? result["reason"].GetString()
                    : null;

                return (ok, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreditAsync");
                return (false, ex.Message);
            }
        }
    }
}
