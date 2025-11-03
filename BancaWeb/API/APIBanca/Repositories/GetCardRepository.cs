using System.Text.Json;
using System.Text;
using APIBanca.Models;


namespace APIBanca.Services 
{
    public class GetCardRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public GetCardRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<List<GetCard>> GetCard(string? userId, string? cardId)
        {
            var url = $"{_supabaseUrl}/rest/v1/rpc/sp_cards_get";
            var body = new {
                p_owner_id  = userId,
                p_card_id   = cardId,
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

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                throw new Exception("No se encontró la tarjeta");

            var listaCards = new List<GetCard>();

            for (var i = 0; i < root.GetArrayLength(); i++) {
                var c = root[i];
                listaCards.Add(new GetCard
                {
                    id = c.GetProperty("id").GetGuid(),
                    usuarioID = c.GetProperty("usuario_id").GetGuid(),
                    tipo = c.GetProperty("tipo").GetGuid(),
                    nombreTarjeta = c.GetProperty("nombre_tipo_tarjeta").GetString(),
                    numeroEnmascarado = c.GetProperty("numero_enmascarado").GetString(),
                    fechaExpiracion = c.GetProperty("fecha_expiracion").GetString(),
                    moneda = c.GetProperty("moneda").GetGuid(),
                    nombreMoneda = c.GetProperty("nombre_moneda").GetString(),
                    limiteCredito = c.GetProperty("limite_credito").GetDecimal(),
                    saldoActual = c.GetProperty("saldo_actual").GetDecimal(),
                    saldoDisponible = c.GetProperty("disponible").GetDecimal(),
                    fechaCreacion = c.GetProperty("fecha_creacion").GetDateTime(),
                    fechaActualizacion = c.GetProperty("fecha_actualizacion").GetDateTime()
                }
                );
            };

            foreach (var tarjeta in listaCards)
            {
                Console.WriteLine(
                    $"ID: {tarjeta.id}, " +
                    $"UsuarioID: {tarjeta.usuarioID}, " +
                    $"Tipo: {tarjeta.tipo}, " +
                    $"NombreTarjeta: {tarjeta.nombreTarjeta}, " +
                    $"NúmeroEnmascarado: {tarjeta.numeroEnmascarado}, " +
                    $"FechaExpiracion: {tarjeta.fechaExpiracion}, " +
                    $"MonedaID: {tarjeta.moneda}, " +
                    $"NombreMoneda: {tarjeta.nombreMoneda}, " +
                    $"LimiteCredito: {tarjeta.limiteCredito}, " +
                    $"SaldoActual: {tarjeta.saldoActual}, " +
                    $"SaldoDisponible: {tarjeta.saldoDisponible}, " +
                    $"FechaCreacion: {tarjeta.fechaCreacion}, " +
                    $"FechaActualizacion: {tarjeta.fechaActualizacion}"
                );
            }

            return listaCards;


        } 
    }
}