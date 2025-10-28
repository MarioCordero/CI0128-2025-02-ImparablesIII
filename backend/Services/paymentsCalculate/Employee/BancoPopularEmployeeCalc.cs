namespace backend.Services.PaymentsCalculate.Employee
{
    // Aporte trabajador Banco Popular
    public class BancoPopularEmployeeCalc
    {
        private const decimal RATE = 1m; // 1% (valor quemado)
        public CalcLine Calculate(decimal grossSalary)
            => new("BP_TRAB", Math.Round(grossSalary * RATE, 2), CalcRole.EmployeeDeduction);
    }
}