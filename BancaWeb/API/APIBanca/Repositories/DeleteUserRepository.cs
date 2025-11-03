using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services
{
    public class DeleteUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public DeleteUserRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<bool> EliminarUsuario(string idUsuario)
        
        {
            if (string.IsNullOrWhiteSpace(idUsuario))
            {
                Console.WriteLine("ERROR: TIENE QUE ENVIAR LA IDENTIFICACION.");
                return false;
            }
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_users_delete";

            var body = new{p_user_id = idUsuario};




            var request = new HttpRequestMessage(HttpMethod.Post, url){
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Contenido: {responseContent}");


            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("ERROR: La llamada al SP falló.");
                return false;
            }

            JsonElement deletedResult;
            var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("deleted", out deletedResult))
            {
                return deletedResult.GetBoolean();
            }
            return false;

        }
       
    }
}