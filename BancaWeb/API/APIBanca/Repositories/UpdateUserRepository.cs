using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services
{
    public class UpdateUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public UpdateUserRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<bool> ActualizarUsuario(string idUsuario, string? nombre, string? apellido, string? correo,
        string? usuario, int? rol)
        {
            if (string.IsNullOrWhiteSpace(idUsuario))
            {
                Console.WriteLine("ERROR: TIENE QUE ENVIAR LA IDENTIFICACION.");
                return false;
            }
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_users_update";
            

            var body = new{p_user_id = idUsuario,p_nombre = nombre,p_apellido = apellido, p_correo = correo, p_usuario = usuario,
                p_rol = rol};




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

            JsonElement updatedResult;
            var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("updated", out updatedResult))
            {
                return updatedResult.GetBoolean();
            }
            return false;

        }
       
    }
}