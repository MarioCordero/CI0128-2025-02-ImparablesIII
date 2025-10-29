using backend.DTOs;
namespace backend.Services.PaymentsCalculate.Employee
{
    // Seguro de Enfermedad y Maternidad (trabajador)
    public class CcssSemEmployeeCalc
    {
        private readonly decimal _rate;
        public CcssSemEmployeeCalc(decimal rate) { _rate = rate; }
        public EmployeeDeductionLineDto Calculate(decimal grossSalary)
            => new EmployeeDeductionLineDto
            {
                Code = "CCSS_SEM_EE",
                Amount = Math.Round(grossSalary * _rate, 2),
                Role = "EmployeeDeduction"
            };
    }
}