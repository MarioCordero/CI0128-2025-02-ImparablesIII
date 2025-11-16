using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IEmployeeService
    {
        Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto);
        Task<bool> ValidateCedulaExistsAsync(string cedula);
        Task<bool> ValidateEmailExistsAsync(string email);
        Task<Empleado?> GetEmployeeByIdAsync(int employeeId);
        Task<int?> GetEmployeeCompanyIdAsync(int employeeId); // Id empresa
        Task<ProjectResponseDTO?> GetEmployeeCompanyAsync(int employeeId); // Empresa completa
        Task<EmployeeListResponseDto> GetEmployeesByCompanyAsync(int companyId);
    }
}

