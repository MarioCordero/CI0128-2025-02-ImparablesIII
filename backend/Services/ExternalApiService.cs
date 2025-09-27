using System.Net.Http.Headers;
using System.Text.Json;

namespace backend.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void AddToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<JsonDocument> GetHaciendaDeduccionAsync(decimal salarioBruto, string token)
        {
            AddToken(token);
            var response = await _httpClient.GetAsync($"https://api.hacienda.go.cr/deduccion-renta-empleado?SalarioBruto={salarioBruto}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }

        public async Task<JsonDocument> GetCCSSDeduccionAsync(decimal salarioBruto, string token)
        {
            AddToken(token);
            var response = await _httpClient.GetAsync($"https://api.ccss.go.cr/deducciones-salario?SalarioBruto={salarioBruto}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
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
