using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Invalidez, Vejez y Muerte (patrono)
    public class CcssIvmEmployerCalc
    {
        private const decimal RATE = 0.0542m; // 5.42% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("CCSS_IVM_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}