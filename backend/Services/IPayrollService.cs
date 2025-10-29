using backend.DTOs;

namespace backend.Services
{
    public interface IPayrollService
    {
        Task<List<EmployeePayrollDto>> GetEmployeePayrollWithDeductionsAsync(int companyId);
        Task<List<EmployerDeductionResultDto>> GetEmployerPayrollWithDeductionsAsync(int companyId);
        Task<int> GeneratePayrollWithBenefitsAsync(int companyId, int responsibleEmployeeId, int hours);
    }
}