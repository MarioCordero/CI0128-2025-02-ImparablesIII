using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employer
{
    // Banco Popular (patrono)
    public class BancoPopularEmployerCalc
    {
        private readonly decimal _rate;
        public BancoPopularEmployerCalc(decimal rate) { _rate = rate; }
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
            => new EmployerDeductionLineDto
            {
                Code = "BP_PATRON",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployerDeduction"
            };
    }
}