using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Aporte patronal Banco Popular
    public class BancoPopularEmployerCalc
    {
        private const decimal RATE = 0.0025m; // 0.25% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("BP_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
