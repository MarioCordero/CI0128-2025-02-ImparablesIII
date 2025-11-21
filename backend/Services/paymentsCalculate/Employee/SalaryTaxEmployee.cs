using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employee
{
    // Reducción de impuesto de salario (renta)
    public class SalaryTaxEmployeeCalc : IEmployeeDeductionCalculator
    {
        private readonly decimal _rate;
        private readonly decimal _min;
        private readonly decimal? _max;
        
        public SalaryTaxEmployeeCalc(decimal rate, decimal min, decimal? max)
        {
            _rate = rate;
            _min = min;
            _max = max;
        }
        
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
        {
            decimal amount = 0m;
            if (grossSalary > _min)
            {
                decimal baseAmount = _max.HasValue
                    ? Math.Min(grossSalary, _max.Value) - _min
                    : grossSalary - _min;
                amount = baseAmount * _rate;
            }
            
            return new EmployeeDeductionLineDto
            {
                Code = SalaryTax,
                Amount = Math.Round(amount, 2),
                Role = EmployeeDeduction
            };
        }
    }
}