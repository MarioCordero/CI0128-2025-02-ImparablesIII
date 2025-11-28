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
        // NEW
        Task<List<EmployeeDeductionDto>> GetEmployeeDeductionsAsync();
        Task<List<EmployerDeductionDto>> GetEmployerDeductionsAsync();
        Task<List<EmployeePayrollDto>> GetEmployeesForPayrollAsync(int companyId);
        Task<List<EmployeePayrollDto>> GetEmployeesForPayrollIdAsync(int payrollId);
        
        // Payroll Insert Methods
        Task<int> InsertPayrollAsync(PayrollInsertDto payroll);
        Task InsertPayrollDetailsAsync(int payrollId, List<PayrollDetailInsertDto> details);
        Task InsertPayrollBenefitsAsync(int payrollId, int companyId, IEnumerable<string> benefitNames);

        // Payroll summary
        Task<PayrollTotalsDto?> GetLatestPayrollTotalsByCompanyAsync(int companyId);
        Task<bool> ExistsPayrollForMonthAsync(int companyId, int year, int month);
        Task<bool> ExistsPayrollForFortnightAsync(int companyId, int year, int month, int fortnight);
        Task<List<PayrollHistoryItemDto>> GetPayrollHistoryByCompanyAsync(int companyId);
        Task<Dictionary<int, string>> GetEmployeePositionsByCompanyAsync(int companyId);
        Task<List<EmployeePayrollReportDto>> GetEmployeePayrollReportsAsync(int employeeId, int? year, int? month, string? puesto);
        Task<DetailedPayrollReportDto?> GetDetailedPayrollReportAsync(int employeeId, int payrollId);
        Task<(string NombreCompleto, string TipoContrato)> GetEmployeeBasicInfoAsync(int employeeId);
        Task<int?> GetCompanyIdFromPayrollAsync(int payrollId);
        Task<HistoricalPayrollReportDto> GetHistoricalPayrollReportAsync(int employeeId, DateTime? startDate, DateTime? endDate);
    }
}