using backend.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);
        Task<UpdateProjectResult> UpdateProjectAsync(int id, UpdateProjectDTO dto);
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId);
        Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId);
    }
}