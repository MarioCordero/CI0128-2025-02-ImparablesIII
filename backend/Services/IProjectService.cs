using backend.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectResponseDTO?> GetProjectByIdAsync(int id);
        Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto);
        Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId);
        Task<int> GetActiveEmployeesCountAsync(int projectId);
        Task<List<ProjectResponseDTO>> GetProjectsByEmployerIdAsync(int employerId);


        // Nuevos métodos para consolidar funcionalidad del dashboard
        Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId);
        Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int id);
        Task<bool> DeleteProjectAsync(int id);
        
        // Métodos para validación
        Task<bool> ExistsByLegalIdAsync(string legalId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ProjectExistsAsync(int id);

        // DASHBOARD METHODS
        Task<List<DepartmentStatsDTO>> GetDepartmentStatsAsync(int projectId);
        Task<DashboardMetricsDTO?> GetDashboardMetricsAsync(int projectId);
        Task<DirectionDTO?> GetProjectDirectionByDirectionId(int id);
    }
}