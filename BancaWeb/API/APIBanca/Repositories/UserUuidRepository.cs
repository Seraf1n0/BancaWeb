using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace APIBanca.Repositories
{
    public class UserUuidRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;

        public UserUuidRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _supabaseUrl = config["Supabase:Url"] ?? "";
            var supabaseKey = config["Supabase:Key"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
        }

        public async Task<Guid?> GetUserIdByUsernameAsync(string username)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_get_user";
            var body = new { p_username = username };
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
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al obtener UUID: {responseContent}");
            }
            if (Guid.TryParse(responseContent.Trim('\"'), out var guid))
                return guid;
            return null;
        }
    }
}