using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Seguro de Enfermedad y Maternidad (patrono)
    public class CcssSemEmployerCalc
    {
        private const decimal RATE = 0.0925m; // 9.25% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("CCSS_SEM_PATR", Math.Round(grossSalary * RATE, 2), CalcRole.EmployerDeduction);
    }
}
