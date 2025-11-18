using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Capitalización Laboral (patrono)
    public class FclEmployerCalc : IEmployerDeductionCalculator
    {
        private readonly decimal _rate;
        
        public FclEmployerCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployerDeductionLineDto
            {
                Code = FclEmployer,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployerDeduction
            };
        }
    }
}