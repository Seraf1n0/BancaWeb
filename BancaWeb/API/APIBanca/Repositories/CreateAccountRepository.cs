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
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        // Método de creación de cuenta
        public async Task<Cuenta> CreateAccountAsync(CreateCuenta newAccount)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_create_account";

            var body = new
            {
                usuario_id = newAccount.usuario_id,
                iban = newAccount.iban,
                alias = newAccount.alias,
                tipo = newAccount.tipo,
                moneda = newAccount.moneda,
                saldo_inicial = newAccount.saldo_inicial,
                estado = newAccount.estado
            };

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            // Con esto supabase devuelve la cuenta creada
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error Supabase: {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);

            // Intentamos recuperar el id creado
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (doc.RootElement.TryGetProperty("account_id", out var idProp))
                {
                    // Intentamos devolver el GUID
                    return Guid.TryParse(idProp.GetString(), out var accountId)
                        ? new Cuenta { id = accountId }
                        : throw new Exception("El ID de la cuenta no es un GUID válido.");
                }
            }
        }
    }
}