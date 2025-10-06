using System.Net.Http.Headers;
using System.Text.Json;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class AsociacionSolidaristaApiService : IAsociacionSolidaristaApiService
    {
        // This class only handles the external API communication. (SRP)
        private readonly HttpClient _httpClient;

        public AsociacionSolidaristaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void AddToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<JsonDocument> GetAsociacionAporteAsync(decimal salarioBruto, string cedulaEmpresa, string token)
        {
            AddToken(token);
            var response = await _httpClient.GetAsync($"https://api.asociacionsolidarista.cr/aporte-empleado?SalarioBruto={salarioBruto}&cedulaEmpresa={cedulaEmpresa}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }
    }
}