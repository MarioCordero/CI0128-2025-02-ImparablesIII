using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDireccionRepository _direccionRepository;

        public ProjectService(IProjectRepository projectRepository, IDireccionRepository direccionRepository)
        {
            _projectRepository = projectRepository;
            _direccionRepository = direccionRepository;
        }

        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto)
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

            if (await _projectRepository.ExistsByCedulaJuridicaAsync(createProjectDto.CedulaJuridica))
            {
                throw new ArgumentException("Ya existe una empresa con esta cédula jurídica");
            }

            // Create address using ProjectRepository (which delegates to DireccionRepository)
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

            return new ProjectResponseDto
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

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetProjectWithDireccionAsync(id);
        }

        // Métodos para compatibilidad
        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId)
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

            project.Nombre = dto.Nombre.Trim();
            project.CedulaJuridica = dto.CedulaJuridica;
            project.Email = dto.Email.Trim();
            project.Telefono = dto.Telefono;
            project.PeriodoPago = dto.PeriodoPago;
            project.MaximoBeneficios = dto.MaximoBeneficios;

            // Recupera la dirección como DTO
            var direccionDto = await _direccionRepository.GetDireccionByIdAsync(project.IdDireccion);

            // Convierte el DTO a modelo para actualizar
            var direccionModel = direccionDto != null
                ? new Direccion
                {
                    Id = direccionDto.Id,
                    Provincia = dto.Direccion.Provincia,
                    Canton = dto.Direccion.Canton,
                    Distrito = dto.Direccion.Distrito,
                    DireccionParticular = dto.Direccion.DireccionParticular
                }
                : new Direccion
                {
                    Provincia = dto.Direccion.Provincia,
                    Canton = dto.Direccion.Canton,
                    Distrito = dto.Direccion.Distrito,
                    DireccionParticular = dto.Direccion.DireccionParticular
                };

            await _direccionRepository.UpdateDireccionAsync(direccionModel);
            await _projectRepository.UpdateAsync(id, dto);

            var updatedProject = await _projectRepository.GetProjectWithDireccionAsync(id);
            
            return new UpdateProjectResult
            {
                Success = true,
                Project = updatedProject
            };
        }
    }
}