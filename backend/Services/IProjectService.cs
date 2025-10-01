using backend.DTOs;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<List<ProjectListDto>> GetAllProjectsAsync();
        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);
        
        // Métodos para compatibilidad con código existente
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId);
        Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId);
    }
}