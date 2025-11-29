using System.Text.Json;
using System.Text;
using APIBanca.Models;

namespace APIBanca.Services 
{
    public class BankValidateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public BankValidateRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:ServiceRoleKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
        }

        public async Task<BankValidate> VerificarCuenta(string iban)
        {
            try {
                var url = $"{_supabaseUrl}/rest/v1/rpc/sp_bank_validate_account";
                
                var body = new {
                    p_iban = iban,
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
                    var firstElement = root[0];
                    return new BankValidate
                    {
                        exists = firstElement.GetProperty("exists_account").GetBoolean(),
                        info = new Info
                        {
                            name = firstElement.GetProperty("name").GetString(),
                            identification = firstElement.GetProperty("identification").GetString(),
                            currency = firstElement.GetProperty("currency").GetString(),
                            debit = firstElement.GetProperty("debit").GetBoolean(),
                            credit = firstElement.GetProperty("credit").GetBoolean()
                        }
                    };
                }
                else
                {
                    throw new Exception($"Formato de respuesta inesperado: {responseContent}");
                }
            }  catch (Exception ex)
            {
                // Pasamos la excepción hacia el servicio
                throw new Exception("Error al verificar la cuenta bancaria: " + ex.Message, ex);
            }
        }
       
    }
}