using backend.DTOs;

namespace backend.Services
{
    public interface IPayrollService
    {
        Task<PayrollSummaryDto> GetReportAsync(PayrollFiltersDto filters);
        Task<TestEmployeeDeductionsResponse> TestEmployeeDeductionsAsync(TestEmployeeDeductionsRequest request); // TEST
    }
}