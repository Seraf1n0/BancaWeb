using System.Text.Json;
using System.Text;
using APIBanca.Models;
using System.Net.Http.Headers;

namespace APIBanca.Repositories 
{
    public class AccountMovementRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public AccountMovementRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"]!;
            _supabaseKey = configuration["Supabase:ServiceRoleKey"]!;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        public async Task<TransferInternalResponse> TransferInternalAsync(
            Guid fromAccountId, Guid toAccountId, decimal amount, Guid currency, string? descripcion, Guid userId)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_transfer_create_internal";
            var body = new {
                p_from_account_id = fromAccountId,
                p_to_account_id   = toAccountId,
                p_amount          = amount,
                p_currency        = currency,
                p_descripcion     = descripcion,
                p_user_id         = userId
            };

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Prefer", "return=representation");

            var resp = await _httpClient.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase ({resp.StatusCode}): {json}");

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            JsonElement obj = root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0 ? root[0] :
                              root.ValueKind == JsonValueKind.Object ? root :
                              throw new InvalidOperationException("Respuesta inesperada del SP");

            return new TransferInternalResponse
            {
                transfer_id    = obj.GetProperty("transfer_id").GetGuid(),
                receipt_number = obj.TryGetProperty("receipt_number", out var rn) ? rn.GetString() : null,
                status         = obj.TryGetProperty("status", out var st) ? st.GetString() : null
            };
        }
    }
}