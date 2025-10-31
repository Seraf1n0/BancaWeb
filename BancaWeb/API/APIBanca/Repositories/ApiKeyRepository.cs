using System.Text.Json;

namespace APIBanca.Services
{
    public class ApiKeyRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;

        public ApiKeyRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _supabaseUrl = config["Supabase:Url"] ?? "";
            var supabaseKey = config["Supabase:Key"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
        }

        public async Task<bool> CrearApiKeyAsync(Guid userId, string apiKey)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_api_key_create";
            
            // Esto es un hash aplicado con BCrypt
            string apiKeyHash = BCrypt.Net.BCrypt.HashPassword(apiKey);
            
            var body = new
            {
                p_api_hash = apiKeyHash,
                p_label = "Default Key",
                p_active = true,
                p_user_id = userId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[DEBUG] Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al crear API Key: {responseContent}");
            }

            return true;
        }
    }
}