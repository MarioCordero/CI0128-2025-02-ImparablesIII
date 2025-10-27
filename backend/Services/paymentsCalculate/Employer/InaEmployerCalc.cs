using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Instituto Nacional de Aprendizaje (patrono)
    public class InaEmployerCalc
    {
        private const decimal RATE = 0.015m; // 1.5% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("INA_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
