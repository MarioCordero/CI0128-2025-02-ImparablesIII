using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IAporteSolidaristaCalculatorService
    {
        AporteSolidaristaResponse CalculateAporte(string cedulaEmpresa, decimal salarioBruto);
        bool ValidateInput(string? cedulaEmpresa, decimal salarioBruto);
        string? GetValidationError(string? cedulaEmpresa, decimal salarioBruto);
    }
}