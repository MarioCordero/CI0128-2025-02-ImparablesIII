using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Invalidez, Vejez y Muerte (patrono)
    public class CcssIvmEmployerCalc : IEmployerDeductionCalculator
    {
        private readonly decimal _rate;
        
        public CcssIvmEmployerCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployerDeductionLineDto
            {
                Code = CcssIvmEmployer,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployerDeduction
            };
        }
    }
}