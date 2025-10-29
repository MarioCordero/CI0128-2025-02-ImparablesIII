using backend.DTOs;

namespace backend.Services
{
    public interface IPayrollService
    {
        // Task<PayrollSummaryDto> GetReportAsync(PayrollFiltersDto filters);
        // Task<EmployeeDeductionsResponse> EmployeeDeductionsAsync(EmployeeDeductionsRequest request);
        Task<List<EmployeePayrollDto>> GetEmployeePayrollWithDeductionsAsync(int companyId);
        Task<List<EmployerDeductionResultDto>> GetEmployerPayrollWithDeductionsAsync(int companyId);
        // Task<PayrollCalculationResultDto> CalculatePayrollAsync(PayrollCalculationRequestDto request);
    }
}