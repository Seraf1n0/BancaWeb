using System.Text.Json;


public class AuthUsuarioRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;

    public AuthUsuarioRepository(IConfiguration config, HttpClient httpClient)
    {
        _supabaseUrl = config["Supabase:Url"];
        var supabaseKey = config["Supabase:Key"];

        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
    }

    public async Task<bool> ValidarCredencialesAsync(string username, string password)
    {
        var url = $"{_supabaseUrl}/rest/v1/rpc/sp_auth_user_check_credentials";
        var body = new { p_username = username, p_password = password };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return false;

        return JsonSerializer.Deserialize<bool>(responseContent);
    }
}
