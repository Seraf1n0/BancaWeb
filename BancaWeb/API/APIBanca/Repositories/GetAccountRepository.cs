using System.Text.Json;
using System.Text;
using APIBanca.Models;

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
            request.Headers.Add("apikey", _supabaseKey);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase ({response.StatusCode}): {responseContent}");

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                throw new Exception("No se encontraron cuentas");

            var listaCuentas = new List<Cuenta>();

            for (var i = 0; i < root.GetArrayLength(); i++) {
                var c = root[i];
                listaCuentas.Add(new Cuenta
                {
                    id = c.GetProperty("id").GetGuid(),
                    usuario_id = c.GetProperty("usuario_id").GetGuid(),
                    iban = c.GetProperty("iban").GetString() ?? "",
                    alias = c.GetProperty("alias").GetString() ?? "",
                    tipoCuenta = c.GetProperty("tipo").GetGuid(),
                    moneda = c.GetProperty("moneda").GetGuid(),
                    saldo = c.GetProperty("saldo").GetDecimal(),
                    estado = c.GetProperty("estado").GetGuid()
                });
            }

            return listaCuentas;
        }
    }
}