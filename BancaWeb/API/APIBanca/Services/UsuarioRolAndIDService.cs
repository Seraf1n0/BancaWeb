using APIBanca.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using System;

namespace APIBanca.Services
{
    public class UserRolIDService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;

        public UserRolIDService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _supabaseUrl = config["Supabase:Url"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", config["Supabase:ServiceRoleKey"]);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Supabase:ServiceRoleKey"]}");
        }

        public async Task<(string tipoIdentificacion, string identificacion)?> obtenerRoleID(Guid id)
        {
            try
            {
                var url = $"{_supabaseUrl}/rest/v1/rpc/sp_users_get_rol_and_id";


                var body = new { p_identificacion = id };
                var json = JsonSerializer.Serialize(body);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                request.Headers.Add("Prefer", "return=representation");

                var response = await _httpClient.SendAsync(request);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("ERROR:Fallo");
                    return null;
                }

                var doc = JsonDocument.Parse(responseContent);


                if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                {
                    var element = doc.RootElement[0];
                    var tipoIdentificacion = element.GetProperty("identificacion_nombre").GetString() ?? string.Empty;
                    var rol = element.GetProperty("rol").GetString() ?? string.Empty;

                    return (tipoIdentificacion, rol);
                }

                Console.WriteLine("ERROR: Array vacío");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
