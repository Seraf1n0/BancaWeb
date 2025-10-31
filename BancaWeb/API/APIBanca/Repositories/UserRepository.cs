using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services 
{   
    public class UserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;

        public UserRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _supabaseUrl = config["Supabase:Url"];
            var supabaseKey = config["Supabase:Key"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
        }

    public async Task<Guid> CrearUsuarioAsync(User usuario)
    {
        var url = $"{_supabaseUrl}/rest/v1/rpc/sp_users_create";

        var body = new
        {
            p_tipo_identificacion = usuario.p_tipo_identificacion,
            p_identificacion = usuario.p_identificacion,
            p_nombre = usuario.p_nombre,
            p_apellido = usuario.p_apellido,
            p_correo = usuario.p_correo,
            p_telefono = usuario.p_telefono,
            p_usuario = usuario.p_usuario,
            p_contrasena_hash = usuario.p_contrasena_hash,
            p_rol = usuario.p_rol
        };

        var jsonBody = JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        //Esto es para que supabase me devuelva lo que creamos en la tabla según el sp
        request.Headers.Add("Prefer", "return=representation");
        var response = await _httpClient.SendAsync(request);

        // Esperamos la respuesta
        var responseContent = await response.Content.ReadAsStringAsync();

        //Si la respuesta no llega buena enviamos un error
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error Supabase: {responseContent}");
        }

    
        using var doc = JsonDocument.Parse(responseContent);
        
        if (doc.RootElement.ValueKind == JsonValueKind.Object)
        {
            if (doc.RootElement.TryGetProperty("user_id", out var idProp))
            {
                return Guid.Parse(idProp.GetString() ?? "");
            }
        } 

        throw new Exception("No se pudo obtener el user_id de la respuesta.");

            
    }


    }
}