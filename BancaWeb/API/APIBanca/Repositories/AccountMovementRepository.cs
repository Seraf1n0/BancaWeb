using System.Text.Json;
using System.Text;
using APIBanca.Models;

namespace APIBanca.Repositories 
{
    public class GetAccountMovementsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public GetAccountMovementsRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }
        public async Task<(Guid, string, string)> AccountMovement(Movimiento movimiento)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_transfer_create_internal";
            var body = new {
                p_account_id  = movimiento.AccountID,
                p_fecha   = movimiento.FechaMovimiento,
                p_tipo = movimiento.tipo,
                p_descripcion = movimiento.descripcion,
                p_moneda = movimiento.moneda,
                p_monto = movimiento.monto    
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

            var res1 = root.GetProperty("transfer_id").GetGuid();
            var res2 = root.GetProperty("receipt_number").GetString();
            var res3 = root.GetProperty("status").GetString();
            return (res1, res2, res3); // Retorna el ID de la transferencia, número de recibo y estado
    }
}