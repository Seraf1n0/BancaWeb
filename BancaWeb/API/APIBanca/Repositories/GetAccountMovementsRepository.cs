using System.Text.Json;
using System.Text;
using APIBanca.Models;

namespace APIBanca.Repositories
{
    public class GetAccountMovementsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public GetAccountMovementsRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<MovimientoPage> AccountMovements(string? account_id, DateTime? start_date, DateTime? to_date, string type,
        string? q, int page, int pageSize)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_account_movements_list";
            var body = new {
                p_account_id  = account_id,
                p_from_date = start_date,
                p_to_date = to_date,
                p_type = type,
                p_q = q,
                p_page = page,
                p_page_size = pageSize 
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

            Console.WriteLine(responseContent);
            var result = JsonSerializer.Deserialize<MovimientoPage>(responseContent);
            if (result == null)
                throw new Exception("Error mapeando respuesta de Supabase");

            return result;
        }
    }
}