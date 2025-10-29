using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        // CRUD básico
        Task<Project> CreateAsync(Project project);
        Task<Project?> GetByIdAsync(int id);
        Task<Project?> GetByNameAsync(string nombre);
        Task<Project?> GetByEmailAsync(string email);
        Task<List<Project>> GetAllAsync();
        Task<bool> UpdateAsync(int id, UpdateProjectDTO dto);
        Task<bool> DeleteAsync(int id);

        // Validaciones de existencia
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByCedulaJuridicaAsync(int cedulaJuridica);
        Task<bool> ExistsByLegalIdAsync(string legalId);

        // Consultas específicas
        Task<List<Project>> GetByEmployerIdAsync(int employerId);
        Task<List<Project>> GetByCompanyIdAsync(int companyId);
        Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id);
        Task<List<CompanyDashboardMainEmployerDto>> GetProjectsForDashboardAsync(int employerId);

        // Operaciones de negocio
        Task<int> CountActiveEmployeesAsync(int projectId);
        Task<decimal> GetMonthlyPayrollAsync(int projectId);
        Task<bool> ActivateAsync(int projectId);
        Task<bool> DeactivateAsync(int projectId);

        // Dirección (delegadas)
        Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular);
        Task<DireccionDto?> GetDireccionByIdAsync(int id);

    }
}