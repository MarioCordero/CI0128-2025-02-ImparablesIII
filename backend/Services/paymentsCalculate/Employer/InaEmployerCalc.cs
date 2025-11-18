using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Instituto Nacional de Aprendizaje (patrono)
    public class InaEmployerCalc : IEmployerDeductionCalculator
    {
        private readonly decimal _rate;
        
        public InaEmployerCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployerDeductionLineDto
            {
                Code = InaEmployer,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployerDeduction
            };
        }
    }
}