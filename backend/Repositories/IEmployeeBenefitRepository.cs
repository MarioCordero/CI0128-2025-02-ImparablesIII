using backend.DTOs;

namespace backend.Repositories
{
    public interface IEmployeeBenefitRepository
    {
        Task<List<EmployeeBenefitDto>> GetAvailableBenefitsForEmployeeAsync(int employeeId, int companyId, BenefitFilterDto? filter = null);
        Task<List<EmployeeBenefitDto>> GetSelectedBenefitsForEmployeeAsync(int employeeId, int companyId);
        Task<EmployeeBenefitsSummaryDto> GetEmployeeBenefitsSummaryAsync(int employeeId, int companyId, BenefitFilterDto? filter = null);
        Task<bool> IsBenefitSelectedAsync(int employeeId, int companyId, string benefitName);
        Task<int> GetSelectedBenefitsCountAsync(int employeeId, int companyId);
        Task<(bool Success, string Message)> AddBenefitToEmployeeAsync(int employeeId, int companyId, string benefitName, string benefitType, int? NumDependents = null, string? PensionType = null);
        Task<int> GetMaxBenefitLimitAsync(int companyId);
        Task<int> GetTotalEmployeeCountAsync(int companyId);
    }
}

