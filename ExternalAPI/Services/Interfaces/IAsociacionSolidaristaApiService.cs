using System.Text.Json;

namespace ExternalAPI.Services.Interfaces
{
    public interface IAsociacionSolidaristaApiService
    {
        Task<JsonDocument> GetAsociacionAporteAsync(decimal salarioBruto, string cedulaEmpresa, string token);
    }
}