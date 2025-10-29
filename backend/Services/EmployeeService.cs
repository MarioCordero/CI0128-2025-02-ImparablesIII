using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _logger = logger;
        }

        public async Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto)
        {
            _logger.LogInformation("Registering new employee");
            return await _employeeRepository.RegisterEmployeeAsync(employeeDto);
        }

        public async Task<bool> ValidateCedulaExistsAsync(string cedula)
        {
            _logger.LogInformation("Validating cedula: {Cedula}", cedula);
            return await _employeeRepository.ValidateCedulaExistsAsync(cedula);
        }

        public async Task<bool> ValidateEmailExistsAsync(string email)
        {
            _logger.LogInformation("Validating email: {Email}", email);
            return await _employeeRepository.ValidateEmailExistsAsync(email);
        }

        public async Task<Empleado?> GetEmployeeByIdAsync(int employeeId)
        {
            _logger.LogInformation("Getting employee by ID: {EmployeeId}", employeeId);

            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employee ID: {EmployeeId}", employeeId);
                throw new ArgumentException("Employee ID must be greater than 0");
            }

            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task<int?> GetEmployeeCompanyIdAsync(int employeeId)
        {
            _logger.LogInformation("Getting company ID for employee: {EmployeeId}", employeeId);

            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employee ID: {EmployeeId}", employeeId);
                throw new ArgumentException("Employee ID must be greater than 0");
            }

            return await _employeeRepository.GetEmployeeCompanyIdAsync(employeeId);
        }

        public async Task<Project?> GetEmployeeCompanyAsync(int employeeId)
        {
            _logger.LogInformation("Getting company for employee: {EmployeeId}", employeeId);

            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employee ID: {EmployeeId}", employeeId);
                throw new ArgumentException("Employee ID must be greater than 0");
            }

            var companyId = await GetEmployeeCompanyIdAsync(employeeId);

            if (!companyId.HasValue)
            {
                _logger.LogWarning("No company found for employee: {EmployeeId}", employeeId);
                return null;
            }

            return await _projectRepository.GetByIdAsync(companyId.Value);
        }

        public async Task<EmployeeListResponseDto> GetEmployeesByCompanyAsync(int companyId)
        {
            _logger.LogInformation("Obteniendo empleados para la empresa {CompanyId}", companyId);

            if (companyId <= 0)
            {
                _logger.LogWarning("ID de empresa inválido: {CompanyId}", companyId);
                throw new ArgumentException("El ID de la empresa debe ser mayor a 0");
            }

            var employees = await _employeeRepository.GetEmployeesByCompanyAsync(companyId);
            
            _logger.LogInformation("Se encontraron {Count} empleados para la empresa {CompanyId}", employees.Count, companyId);
            
            return new EmployeeListResponseDto
            {
                Employees = employees,
                TotalCount = employees.Count
            };
        }
    }
}

