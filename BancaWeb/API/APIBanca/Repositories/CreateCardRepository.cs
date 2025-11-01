using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services 
{
    public class CreateCardRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public CreateCardRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<Guid> CreateCard(CreateCard card)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_cards_create";
            
            var body = new {
                p_usuario_id = card.usuarioID,
                p_tipo = card.tipo,
                p_numero_enmascarado = card.numeroEnmascarado,
                p_fecha_expiracion = card.fechaExpiracion,
                p_cvv_encriptado = card.cvv_encriptado,
                p_pin_encriptado = card.pin_encriptado,
                p_moneda = card.moneda,
                p_limite_credito = card.limiteCredito,
                p_saldo_actual = card.saldoActual
            };

            var jsonBody = JsonSerializer.Serialize(body);
Console.WriteLine("JSON enviado a Supabase:");
Console.WriteLine(jsonBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {Content = content};

            request.Headers.Add("Prefer", "return=representation");
            request.Headers.Add("apikey", _supabaseKey);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error Supabase ({response.StatusCode}): {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
            {
                // Si es un array, toma el primer elemento
                var firstElement = root[0];
                return firstElement.GetProperty("card_id").GetGuid();
            }
            else
            {
                // Si es objeto directo
                return root.GetProperty("card_id").GetGuid();
            }
        }
       
    }
}