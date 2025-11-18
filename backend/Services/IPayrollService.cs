using backend.DTOs;

namespace backend.Services
{
    public interface IPayrollService
    {
        Task<List<EmployeePayrollDto>> GetEmployeePayrollWithDeductionsAsync(int companyId);
        Task<List<EmployerDeductionResultDto>> GetEmployerPayrollWithDeductionsAsync(int companyId);
        Task<int> GeneratePayrollWithBenefitsAsync(int companyId, int responsibleEmployeeId, int hours, string? periodType = null, int? fortnight = null);
        Task<PayrollTotalsDto?> GetLatestPayrollTotalsByCompanyAsync(int companyId);
        Task<List<PayrollHistoryItemDto>> GetPayrollHistoryByCompanyAsync(int companyId);
        Task<DetailedPayrollReportDto?> GetDetailedPayrollReportAsync(int employeeId, int payrollId, int authenticatedEmployeeId);
        Task<List<EmployeePayrollReportDto>> GetEmployeePayrollReportsAsync(int employeeId, int authenticatedEmployeeId, int? year = null, int? month = null, string? puesto = null);
    }
}