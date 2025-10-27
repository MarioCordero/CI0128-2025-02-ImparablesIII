namespace backend.Services.PaymentsCalculate.Employee
{
    // Seguro de Enfermedad y Maternidad (trabajador)
    public class CcssSemEmployeeCalc
    {
        private const decimal RATE = 0.055m; // 5.50% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("CCSS_SEM_TRAB", Math.Round(grossSalary * RATE, 2), CalcRole.EmployeeDeduction);
    }
}