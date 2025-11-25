using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository,
            IUsuarioRepository usuarioRepository,
            IEmailHelper emailHelper,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _usuarioRepository = usuarioRepository;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        public async Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto)
        {
            try
            {
                var employeeId = await _employeeRepository.RegisterEmployeeAsync(employeeDto);   
                if (employeeId <= 0)
                {
                    _logger.LogError("Error creating employee");
                    return 0;
                }

                _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", employeeId);

                var rawToken = _emailHelper.GenerateVerificationToken();
                var hash = _emailHelper.HashToken(rawToken);
                var usuario = new Usuario
                {
                    IdPersona = employeeId,
                    TipoUsuario = "Empleado",
                    Contrasena = null,
                    VerificationTokenHash = hash,
                    VerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                    IsVerified = false
                };

                _logger.LogInformation("Attempting to create user for Employee {EmployeeId} with token hash", employeeId);

                try
                {
                    var userCreated = await _usuarioRepository.CreateUserAsync(usuario);
                    if (!userCreated)
                    {
                        _logger.LogError("CreateUserAsync returned false for Employee {EmployeeId}", employeeId);
                        return 0;
                    }
                    
                    _logger.LogInformation("User created successfully for Employee {EmployeeId}", employeeId);
                }
                catch (Exception userEx)
                {
                    _logger.LogError(userEx, "Exception occurred while creating user for Employee {EmployeeId}: {Message}", employeeId, userEx.Message);
                    return 0;
                }

                await _emailHelper.SendVerificationLinkAsync(employeeDto.Correo, rawToken, "Empleado");                
                _logger.LogInformation("Employee registration completed successfully for Employee {EmployeeId}", employeeId);
                return employeeId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering employee: {Message}", ex.Message);
                return 0;
            }
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

        public async Task<ProjectResponseDTO?> GetEmployeeCompanyAsync(int employeeId)
        {
            _logger.LogInformation("Getting whole company for employee: {EmployeeId}", employeeId);

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

