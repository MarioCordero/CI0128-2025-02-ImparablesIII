using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDirectionRepository _directionRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly ILoginService _loginService;
        private readonly ILogger<ProjectService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public ProjectService(IProjectRepository projectRepository, IDirectionRepository direccionRepository, IEmployeeRepository employeeRepository, IPayrollRepository payrollRepository, ILoginService loginService, ILogger<ProjectService> logger, IUsuarioRepository usuarioRepository)
        {
            _projectRepository = projectRepository;
            _directionRepository = direccionRepository;
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _loginService = loginService;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // CREATE A PROJECT
        public async Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            if (await _projectRepository.ExistsByNameAsync(createProjectDto.Nombre))
            {
                throw new ArgumentException("Ya existe una empresa con este nombre");
            }
            if (await _projectRepository.ExistsByEmailAsync(createProjectDto.Email))
            {
                throw new ArgumentException("Ya existe una empresa con este correo electrónico");
            }
            if (await _projectRepository.ExistsByLegalIdAsync(createProjectDto.CedulaJuridica.ToString()))
            {
                throw new ArgumentException("Ya existe una empresa con esta cédula jurídica");
            }
            int direccionId = await _projectRepository.CreateDireccionAsync(
                createProjectDto.Provincia,
                createProjectDto.Canton,
                createProjectDto.Distrito,
                createProjectDto.DireccionParticular
            );
            if (direccionId <= 0)
            {
                throw new Exception("Error al crear la dirección");
            }
            var project = new Project
            {
                Nombre = createProjectDto.Nombre.Trim(),
                CedulaJuridica = createProjectDto.CedulaJuridica,
                Email = createProjectDto.Email.Trim().ToLowerInvariant(),
                PeriodoPago = createProjectDto.PeriodoPago,
                Telefono = createProjectDto.Telefono,
                IdDireccion = direccionId,
                MaximoBeneficios = createProjectDto.MaximoBeneficios,
                EmployerId = createProjectDto.EmployerId
            };
            var createdProject = await _projectRepository.CreateAsync(project);
            var direccion = await _directionRepository.GetDirectionByIdAsync(direccionId);
            return new ProjectResponseDTO
            {
                Id = createdProject.Id,
                Nombre = createdProject.Nombre,
                CedulaJuridica = createdProject.CedulaJuridica,
                Email = createdProject.Email,
                PeriodoPago = createdProject.PeriodoPago,
                Telefono = createdProject.Telefono,
                IdDireccion = createdProject.IdDireccion,
                Direccion = direccion,
                MaximoBeneficios = createdProject.MaximoBeneficios
            };
        }

        // GET ALL PROJECTS ON THE APP
        public async Task<List<ProjectListDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(p => new ProjectListDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                CedulaJuridica = p.CedulaJuridica,
                Email = p.Email,
                PeriodoPago = p.PeriodoPago,
                MaximoBeneficios = p.MaximoBeneficios
            }).ToList();
        }

        // GET A PROJECT BY ID
        public async Task<ProjectResponseDTO?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetProjectWithDireccionAsync(id);
        }

        // GET A PROJECT'S DIRECTION BY DIRECTION ID
        public async Task<DirectionDTO?> GetProjectDirectionByDirectionId(int id)
        {
            return await _directionRepository.GetDirectionByIdAsync(id);
        }

        // GET A PROJECTS BY EMPLOYER ID
        public async Task<List<ProjectResponseDTO>> GetProjectsByEmployerIdAsync(int employerId)
        {
            return await _projectRepository.GetByEmployerIdAsync(employerId);
        }

        // UPDATE THE INFO OF A PROJECT GIVEN ITS ID
        public async Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return new UpdateProjectResult { Success = false, ErrorMessage = "Empresa no encontrada." };

            if (dto.Direccion != null)
            {
                var direccionModel = new Direccion
                {
                    Id = project.IdDireccion,
                    Provincia = dto.Direccion.Provincia,
                    Canton = dto.Direccion.Canton,
                    Distrito = dto.Direccion.Distrito,
                    DireccionParticular = dto.Direccion.DireccionParticular
                };

                await _projectRepository.UpdateDireccionAsync(project.IdDireccion, dto.Direccion);
            }

            await _projectRepository.UpdateAsync(id, dto);
            var updatedProject = await _projectRepository.GetProjectWithDireccionAsync(id);
            return new UpdateProjectResult
            {
                Success = true,
                Project = updatedProject
            };
        }

        // GENERAL DASHBOARD METHODS
        public async Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId)
        {
            try
            {
                var projects = await _projectRepository.GetProjectsForDashboardAsync(employerId);
                foreach (var project in projects)
                {
                    project.ActiveEmployees = await _projectRepository.CountActiveEmployeesAsync(project.Id);

                    // Use the new payroll method
                    var payrollTotals = await GetMonthlyPayrollAsync(project.Id);
                    project.MonthlyPayroll = payrollTotals?.TotalGross ?? 0; // Or use another property as needed

                    project.CedulaJuridica = project.CedulaJuridica;
                    project.MaximoBeneficios = project.MaximoBeneficios;
                    project.PeriodoPago = project.PeriodoPago;
                }
                return projects;
            }
            catch (Exception ex)
            {
                return new List<ProjectResponseDTO>();
            }
        }

        public async Task<int> GetActiveEmployeesCountAsync(int projectId)
        {
            return await _projectRepository.CountActiveEmployeesAsync(projectId);
        }

        public async Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int id)
        {
            return await _projectRepository.GetProjectWithDireccionAsync(id);
        }

        // DELETE A PROJECT BY ID
        public async Task<bool> DeleteProjectAsync(DeleteProjectRequestDto deleteProjectRequest)
        {
            var user = await _usuarioRepository.GetUserByIdAsync(deleteProjectRequest.UsuarioBajaId);
            if (user == null)
                throw new Exception("Usuario no encontrado.");

            if (!_loginService.VerifyPassword(deleteProjectRequest.Contrasena, user.Contrasena))
                throw new Exception("Contraseña incorrecta.");

            if (!await _projectRepository.ExistsAsync(deleteProjectRequest.ProjectId))
                throw new Exception("Error al eliminar la empresa porque no existe.");

            if (await _projectRepository.CountActiveEmployeesAsync(deleteProjectRequest.ProjectId) > 0)
                throw new Exception("Error al eliminar la empresa porque tiene empleados activos.");

            var payrolls = await _payrollRepository.GetPayrollHistoryByCompanyAsync(deleteProjectRequest.ProjectId);
            if (payrolls != null && payrolls.Any())
            {
                // LOGICAL DELETE
                return await _projectRepository.LogicalDeleteAsync(deleteProjectRequest);
            }
            // PHYSICAL DELETE
            return await _projectRepository.PhysicalDeleteAsync(deleteProjectRequest.ProjectId);
        }

        public async Task<bool> ExistsByLegalIdAsync(string legalId)
        {
            return await _projectRepository.ExistsByLegalIdAsync(legalId);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _projectRepository.ExistsByEmailAsync(email);
        }

        public async Task<bool> ProjectExistsAsync(int id)
        {
            return await _projectRepository.ExistsAsync(id);
        }

        // DASHBOARD FOR THE PROJECT (INDIVIDUALLY) DASHBOARD METRICS
        public async Task<DashboardMetricsDTO?> GetDashboardMetricsAsync(int projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    throw new ArgumentException("El ID del proyecto debe ser válido", nameof(projectId));
                }
                var projectExists = await _projectRepository.ExistsAsync(projectId);
                if (!projectExists)
                {
                    return null;
                }

                var employees = await _employeeRepository.GetEmployeesByCompanyAsync(projectId);
                var activeEmployees = employees.Where(e => e.Estado == "Activo").ToList();
                var currentPayroll = activeEmployees.Sum(e => e.Salario ?? 0);
                var activeDepartments = activeEmployees
                    .Where(e => !string.IsNullOrEmpty(e.Departamento))
                    .Select(e => e.Departamento)
                    .Distinct()
                    .Count();
                var notifications = 0; // TODO
                var metrics = new DashboardMetricsDTO
                {
                    TotalEmployees = activeEmployees.Count,
                    CurrentPayroll = currentPayroll,
                    ActiveDepartments = activeDepartments,
                    Notifications = notifications,
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo métricas del dashboard para el proyecto {ProjectId}", projectId);
                throw;
            }
        }

        // HELPER METHOD TO GET DEPARTMENT STATS
        public async Task<List<DepartmentStatsDTO>> GetDepartmentStatsAsync(int projectId)
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesByCompanyAsync(projectId);
                var activeEmployees = employees.Where(e => e.Estado == "Activo").ToList();

                var departmentStats = activeEmployees
                    .GroupBy(e => e.Departamento ?? "Sin Departamento")
                    .Select(g => new DepartmentStatsDTO
                    {
                        DepartmentName = g.Key,
                        EmployeeCount = g.Count(),
                        TotalSalary = g.Sum(e => e.Salario ?? 0)
                    })
                    .OrderByDescending(d => d.EmployeeCount)
                    .ToList();

                return departmentStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de departamentos para el proyecto {ProjectId}", projectId);
                throw;
            }
        }

        // HELPER METHOD TO GET MONTHLY PAYROLL
        public async Task<PayrollTotalsDto?> GetMonthlyPayrollAsync(int projectId)
        {
            return await _payrollRepository.GetLatestPayrollTotalsByCompanyAsync(projectId);
        }
    }
}