using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employee
{
    // Aporte trabajador Banco Popular
    public class BancoPopularEmployeeCalc : IEmployeeDeductionCalculator
    {
        private readonly decimal _rate;
        
        public BancoPopularEmployeeCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployeeDeductionLineDto
            {
                Code = BancoPopularEmployee,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployeeDeduction
            };
        }
    }
}