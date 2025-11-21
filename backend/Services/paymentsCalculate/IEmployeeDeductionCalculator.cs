using backend.DTOs;

namespace backend.Services.PaymentsCalculate
{
    public interface IEmployeeDeductionCalculator
    {
        EmployeeDeductionLineDto Calculate(decimal grossSalary);
    }
}

