using backend.Models;

namespace backend.Services
{
    public interface IEmployeeService
    {
        Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto);
        Task<bool> ValidateCedulaExistsAsync(string cedula);
        Task<bool> ValidateEmailExistsAsync(string email);
        Task<Empleado?> GetEmployeeByIdAsync(int employeeId);
        Task<int?> GetEmployeeCompanyIdAsync(int employeeId);
        Task<Project?> GetEmployeeCompanyAsync(int employeeId);
    }
}

