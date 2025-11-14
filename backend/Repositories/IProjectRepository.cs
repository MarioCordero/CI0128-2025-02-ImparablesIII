using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        Task<List<ProjectResponseDTO>> GetByEmployerIdAsync(int employerId);
        Task<ProjectResponseDTO?> GetByIdAsync(int id);
        Task<List<ProjectResponseDTO>> GetAllAsync();
        Task<DirectionDTO?> GetDireccionByIdAsync(int id);
        Task<decimal> GetMonthlyPayrollAsync(int projectId);
        Task<int> CountActiveEmployeesAsync(int projectId);
        Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int id);
        Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId);

        Task<ProjectResponseDTO> CreateAsync(Project project);
        Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular);

        Task<bool> UpdateAsync(int id, UpdateProjectDTO dto);
        Task<bool> UpdateDireccionAsync(int id, DirectionDTO direccion);

        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByLegalIdAsync(string legalId); // Cedula Jurídica
        Task<bool> ExistsAsync(int id); // Id
    }
}