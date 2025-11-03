using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services 
{
    public class OtpCreateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public OtpCreateRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<Guid> OtpCreate(OtpCreateM otp)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_otp_create";
            var body = new {
                p_user_id = otp.usuario_id,
                p_proposito = otp.proposito,
                p_expires_in_seconds = otp.tiempoExpiracionSegundos,
                p_codigo_hash = otp.codigo_hash
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

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("otp_id", out var otpIdElement))
            {
                return otpIdElement.GetGuid();
            }

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                return root[0].GetProperty("otp_id").GetGuid();
            } 
            throw new Exception($"Respuesta Supabase: {responseContent}");


        } 
    }
}