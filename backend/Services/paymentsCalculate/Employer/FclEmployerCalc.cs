using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Capitalización Laboral (patrono)
    public class FclEmployerCalc
    {
        private const decimal RATE = 0.015m; // 1.5% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("FCL_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
