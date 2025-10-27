namespace backend.Services.PaymentsCalculate.Employee
{
    // Reducción de impuesto de salario (renta)
    public class SalaryTaxEmployeeCalc
    {
        // Tramos y tasas
        private static readonly (decimal Min, decimal Max, decimal Rate)[] Brackets = new[]
        {
            (0m,        922000m,    0.00m),
            (922000m,   1352000m,   0.10m),
            (1352000m,  2373000m,   0.15m),
            (2373000m,  4750000m,   0.20m),
            (4750000m,  decimal.MaxValue, 0.25m)
        };

        public CalcLine Calculate(decimal grossSalary)
        {
            decimal tax = 0m;
            foreach (var (min, max, rate) in Brackets)
            {
                if (grossSalary > min)
                {
                    var taxable = Math.Min(grossSalary, max) - min;
                    tax += taxable * rate;
                }
            }
            tax = Math.Round(tax, 2);
            return new CalcLine("RENTA", tax, CalcRole.EmployeeDeduction);
        }
    }
}