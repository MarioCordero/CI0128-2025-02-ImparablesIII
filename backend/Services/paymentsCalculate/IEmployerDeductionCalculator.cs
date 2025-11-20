using backend.DTOs;

namespace backend.Services.PaymentsCalculate
{
    public interface IEmployerDeductionCalculator
    {
        EmployerDeductionLineDto Calculate(decimal grossSalary);
    }
}

