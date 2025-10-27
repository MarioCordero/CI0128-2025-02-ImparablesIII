using backend.DTOs;
using backend.Repositories;

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

                var availableBenefits = await _employeeBenefitRepository.GetAvailableBenefitsForEmployeeAsync(employeeId, companyId, filter);
                var selectedBenefits = await _employeeBenefitRepository.GetSelectedBenefitsForEmployeeAsync(employeeId, companyId);
                var maxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId);
                var currentSelections = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId);

                return new EmployeeBenefitsSummaryDto
                {
                    AvailableBenefits = availableBenefits,
                    SelectedBenefits = selectedBenefits,
                    CurrentSelections = currentSelections,
                    MaxSelections = maxSelections
                };
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
                var (success, message) = await AddBenefitAsync(employeeId, companyId, request.BenefitName, benefit.CalculationType);

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

                return await CreateSuccessResponseAsync(employeeId, companyId);
            }
            catch (Exception ex)
            {
                return HandleSelectionErrorAsync(ex, employeeId);
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
            return benefit;
        }

        private async Task<(bool Success, string Message)> AddBenefitAsync(int employeeId, int companyId, string benefitName, string benefitType)
        {
            return await _employeeBenefitRepository.AddBenefitToEmployeeAsync(employeeId, companyId, benefitName, benefitType);
        }

        private async Task<EmployeeBenefitSelectionResponseDto> CreateSuccessResponseAsync(int employeeId, int companyId)
        {
            var newCount = await _employeeBenefitRepository.GetSelectedBenefitsCountAsync(employeeId, companyId);
            var maxSelections = await _employeeBenefitRepository.GetMaxBenefitLimitAsync(companyId);

            return new EmployeeBenefitSelectionResponseDto
            {
                Success = true,
                Message = "Beneficio agregado exitosamente",
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

