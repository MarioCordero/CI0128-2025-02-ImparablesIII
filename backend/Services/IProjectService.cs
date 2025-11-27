using backend.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDTO> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectResponseDTO?> GetProjectByIdAsync(int id);
        Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto);
        Task<int> GetActiveEmployeesCountAsync(int projectId);
        Task<List<ProjectResponseDTO>> GetProjectsByEmployerIdAsync(int employerId);
        Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId);
        Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int id);
        Task<bool> ExistsByLegalIdAsync(string legalId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ProjectExistsAsync(int id);
        Task<bool> DeleteProjectAsync(DeleteProjectRequestDto deleteProjectRequest);
        // DASHBOARD METHODS
        Task<List<DepartmentStatsDTO>> GetDepartmentStatsAsync(int projectId);
        Task<DashboardMetricsDTO?> GetDashboardMetricsAsync(int projectId);
        Task<DirectionDTO?> GetProjectDirectionByDirectionId(int id);
        Task<PayrollTotalsDto?> GetMonthlyPayrollAsync(int projectId);
    }
}