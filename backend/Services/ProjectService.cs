using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId)
        {
            // Validate unique constraints
            if (await _projectRepository.ExistsByNameAsync(createProjectDto.ProjectName))
            {
                throw new ArgumentException("Ya existe un proyecto con este nombre");
            }

            if (await _projectRepository.ExistsByDescriptionAsync(createProjectDto.Description))
            {
                throw new ArgumentException("Ya existe un proyecto con esta descripción");
            }

            if (await _projectRepository.ExistsByEmailAsync(createProjectDto.Email))
            {
                throw new ArgumentException("Ya existe un proyecto con este correo electrónico");
            }

            // Create project entity
            var project = new Project
            {
                ProjectName = createProjectDto.ProjectName.Trim(),
                Description = createProjectDto.Description.Trim(),
                Email = createProjectDto.Email.Trim().ToLowerInvariant(),
                Address = createProjectDto.Address?.Trim(),
                Phone = createProjectDto.Phone?.Trim(),
                MaxBenefits = createProjectDto.MaxBenefits,
                PaymentPeriod = createProjectDto.PaymentPeriod,
                EmployerId = employerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdCompany = await _companyRepository.CreateAsync(company);

            return new CompanyResponseDto
            {
                Id = createdCompany.Id,
                CompanyName = createdCompany.CompanyName,
                LegalId = createdCompany.LegalId,
                Email = createdCompany.Email,
                Address = createdCompany.Address,
                Phone = createdCompany.Phone,
                MaxBenefits = createdCompany.MaxBenefits,
                PaymentPeriod = createdCompany.PaymentPeriod,
                CreatedAt = createdCompany.CreatedAt
            };
        }

        public async Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId)
        {
            var projects = await _projectRepository.GetByEmployerIdAsync(employerId);

            return projects.Select(p => new ProjectListDto
            {
                Id = p.Id,
                ProjectName = p.ProjectName,
                Description = p.Description,
                Email = p.Email,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                return null;

            return new ProjectResponseDto
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                Email = project.Email,
                Address = project.Address,
                Phone = project.Phone,
                MaxBenefits = project.MaxBenefits,
                PaymentPeriod = project.PaymentPeriod,
                CreatedAt = project.CreatedAt
            };
        }
    }
}