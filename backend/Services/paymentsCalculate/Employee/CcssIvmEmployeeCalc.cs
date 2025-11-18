using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employee
{
    // Invalidez, Vejez y Muerte (trabajador)
    public class CcssIvmEmployeeCalc : IEmployeeDeductionCalculator
    {
        private readonly decimal _rate;
        
        public CcssIvmEmployeeCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployeeDeductionLineDto
            {
                Code = CcssIvmEmployee,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployeeDeduction
            };
        }
    }
}