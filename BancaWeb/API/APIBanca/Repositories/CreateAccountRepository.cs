using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using APIBanca.Models;

namespace APIBanca.Repositories
{
    public class CreateAccountRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public CreateAccountRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"]
                ?? throw new InvalidOperationException("Falta Supabase:Url en configuración.");
            _supabaseKey = configuration["Supabase:ServiceRoleKey"]
                ?? throw new InvalidOperationException("Falta Supabase:ServiceRoleKey en configuración.");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        // Método de creación de cuenta
        public async Task<Guid> CreateAccountAsync(CreateCuenta newAccount)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_accounts_create";

            var body = new
            {
                p_usuario_id = newAccount.usuario_id,
                p_iban = newAccount.iban,
                p_alias = newAccount.alias,
                p_tipo = newAccount.tipo,
                p_moneda = newAccount.moneda,
                p_saldo_inicial = newAccount.saldo_inicial,
                p_estado = newAccount.estado
            };

            var jsonBody = JsonSerializer.Serialize(body);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase ({response.StatusCode}): {responseContent}");

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.String && Guid.TryParse(root.GetString(), out var g1)) return g1;
            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("account_id", out var a) && a.ValueKind == JsonValueKind.String && Guid.TryParse(a.GetString(), out var g2)) return g2;
                if (root.TryGetProperty("id", out var b) && b.ValueKind == JsonValueKind.String && Guid.TryParse(b.GetString(), out var g3)) return g3;
            }
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var el = root[0];
                if (el.TryGetProperty("account_id", out var c) && c.ValueKind == JsonValueKind.String && Guid.TryParse(c.GetString(), out var g4)) return g4;
                if (el.TryGetProperty("id", out var d) && d.ValueKind == JsonValueKind.String && Guid.TryParse(d.GetString(), out var g5)) return g5;
            }
            throw new Exception("No se pudo obtener el ID de la cuenta creada.");
        }
    }
}