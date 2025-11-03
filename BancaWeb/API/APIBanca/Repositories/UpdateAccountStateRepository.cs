using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace APIBanca.Repositories
{
    public class UpdateAccountStateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public UpdateAccountStateRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"]!;
            _supabaseKey = configuration["Supabase:ServiceRoleKey"]!;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        }

        public async Task<bool> SetStatusAsync(Guid accountId, Guid nuevoEstado)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_accounts_set_status";
            var body = new { p_account_id = accountId, p_nuevo_estado = nuevoEstado };

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Prefer", "return=representation");

            var resp = await _httpClient.SendAsync(req);
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase ({resp.StatusCode}): {content}");

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("updated", out var u)) return u.GetBoolean();
            if (root.ValueKind == JsonValueKind.Array) return root.GetArrayLength() > 0;
            return true;
        }

        public async Task<Guid?> GetEstadoIdByNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            var term = nombre.Trim();

            async Task<Guid?> TryUrlAsync(string url)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Add("apikey", _supabaseKey);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);

                var resp = await _httpClient.SendAsync(req);
                var json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"[REST estadoCuenta] {url} -> {resp.StatusCode} {json}");
                if (!resp.IsSuccessStatusCode) return null;

                using var doc = JsonDocument.Parse(json);
                var arr = doc.RootElement;
                if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0)
                {
                    var idStr = arr[0].GetProperty("id").GetString();
                    if (Guid.TryParse(idStr, out var g)) return g;
                }
                return null;
            }

            var urlEq = $"{_supabaseUrl}/rest/v1/estadoCuenta?select=id&nombre=eq.{Uri.EscapeDataString(term)}&limit=1";
            var id = await TryUrlAsync(urlEq);
            if (id != null) return id;

            var urlIlike = $"{_supabaseUrl}/rest/v1/estadoCuenta?select=id&nombre=ilike.*{Uri.EscapeDataString(term)}*&limit=1";
            return await TryUrlAsync(urlIlike);
        }
    }
}