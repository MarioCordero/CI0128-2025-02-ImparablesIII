using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        Task<List<ProjectResponseDTO>> GetAllAsync();
        Task<ProjectResponseDTO?> GetByIdAsync(int id);
        Task<List<ProjectResponseDTO>> GetByEmployerIdAsync(int employerId);
        Task<ProjectResponseDTO> CreateAsync(Project project); // Changed return type
        Task<bool> UpdateAsync(int id, UpdateProjectDTO dto); // Update parameter type
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<DirectionDTO?> GetDireccionByIdAsync(int id);
        Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular);
        Task<bool> UpdateDireccionAsync(int id, DirectionDTO direccion);
        Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId); // Fixed return type
        Task<int> CountActiveEmployeesAsync(int projectId);
        Task<decimal> GetMonthlyPayrollAsync(int projectId);

        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByLegalIdAsync(string legalId); // Cedula Jurídica
        Task<bool> ExistsAsync(int id); // Id
    }
}