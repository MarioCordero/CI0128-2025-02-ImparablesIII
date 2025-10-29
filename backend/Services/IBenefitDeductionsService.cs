using backend.DTOs;

namespace backend.Services
{
    public interface IBenefitDeductionsService
    {
        Task<BenefitDeductionCalculationDto> CalculateBenefitDeductionsAsync(int userId, int projectId);
    }
}
