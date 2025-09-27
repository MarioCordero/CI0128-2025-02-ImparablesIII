using System.Net.Http.Headers;
using System.Text.Json;

namespace backend.Services
{
    /// <summary>
    /// Servicio para consumir la API externa de Asociación Solidarista.
    /// Permite obtener los aportes solidaristas del empleado y empleador usando salario bruto y cédula de empresa.
    /// </summary>
    public class AsociacionSolidaristaApiService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa el servicio con un HttpClient inyectado.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP para realizar las peticiones.</param>
        public AsociacionSolidaristaApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Agrega el token de autenticación al header de la petición HTTP.
        /// </summary>
        /// <param name="token">Token de autenticación tipo Bearer.</param>
        private void AddToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Llama a la API externa de Asociación Solidarista para obtener los aportes.
        /// </summary>
        /// <param name="salarioBruto">Salario bruto del empleado.</param>
        /// <param name="cedulaEmpresa">Cédula de la empresa.</param>
        /// <param name="token">Token de autenticación.</param>
        /// <returns>Un JsonDocument con la respuesta de la API externa.</returns>
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