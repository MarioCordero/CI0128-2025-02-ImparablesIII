using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDirectionRepository _direccionRepository;

        public ProjectService(IProjectRepository projectRepository, IDirectionRepository direccionRepository)
        {
            _projectRepository = projectRepository;
            _direccionRepository = direccionRepository;
        }

        public async Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            // Validate unique constraints
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

            // Create address using ProjectRepository (which delegates to DirectionRepository)
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

            // Create project entity
            var project = new Project
            {
                Nombre = createProjectDto.Nombre.Trim(),
                CedulaJuridica = createProjectDto.CedulaJuridica,
                Email = createProjectDto.Email.Trim().ToLowerInvariant(),
                PeriodoPago = createProjectDto.PeriodoPago,
                Telefono = createProjectDto.Telefono,
                IdDireccion = direccionId,
                MaximoBeneficios = createProjectDto.MaximoBeneficios,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProject = await _projectRepository.CreateAsync(project);
            var direccion = await _direccionRepository.GetDireccionByIdAsync(direccionId);

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
                MaximoBeneficios = p.MaximoBeneficios,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<ProjectResponseDTO?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetProjectWithDireccionAsync(id);
        }

        // Métodos para compatibilidad
        public async Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId)
        {
            // Ignora el employerId ya que no está en tu esquema
            return await CreateProjectAsync(createProjectDto);
        }

        public async Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId)
        {
            // Devuelve todos los proyectos ya que no hay relación con empleador
            return await GetAllProjectsAsync();
        }
        public async Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return new UpdateProjectResult { Success = false, ErrorMessage = "Empresa no encontrada." };

            // Update address first if it exists and dto.Direccion is provided
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

            // Update project using repository method
            await _projectRepository.UpdateAsync(id, dto);

            var updatedProject = await _projectRepository.GetProjectWithDireccionAsync(id);
            
            return new UpdateProjectResult
            {
                Success = true,
                Project = updatedProject
            };
        }

       public async Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId)
        {
            try
            {
                var projects = await _projectRepository.GetProjectsForDashboardAsync(employerId);
                
                foreach (var project in projects)
                {
                    // TODO
                    // project.ActiveEmployees = await GetActiveEmployeesCountAsync(project.Id);
                    // project.MonthlyPayroll = await GetMonthlyPayrollAsync(project.Id);
                    // project.CurrentProfitability = await CalculateCurrentProfitabilityAsync(project.Id);
                    // project.LastMonthProfitability = await CalculateLastMonthProfitabilityAsync(project.Id);
                    // project.Notifications = await GetProjectNotificationsAsync(project.Id);

                    project.ActiveEmployees = 0; // Placeholder si no se implementa
                    project.MonthlyPayroll = 0; // Placeholder si no se implementa
                    project.CurrentProfitability = 0; // Placeholder si no se implementa
                    project.LastMonthProfitability = 0; // Placeholder si no se implementa
                    project.Notifications = new List<NotificationDto>(); // Placeholder si no se implementa
                }
                
                return projects;
            }
            catch (Exception)
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

        public async Task<bool> DeleteProjectAsync(int id)
        {
            return await _projectRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateProjectAsync(int id)
        {
            return await _projectRepository.ActivateAsync(id);
        }

        public async Task<bool> DeactivateProjectAsync(int id)
        {
            return await _projectRepository.DeactivateAsync(id);
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
    }
}