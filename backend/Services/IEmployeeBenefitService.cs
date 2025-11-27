using backend.DTOs;

namespace backend.Services
{
    public interface IEmployeeBenefitService
    {
        Task<EmployeeBenefitsSummaryDto> GetEmployeeBenefitsAsync(int employeeId, int companyId, BenefitFilterDto? filter = null);
        Task<EmployeeBenefitSelectionResponseDto> SelectBenefitAsync(int employeeId, int companyId, SelectBenefitRequestDto request);
        Task<EmployeeBenefitSelectionResponseDto> DeselectBenefitAsync(int employeeId, int companyId, string benefitName);
        Task<bool> ValidateBenefitSelectionAsync(int employeeId, int companyId);
    }
}

