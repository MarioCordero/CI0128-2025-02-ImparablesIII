using System.Text.Json;

namespace backend.Services.Interfaces
{
    public interface IAsociacionSolidaristaApiService
    {
        Task<JsonDocument> GetAsociacionAporteAsync(decimal salarioBruto, string cedulaEmpresa, string token);
    }
}