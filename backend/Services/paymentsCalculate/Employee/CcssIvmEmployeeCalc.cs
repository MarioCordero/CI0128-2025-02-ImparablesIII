using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employee
{
    // Invalidez, Vejez y Muerte (trabajador)
    public class CcssIvmEmployeeCalc
    {
        private readonly decimal _rate;
        public CcssIvmEmployeeCalc(decimal rate) { _rate = rate; }
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
            => new EmployeeDeductionLineDto
            {
                Code = "CCSS_IVM_EE",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployeeDeduction"
            };
    }
}