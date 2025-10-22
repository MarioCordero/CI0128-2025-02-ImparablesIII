using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IBenefitRepository
    {
        // Basic CRUD
        Task<Benefit> CreateAsync(Benefit benefit);
        Task<Benefit?> GetByIdAsync(int companyId, string name);
        Task<List<Benefit>> GetAllAsync();
        Task<List<Benefit>> GetByCompanyIdAsync(int companyId);
        Task<bool> UpdateAsync(int companyId, string name, UpdateBenefitDto updateDto);
        Task<bool> DeleteAsync(int companyId, string name);

        // Existence validations
        Task<bool> ExistsAsync(int companyId, string name);
        Task<bool> ExistsByCompanyIdAsync(int companyId);

        // Specific queries
        Task<List<BenefitResponseDto>> GetBenefitsWithCompanyNameAsync(int companyId);
        Task<int> CountByCompanyIdAsync(int companyId);
    }
}
