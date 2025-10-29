using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Capitalización Laboral (patrono)
    public class FpcEmployerCalc
    {
        private readonly decimal _rate;
        public FpcEmployerCalc(decimal rate) { _rate = rate; }
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
            => new EmployerDeductionLineDto
            {
                Code = "FPC_PATR",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployerDeduction"
            };
    }
}