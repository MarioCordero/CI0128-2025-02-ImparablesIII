using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateAsync(Project project);
        Task<Project?> GetByIdAsync(int id);
        Task<Project?> GetByNameAsync(string nombre);
        Task<Project?> GetByEmailAsync(string email);
        Task<List<Project>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByCedulaJuridicaAsync(int cedulaJuridica);
        Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular);
        Task<DireccionDto?> GetDireccionByIdAsync(int id);
        Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id);
        
        // Métodos para compatibilidad con código existente
        Task<List<Project>> GetByEmployerIdAsync(int employerId);
        Task<bool> ExistsByLegalIdAsync(string legalId);
        Task<List<Project>> GetByCompanyIdAsync(int companyId);
    }
}