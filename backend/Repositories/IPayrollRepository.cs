using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IPayrollRepository
    {
        Task<List<EmployeeRow>> GetEmployeesAsync(int companyId, string? department);
        Task<List<PayrollRow>> GetPayrollsAsync(int companyId, DateTime start, DateTime end);
        Task<List<PayrollDetailRow>> GetPayrollDetailsAsync(IEnumerable<int> payrollIds, IEnumerable<int> employeeIds);
        Task<List<DeductionEmployeeRow>> GetDeductionsAsync(IEnumerable<int> payrollIds, IEnumerable<int> employeeIds);
        Task<List<BenefitRow>> GetBenefitsAsync(int companyId);
        Task<List<BenefitEmployeeRow>> GetEmployeeBenefitsAsync(int companyId, IEnumerable<int> employeeIds);
        Task<(List<EmployeePayrollDto> Items, PayrollTotalsDto Totals)> ExecutePayrollReportAsync(int companyId, int year, int month, string periodType, int? fortnight, string? department);
    }
}