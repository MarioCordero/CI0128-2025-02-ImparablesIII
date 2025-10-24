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
    }
}