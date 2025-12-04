using System.Text.Json;
using System.Text;
using APIBanca.Models;

namespace APIBanca.Services 
{
    public class ResetPasswordRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public ResetPasswordRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<bool> ResetPassword(ResetPasswordM resetPasswordM)
        {
            
            var urlOtp = $"{_supabaseUrl}/rest/v1/rpc/sp_otp_consume";
            var bodyOtp = new {
                p_user_id = resetPasswordM.user_id,
                p_proposito = resetPasswordM.proposito,
                p_codigo_hash = resetPasswordM.codigo_hash,
            };

            var jsonBodyOtp = JsonSerializer.Serialize(bodyOtp);
            Console.WriteLine($"JSON OTP enviado: {jsonBodyOtp}");
            
            var contentOtp = new StringContent(jsonBodyOtp, Encoding.UTF8, "application/json");
            var requestOtp = new HttpRequestMessage(HttpMethod.Post, urlOtp) { Content = contentOtp };
            requestOtp.Headers.Add("apikey", _supabaseKey);

            var responseOtp = await _httpClient.SendAsync(requestOtp);
            var responseContentOtp = await responseOtp.Content.ReadAsStringAsync();

            Console.WriteLine($"Response OTP Supabase: {responseContentOtp}");

            if (!responseOtp.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase OTP ({responseOtp.StatusCode}): {responseContentOtp}");

            using var docOtp = JsonDocument.Parse(responseContentOtp);
            var rootOtp = docOtp.RootElement;

            bool otpConsumed = false;
            if (rootOtp.ValueKind == JsonValueKind.Object && rootOtp.TryGetProperty("consumed", out var otpConsumedElement))
            {
                otpConsumed = otpConsumedElement.GetBoolean();
            }
            else if (rootOtp.ValueKind == JsonValueKind.Array && rootOtp.GetArrayLength() > 0)
            {
                otpConsumed = rootOtp[0].GetProperty("consumed").GetBoolean();
            }

            if (!otpConsumed)
            {
                return false; 
            }

            
            var urlPassword = $"{_supabaseUrl}/rest/v1/rpc/sp_users_update_password";
            var bodyPassword = new {
                p_user_id = resetPasswordM.user_id,
                p_nueva_contrasena_hash = resetPasswordM.nueva_contrasena
            };

            var jsonBodyPassword = JsonSerializer.Serialize(bodyPassword);
            Console.WriteLine($"JSON Password enviado: {jsonBodyPassword}");
            
            var contentPassword = new StringContent(jsonBodyPassword, Encoding.UTF8, "application/json");
            var requestPassword = new HttpRequestMessage(HttpMethod.Post, urlPassword) { Content = contentPassword };
            requestPassword.Headers.Add("apikey", _supabaseKey);

            var responsePassword = await _httpClient.SendAsync(requestPassword);
            var responseContentPassword = await responsePassword.Content.ReadAsStringAsync();

            Console.WriteLine($"Response Password Supabase: {responseContentPassword}");

            if (!responsePassword.IsSuccessStatusCode)
                throw new HttpRequestException($"Error Supabase Password ({responsePassword.StatusCode}): {responseContentPassword}");

            using var docPassword = JsonDocument.Parse(responseContentPassword);
            var rootPassword = docPassword.RootElement;

            if (rootPassword.ValueKind == JsonValueKind.Object && rootPassword.TryGetProperty("updated", out var updatedElement))
            {
                return updatedElement.GetBoolean();
            }

            if (rootPassword.ValueKind == JsonValueKind.Array && rootPassword.GetArrayLength() > 0)
            {
                return rootPassword[0].GetProperty("updated").GetBoolean();
            }

            throw new Exception($"Formato de respuesta inesperado de Supabase: {responseContentPassword}");
        } 
    }
}