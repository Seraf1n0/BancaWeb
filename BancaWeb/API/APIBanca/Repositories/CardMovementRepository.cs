using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services 
{
    public class CardMovementRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public CardMovementRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<(Guid, decimal)> CardMovement(CardMovement card)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_card_movement_add";
            var body = new {
                p_card_id  = card.CardID,
                p_fecha   = card.FechaMovimiento,
                p_tipo = card.tipo,
                p_descripcion = card.descripcion,
                p_moneda = card.moneda,
                p_monto = card.monto    
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


            var res1 = root.GetProperty("movement_id").GetGuid();
            var res2 = root.GetProperty("nuevo_saldo_tarjeta").GetDecimal();

            Console.WriteLine($"ID MOVIMIENTO: {res1}, NUEVO SALDO: {res2}");
            return (res1,res2); 
            

        } 
    }
}