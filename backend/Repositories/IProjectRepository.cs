using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        Task<List<ProjectResponseDto>> GetAllAsync();
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task<List<ProjectResponseDto>> GetByEmployerIdAsync(int employerId);
        Task<ProjectResponseDto> CreateAsync(Project project); // Changed return type
        Task<bool> UpdateAsync(int id, UpdateProjectDTO dto); // Update parameter type
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<DirectionDTO?> GetDireccionByIdAsync(int id);
        Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular);
        Task<bool> UpdateDireccionAsync(int id, DirectionDTO direccion);
        Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<List<ProjectResponseDto>> GetProjectsForDashboardAsync(int employerId); // Fixed return type
        Task<int> CountActiveEmployeesAsync(int projectId);
        Task<decimal> GetMonthlyPayrollAsync(int projectId);

        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByLegalIdAsync(string legalId); // Cedula Jurídica
        Task<bool> ExistsAsync(int id); // Id
    }
}