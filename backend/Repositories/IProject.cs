using backend.Models;

namespace backend.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateAsync(Project project);
        Task<Project?> GetByIdAsync(int id);
        Task<Project?> GetByNameAsync(string projectName);
        Task<List<Project>> GetByCompanyIdAsync(int companyId);
        Task<bool> ExistsByNameAsync(string projectName);
        Task<Project?> GetByEmailAsync(string email);
        Task<List<Project>> GetByEmployerIdAsync(int employerId);
        Task<bool> ExistsByLegalIdAsync(string legalId);
        Task<bool> ExistsByNameAsync(string companyName);
        Task<bool> ExistsByEmailAsync(string email);
    }
}