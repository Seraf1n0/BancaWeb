using System.Text.Json;
using APIBanca.Models;

public class AuthUsuarioRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;

    public AuthUsuarioRepository(IConfiguration config, HttpClient httpClient)
    {
        _supabaseUrl = config["Supabase:Url"] ?? "";
        var supabaseKey = config["Supabase:Key"];

        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    }

    public async Task<UserAuthResponse?> ObtenerUsuarioPorUsernameOEmailAsync(string usernameOrEmail)
    {
        var url = $"{_supabaseUrl}/rest/v1/rpc/sp_auth_user_get_by_username_or_email"; 

        var body = new { p_username_or_email = usernameOrEmail };

        var jsonBody = JsonSerializer.Serialize(body);
        Console.WriteLine($"[DEBUG] URL: {url}");
        Console.WriteLine($"[DEBUG] Body enviado: {jsonBody}");

        var content = new StringContent(
            jsonBody, 
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
            throw new HttpRequestException($"Error Supabase ({response.StatusCode}): {responseContent}");
        }

        using var doc = JsonDocument.Parse(responseContent);
        
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            if (doc.RootElement.GetArrayLength() == 0)
            {
                return null;
            }

            var firstElement = doc.RootElement[0];
            
            return new UserAuthResponse
            {
                UserId = Guid.Parse(firstElement.GetProperty("user_id").GetString() ?? ""),
                ContrasenaHash = firstElement.GetProperty("contrasena_hash").GetString() ?? "",
                Rol = firstElement.GetProperty("rol").GetInt32()
            };
        }

        return null;
    }
}