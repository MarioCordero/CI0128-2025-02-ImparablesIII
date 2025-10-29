using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employer
{
    // Invalidez, Vejez y Muerte (patrono)
    public class CcssIvmEmployerCalc
    {
        private readonly decimal _rate;
        public CcssIvmEmployerCalc(decimal rate) { _rate = rate; }
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
            => new EmployerDeductionLineDto
            {
                Code = "CCSS_IVM_ER",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployerDeduction"
            };
    }
}