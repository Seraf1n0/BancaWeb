using System.Text.Json;
using System.Text;
using APIBanca.Models;
using System.Net.Http.Headers;

namespace APIBanca.Repositories
{
    public class GetAccountRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public GetAccountRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        // Método para obtener el listado de todas las cuentas
        public async Task<List<Cuenta>> GetAccountsAsync(string? userId, string? accountId)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_accounts_get";
            var body = new {
                p_owner_id  = userId,
                p_account_id   = accountId,
            };

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            { Content = content };

            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase ({response.StatusCode}): {responseContent}");

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            var listaCuentas = new List<Cuenta>();
            if (root.ValueKind != JsonValueKind.Array) return listaCuentas;

            foreach (var c in root.EnumerateArray())
            {
                Guid tipoGuid =
                    c.TryGetProperty("tipoCuenta", out var t1) && t1.ValueKind == JsonValueKind.String && Guid.TryParse(t1.GetString(), out var g1) ? g1 :
                    c.TryGetProperty("tipo", out var t2) && t2.ValueKind == JsonValueKind.String && Guid.TryParse(t2.GetString(), out var g2) ? g2 :
                    Guid.Empty;

                listaCuentas.Add(new Cuenta
                {
                    id = c.GetProperty("id").GetGuid(),
                    usuario_id = c.GetProperty("usuario_id").GetGuid(),
                    iban = c.GetProperty("iban").GetString() ?? "",
                    alias = c.GetProperty("alias").GetString() ?? "",
                    tipoCuenta = tipoGuid,
                    moneda = c.GetProperty("moneda").GetGuid(),
                    saldo = c.GetProperty("saldo").GetDecimal(),
                    estado = c.GetProperty("estado").GetGuid(),
                    fecha_creacion = c.TryGetProperty("fecha_creacion", out var fc) && fc.ValueKind == JsonValueKind.String ? fc.GetDateTime() : (DateTime?)null,
                    fecha_actualizacion = c.TryGetProperty("fecha_actualizacion", out var fa) && fa.ValueKind == JsonValueKind.String ? fa.GetDateTime() : (DateTime?)null
                });
            }

            return listaCuentas;
        }
    }
}