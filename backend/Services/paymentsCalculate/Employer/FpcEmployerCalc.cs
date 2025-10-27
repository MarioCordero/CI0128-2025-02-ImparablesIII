using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Pensiones Complementarias (patrono)
    public class FpcEmployerCalc
    {
        private const decimal RATE = 0.02m; // 2% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("FPC_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
