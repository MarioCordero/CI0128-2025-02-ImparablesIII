using backend.Services.PaymentsCalculate;

namespace backend.Services.PaymentsCalculate.Employee
{
    // Invalidez, Vejez y Muerte (trabajador)
    public class CcssIvmEmployeeCalc
    {
        private const decimal RATE = 0.0417m; // 4.17% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("CCSS_IVM_TRAB", Math.Round(grossSalary * RATE, 2), CalcRole.EmployeeDeduction);
    }
}