using System.Text.Json;
using System.Text;
using APIBanca.Models;

namespace APIBanca.Services
{
    public class InfoUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public InfoUserRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<User?> ObtenerUsuarioID(string identificacion)
        {


            if (string.IsNullOrWhiteSpace(identificacion))
            {
                Console.WriteLine("ERROR: La identificación está vacía o nula.");
                return null;
            }

            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_users_get_by_identification?p_identificacion={identificacion}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Prefer", "return=representation");



            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Contenido: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("ERROR: La llamada al SP falló.");
                return null;
            }

            var doc = JsonDocument.Parse(responseContent);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var element = doc.RootElement[0];

                return new User
                {
                    p_identificacion = identificacion,
                    p_nombre = element.GetProperty("nombre").GetString() ?? string.Empty,
                    p_apellido = element.GetProperty("apellido").GetString() ?? string.Empty,
                    p_correo = element.GetProperty("correo").GetString() ?? string.Empty,
                    p_usuario = element.GetProperty("usuario").GetString() ?? string.Empty,
                    p_rol = element.GetProperty("rol").GetInt32()
                };
            }

            Console.WriteLine("No se encontró usuario en la respuesta.");
            return null;
        }
    }
}
