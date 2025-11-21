using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IEmployeeRepository
    {
        Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto);
        Task<bool> ValidateCedulaExistsAsync(string cedula);
        Task<bool> ValidateEmailExistsAsync(string email);
        Task<bool> TestConnectionAsync();
        Task<Empleado?> GetEmployeeByIdAsync(int employeeId);
        Task<int?> GetEmployeeCompanyIdAsync(int employeeId);
        Task<int> GetEmployeeAgeAsync(int employeeId);
        Task<List<EmployeeListDto>> GetEmployeesByCompanyAsync(int companyId);
        Task<bool> HasPayrollRecordsAsync(int employeeId);
        Task<int> GetPayrollRecordsCountAsync(int employeeId);
        Task<bool> DeleteEmployeeLogicallyAsync(int employeeId, int deletedByUserId, string motivoBaja);
        Task<bool> DeleteEmployeePhysicallyAsync(int employeeId);
        Task<bool> IsEmployeeDeletedAsync(int employeeId);
        Task<EmployeeDeletionInfoDto> GetEmployeeDeletionInfoAsync(int employeeId);
    }
}