using backend.DTOs;

namespace backend.Services
{
    public interface IBenefitService
    {
        Task<BenefitResponseDto> CreateBenefitAsync(CreateBenefitDto createBenefitDto);
        Task<List<BenefitResponseDto>> GetAllBenefitsAsync();
        Task<List<BenefitResponseDto>> GetBenefitsByCompanyIdAsync(int companyId);
        Task<BenefitResponseDto?> GetBenefitByIdAsync(int companyId, string name);
        Task<bool> ExistsBenefitAsync(int companyId, string name);
        Task<UpdateBenefitResponseDto> UpdateBenefitAsync(int companyId, string name, UpdateBenefitRequestDto updateDto);
    }
}
