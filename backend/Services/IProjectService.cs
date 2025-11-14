using backend.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        // Métodos existentes - mantener tal como están
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);
        Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto);
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId);
        Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId);

        // Nuevos métodos para consolidar funcionalidad del dashboard
        Task<List<ProjectResponseDto>> GetProjectsForDashboardAsync(int employerId);
        Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id);
        Task<bool> DeleteProjectAsync(int id);
        Task<bool> ActivateProjectAsync(int id);
        Task<bool> DeactivateProjectAsync(int id);
        
        // Métodos para validación
        Task<bool> ExistsByLegalIdAsync(string legalId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ProjectExistsAsync(int id);
    }
}