using backend_lab.DTOs;

namespace backend_lab.Services
{
    public interface IPayrollService
    {
        Task<PayrollSummaryDto> GetPayrollSummaryAsync(PayrollFiltersDto filters);
        Task<PayrollDetailDto> CalculateEmployeePayrollAsync(PayrollCalculationRequestDto request);
        Task<bool> ProcessPayrollAsync(PayrollFiltersDto filters);
        Task<IEnumerable<PayrollDetailDto>> GetPayrollHistoryAsync(int employeeId);
    }
}