using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Fondo de Capitalización Laboral (patrono)
    public class FpcEmployerCalc : IEmployerDeductionCalculator
    {
        private readonly decimal _rate;
        
        public FpcEmployerCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployerDeductionLineDto
            {
                Code = FpcEmployer,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployerDeduction
            };
        }
    }
}