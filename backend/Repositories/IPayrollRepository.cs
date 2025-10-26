using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IPayrollRepository
    {
        Task<IEnumerable<Payroll>> GetPayrollByPeriodAsync(DateTime period, string periodType);
        Task<IEnumerable<Payroll>> GetPayrollByFiltersAsync(PayrollFiltersDto filters);
        Task<Payroll> GetPayrollByIdAsync(int payrollId);
        Task<Payroll> CreatePayrollAsync(Payroll payroll);
        Task<Payroll> UpdatePayrollAsync(Payroll payroll);
        Task<bool> DeletePayrollAsync(int payrollId);
        Task<IEnumerable<PayrollDetailDto>> GetPayrollDetailsAsync(PayrollFiltersDto filters);
        Task<Payroll> GetEmployeePayrollAsync(int employeeId, DateTime period, string periodType);
        Task<bool> IsPayrollProcessedAsync(int employeeId, DateTime period, string periodType);
    }
}