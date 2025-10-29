using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employee
{
    // Aporte trabajador Banco Popular
    public class BancoPopularEmployeeCalc
    {
        private readonly decimal _rate;
        public BancoPopularEmployeeCalc(decimal rate) { _rate = rate; }
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
            => new EmployeeDeductionLineDto
            {
                Code = "BP_TRAB",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployeeDeduction"
            };
    }
}