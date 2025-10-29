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
    }
}