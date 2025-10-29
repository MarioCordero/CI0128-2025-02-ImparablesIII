using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Desarrollo Social y Asignaciones Familiares (patrono)
    public class FodesafEmployerCalc
    {
        private readonly decimal _rate;
        public FodesafEmployerCalc(decimal rate) { _rate = rate; }
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
            => new EmployerDeductionLineDto
            {
                Code = "FODESAF_PATR",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployerDeduction"
            };
    }
}