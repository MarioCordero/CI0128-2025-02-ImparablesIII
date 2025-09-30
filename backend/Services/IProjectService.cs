using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int employerId);
        Task<List<ProjectListDto>> GetProjectsByEmployerAsync(int employerId);
        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);
    }
}