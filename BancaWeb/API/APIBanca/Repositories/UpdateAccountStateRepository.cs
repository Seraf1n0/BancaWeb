using System.Text.Json;
using System.Text;
using APIBanca.Models;

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
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<bool> ActualizarEstadoCuenta(UpdateEstadoCuenta updateEstadoCuenta)
        {
            if (string.IsNullOrWhiteSpace(updateEstadoCuenta.idCuenta))
            {
                Console.WriteLine("ERROR: TIENE QUE ENVIAR LA IDENTIFICACION DE LA CUENTA.");
                return false;
            }
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_update_account_state";

            var body = new { p_account_id = updateEstadoCuenta.idCuenta, p_nuevo_estado = updateEstadoCuenta.nuevoEstado };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
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
            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                updatedResult = doc.RootElement[0];
                Console.WriteLine("Estado de cuenta actualizado correctamente.");
                return true;
            }
            else
            {
                Console.WriteLine("ERROR: No se recibió un resultado válido del SP.");
                return false;
            }
        }
    }
}