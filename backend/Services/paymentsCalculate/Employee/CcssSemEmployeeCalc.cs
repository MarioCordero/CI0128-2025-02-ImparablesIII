using backend.DTOs;
using backend.Services.PaymentsCalculate;
using static backend.Constants.DeductionCodes;
using static backend.Constants.DeductionRoleNames;

namespace backend.Services.PaymentsCalculate.Employee
{
    // Seguro de Enfermedad y Maternidad (trabajador)
    public class CcssSemEmployeeCalc : IEmployeeDeductionCalculator
    {
        private readonly decimal _rate;
        
        public CcssSemEmployeeCalc(decimal rate) 
        { 
            _rate = rate; 
        }
        
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
        {
            return new EmployeeDeductionLineDto
            {
                Code = CcssSemEmployee,
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = EmployeeDeduction
            };
        }
    }
}