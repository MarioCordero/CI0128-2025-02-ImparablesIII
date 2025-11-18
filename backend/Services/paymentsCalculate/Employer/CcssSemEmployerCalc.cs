using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employer
{
    // Seguro de Enfermedad y Maternidad (patrono)
    public class CcssSemEmployerCalc : IEmployerDeductionCalculator
    {
        private readonly decimal _rate;
        
        public CcssSemEmployerCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployerDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployerDeductionLineDto
            {
                Code = CcssSemEmployer,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployerDeduction
            };
        }
    }
}
