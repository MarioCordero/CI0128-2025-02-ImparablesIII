using backend.DTOs;
using backend.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace backend.Services
{
    public class EmployeeBenefitService : IEmployeeBenefitService
    {
        private readonly IEmployeeBenefitRepository _employeeBenefitRepository;
        private readonly IBenefitRepository _benefitRepository;
        private readonly ILogger<EmployeeBenefitService> _logger;

        public EmployeeBenefitService(
            IEmployeeBenefitRepository employeeBenefitRepository,
            IBenefitRepository benefitRepository,
            ILogger<EmployeeBenefitService> logger)
        {
            _employeeBenefitRepository = employeeBenefitRepository;
            _benefitRepository = benefitRepository;
            _logger = logger;
        }

        public async Task<EmployeeBenefitsSummaryDto> GetEmployeeBenefitsAsync(int employeeId, int companyId, BenefitFilterDto? filter = null)
        {
            try
            {
                _logger.LogInformation("Getting benefits for employee {EmployeeId} in company {CompanyId}", employeeId, companyId);

                // Use the optimized stored procedure method instead of multiple calls
                var summary = await _employeeBenefitRepository.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, filter);
                await RemoveDeletedBenefitsFromSummaryAsync(companyId, summary);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting benefits for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<EmployeeBenefitSelectionResponseDto> SelectBenefitAsync(int employeeId, int companyId, SelectBenefitRequestDto request)
        {
            try
            {
                _logger.LogInformation("Selecting benefit {BenefitName} for employee {EmployeeId}", request.BenefitName, employeeId);


                var validationResult = await ValidateBenefitSelectionRequestAsync(employeeId, companyId, request);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var benefit = await GetBenefitAsync(companyId, request.BenefitName);
                var (success, message) = await AddBenefitAsync(employeeId, companyId, request.BenefitName, benefit.CalculationType, request.NumDependents, request.PensionType);

                if (!success)
                {
                    return new EmployeeBenefitSelectionResponseDto
                    {
                        Success = false,
                        Message = message,
                        CurrentSelections = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId),
                        MaxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId)
                    };
                }

                return await CreateSuccessResponseAsync(employeeId, companyId, "Beneficio agregado exitosamente");
            }
            catch (Exception ex)
            {
                return HandleSelectionErrorAsync(ex, employeeId);
            }
        }

        public async Task<EmployeeBenefitSelectionResponseDto> DeselectBenefitAsync(int employeeId, int companyId, string benefitName)
        {
            try
            {
                _logger.LogInformation("Removing benefit {BenefitName} for employee {EmployeeId}", benefitName, employeeId);

                var isSelected = await _employeeBenefitRepository.IsBenefitSelectedAsync(employeeId, companyId, benefitName);
                if (!isSelected)
                {
                    return CreateErrorResponse("El beneficio no está agregado");
                }

                var (success, message) = await _employeeBenefitRepository.RemoveBenefitFromEmployeeAsync(employeeId, companyId, benefitName);
                if (!success)
                {
                    return new EmployeeBenefitSelectionResponseDto
                    {
                        Success = false,
                        Message = message,
                        CurrentSelections = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId),
                        MaxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId)
                    };
                }

                return await CreateSuccessResponseAsync(employeeId, companyId, "Beneficio eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing benefit for employee {EmployeeId}", employeeId);
                return new EmployeeBenefitSelectionResponseDto
                {
                    Success = false,
                    Message = $"Error al eliminar el beneficio: {ex.Message}"
                };
            }
        }

    

        public async Task<bool> ValidateBenefitSelectionAsync(int employeeId, int companyId)
        {
            try
            {
                var currentSelections = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId);
                var maxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId);

                return currentSelections < maxSelections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating benefit selection for employee {EmployeeId}", employeeId);
                return false;
            }
        }

        private async Task<EmployeeBenefitSelectionResponseDto?> ValidateBenefitSelectionRequestAsync(int employeeId, int companyId, SelectBenefitRequestDto request)
        {
            var benefit = await _benefitRepository.GetByIdAsync(companyId, request.BenefitName);
            if (benefit == null)
            {
                return CreateErrorResponse("El beneficio no existe");
            }

            if (benefit.IsDeleted)
            {
                return CreateErrorResponse("El beneficio ya no está disponible");
            }

            var isSelected = await _employeeBenefitRepository.IsBenefitSelectedAsync(employeeId, companyId, request.BenefitName);
            if (isSelected)
            {
                return CreateErrorResponse("El beneficio ya está agregado");
            }

            var currentSelections = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId);
            var maxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId);

            if (currentSelections >= maxSelections)
            {
                return new EmployeeBenefitSelectionResponseDto
                {
                    Success = false,
                    Message = $"Ha alcanzado el límite máximo de {maxSelections} beneficios",
                    CurrentSelections = currentSelections,
                    MaxSelections = maxSelections
                };
            }

            return null;
        }

        private async Task<Models.Benefit> GetBenefitAsync(int companyId, string benefitName)
        {
            var benefit = await _benefitRepository.GetByIdAsync(companyId, benefitName);
            if (benefit == null)
            {
                throw new InvalidOperationException($"Benefit {benefitName} not found");
            }
            if (benefit.IsDeleted)
            {
                throw new InvalidOperationException($"Benefit {benefitName} is no longer available");
            }
            return benefit;
        }

        private async Task<(bool Success, string Message)> AddBenefitAsync(int employeeId, int companyId, string benefitName, string benefitType, int? NumDependents = null, string? PensionType = null)
        {
            return await _employeeBenefitRepository.AddBenefitToEmployeeAsync(employeeId, companyId, benefitName, benefitType, NumDependents, PensionType);
        }

        private async Task RemoveDeletedBenefitsFromSummaryAsync(int companyId, EmployeeBenefitsSummaryDto summary)
        {
            var activeBenefits = await _benefitRepository.GetByCompanyIdAsync(companyId);
            if (activeBenefits == null || activeBenefits.Count == 0)
            {
                summary.AvailableBenefits = new List<EmployeeBenefitDto>();
                summary.SelectedBenefits = new List<EmployeeBenefitDto>();
                return;
            }

            var activeBenefitNames = new HashSet<string>(activeBenefits.Select(b => b.Name), StringComparer.OrdinalIgnoreCase);

            var filteredAvailable = summary.AvailableBenefits
                .Where(b => activeBenefitNames.Contains(b.BenefitName))
                .ToList();

            var filteredSelected = summary.SelectedBenefits
                .Where(b => activeBenefitNames.Contains(b.BenefitName))
                .ToList();

            if (filteredAvailable.Count != summary.AvailableBenefits.Count || filteredSelected.Count != summary.SelectedBenefits.Count)
            {
                _logger.LogInformation("Filtered logically deleted benefits for company {CompanyId}", companyId);
            }

            summary.AvailableBenefits = filteredAvailable;
            summary.SelectedBenefits = filteredSelected;
        }

        private async Task<EmployeeBenefitSelectionResponseDto> CreateSuccessResponseAsync(int employeeId, int companyId, string message)
        {
            var newCount = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId);
            var maxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId);

            return new EmployeeBenefitSelectionResponseDto
            {
                Success = true,
                Message = message,
                CurrentSelections = newCount,
                MaxSelections = maxSelections
            };
        }

        private EmployeeBenefitSelectionResponseDto CreateErrorResponse(string message)
        {
            return new EmployeeBenefitSelectionResponseDto
            {
                Success = false,
                Message = message
            };
        }

        private EmployeeBenefitSelectionResponseDto HandleSelectionErrorAsync(Exception ex, int employeeId)
        {
            _logger.LogError(ex, "Error selecting benefit for employee {EmployeeId}", employeeId);
            return new EmployeeBenefitSelectionResponseDto
            {
                Success = false,
                Message = $"Error al seleccionar el beneficio: {ex.Message}"
            };
        }
    }
}

