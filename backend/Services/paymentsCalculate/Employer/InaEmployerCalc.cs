using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employer
{
    // Instituto Nacional de Aprendizaje (patrono)
    public class InaEmployerCalc
    {
        private readonly decimal _rate;
        public InaEmployerCalc(decimal rate) { _rate = rate; }
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
            => new EmployerDeductionLineDto
            {
                Code = "INA_PATR",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployerDeduction"
            };
    }
}