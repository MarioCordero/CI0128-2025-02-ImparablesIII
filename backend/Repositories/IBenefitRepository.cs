using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IBenefitRepository
    {
        Task<Benefit> CreateAsync(Benefit benefit);
        Task<Benefit?> GetByIdAsync(int companyId, string name);
        Task<List<Benefit>> GetAllAsync();
        Task<List<Benefit>> GetByCompanyIdAsync(int companyId);
        Task<bool> ExistsAsync(int companyId, string name);
        Task<bool> ExistsByCompanyIdAsync(int companyId);
        Task<List<BenefitResponseDto>> GetBenefitsWithCompanyNameAsync(int companyId);
    }
}
