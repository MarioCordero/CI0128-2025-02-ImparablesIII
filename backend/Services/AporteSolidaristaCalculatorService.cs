using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class AporteSolidaristaCalculatorService : IAporteSolidaristaCalculatorService
    {
        // This class only handles the calculation and validation of the input parameters. (SRP)
        public bool ValidateInput(string? cedulaEmpresa, decimal salarioBruto)
        {
            if (string.IsNullOrEmpty(cedulaEmpresa) || salarioBruto <= 0)
                return false;

            char ultimoDigito = cedulaEmpresa[^1];
            return char.IsDigit(ultimoDigito);
        }

        public string? GetValidationError(string? cedulaEmpresa, decimal salarioBruto)
        {
            if (string.IsNullOrEmpty(cedulaEmpresa))
                return "cedulaEmpresa is required";
                
            if (salarioBruto <= 0)
                return "salarioBruto must be positive";
                
            char ultimoDigito = cedulaEmpresa[^1];
            if (!char.IsDigit(ultimoDigito))
                return "cedulaEmpresa debe terminar en un dígito";
                
            return null;
        }

        public AporteSolidaristaResponse CalculateAporte(string cedulaEmpresa, decimal salarioBruto)
        {
            if (!ValidateInput(cedulaEmpresa, salarioBruto))
                throw new ArgumentException("Invalid input parameters");

            char ultimoDigito = cedulaEmpresa[^1];
            int digito = int.Parse(ultimoDigito.ToString());
            
            var (porcentajeEmpleado, porcentajeEmpleador) = GetPorcentajes(digito);
            
            return new AporteSolidaristaResponse
            {
                Deductions = new List<AporteSolidarista>
                {
                    new AporteSolidarista { Type = "EE", Amount = Math.Round(salarioBruto * porcentajeEmpleado, 2) },
                    new AporteSolidarista { Type = "ER", Amount = Math.Round(salarioBruto * porcentajeEmpleador, 2) }
                }
            };
        }

        // Internal Method
        private static (decimal empleado, decimal empleador) GetPorcentajes(int digito)
        {
            return digito >= 0 && digito <= 4
                ? (0.05m, 0.0533m)      // 5% y 5.33%
                : (0.035m, 0.0383m);    // 3.5% y 3.83%
        }
    }
}