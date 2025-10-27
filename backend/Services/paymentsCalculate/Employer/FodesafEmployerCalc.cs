using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // FODESAF / Asignaciones Familiares (patrono)
    public class FodesafEmployerCalc
    {
        private const decimal RATE = 0.05m; // 5% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("FODESAF_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
