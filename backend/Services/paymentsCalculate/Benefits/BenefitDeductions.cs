using backend.Constants;
using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Services.PaymentsCalculate;
using backend.Services.PaymentsCalculate.Benefits;

namespace backend.Services
{
  public class BenefitDeductionsService : IBenefitDeductionsService
  {
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeBenefitRepository _employeeBenefitRepository;
    private readonly IBenefitRepository _benefitRepository;
    private readonly PrivateInsurance _privateInsuranceService;
    private readonly SolidarityAssociation _solidarityAssociationService;
    private readonly VoluntaryPension _voluntaryPensionService;
    private readonly ILogger<BenefitDeductionsService> _logger;

    public BenefitDeductionsService(
        IEmployeeRepository employeeRepository,
        IEmployeeBenefitRepository employeeBenefitRepository,
        IBenefitRepository benefitRepository,
        PrivateInsurance privateInsuranceService,
        SolidarityAssociation solidarityAssociationService,
        VoluntaryPension voluntaryPensionService,
        ILogger<BenefitDeductionsService> logger)
    {
      _employeeRepository = employeeRepository;
      _employeeBenefitRepository = employeeBenefitRepository;
      _benefitRepository = benefitRepository;
      _privateInsuranceService = privateInsuranceService;
      _solidarityAssociationService = solidarityAssociationService;
      _voluntaryPensionService = voluntaryPensionService;
      _logger = logger;
    }

    public async Task<BenefitDeductionCalculationDto> CalculateBenefitDeductionsAsync(int userId, int projectId)
    {
      try
      {
        _logger.LogInformation("Calculating benefit deductions for user {UserId} in project {ProjectId}", userId, projectId);

        var employee = await GetEmployeeAsync(userId);
        if (employee == null)
        {
          throw new ArgumentException($"Employee with ID {userId} not found");
        }

        var employeeBenefits = await GetEmployeeBenefitsAsync(userId, projectId, employee.Salario);
        var deductions = new List<BenefitDeductionItemDto>();

        foreach (var employeeBenefit in employeeBenefits)
        {
          var benefitDeductions = await CalculateBenefitDeductionAsync(employeeBenefit, employee.Salario);
          
          // Convert BenefitDeductionDto to BenefitDeductionItemDto with code and role
          foreach (var deduction in benefitDeductions)
          {
            deductions.Add(ConvertToBenefitDeductionItemDto(deduction, employeeBenefit.BenefitName));
          }
        }

        return new BenefitDeductionCalculationDto
        {
          Deductions = deductions
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error calculating benefit deductions for user {UserId}", userId);
        throw;
      }
    }

    private async Task<Empleado?> GetEmployeeAsync(int userId)
    {
      return await _employeeRepository.GetEmployeeByIdAsync(userId);
    }

    private async Task<List<EmployeeBenefitCalculationDto>> GetEmployeeBenefitsAsync(int userId, int projectId, int employeeSalary)
    {
      var selectedBenefits = await _employeeBenefitRepository.GetSelectedBenefitsForEmployeeAsync(userId, projectId);
      var employeeBenefits = new List<EmployeeBenefitCalculationDto>();

      foreach (var selectedBenefit in selectedBenefits)
      {
          employeeBenefits.Add(new EmployeeBenefitCalculationDto
          {
            EmployeeId = userId,
            CompanyId = projectId,
            BenefitName = selectedBenefit.BenefitName,
            BenefitType = selectedBenefit.BenefitType,
            CalculationType = selectedBenefit.CalculationType,
            BenefitValue = selectedBenefit.Value,
            BenefitPercentage = selectedBenefit.Percentage,
            EmployeeSalary = employeeSalary,
            NumDependents = selectedBenefit.NumDependents,
            PensionType = selectedBenefit.PensionType
          });
        }

      return employeeBenefits;
    }

    private async Task<List<BenefitDeductionDto>> CalculateBenefitDeductionAsync(EmployeeBenefitCalculationDto employeeBenefit, int employeeSalary)
    {
      var benefitType = employeeBenefit.BenefitType.ToLower();
      
      return benefitType switch
      {
        BenefitConstants.BenefitTypeBonificacion => await CalculateBonificationDeductions(employeeBenefit),
        BenefitConstants.BenefitTypeDescuento => await CalculateDiscountDeductions(employeeBenefit),
        BenefitConstants.BenefitTypeAmbos => await CalculateBothDeductions(employeeBenefit),
        _ => await HandleUnknownBenefitTypeAsync(employeeBenefit.BenefitType)
      };
    }

    private Task<List<BenefitDeductionDto>> HandleUnknownBenefitTypeAsync(string benefitType)
    {
      _logger.LogWarning("Unknown benefit type: {BenefitType}", benefitType);
      return Task.FromResult(new List<BenefitDeductionDto>());
    }

    private async Task<List<BenefitDeductionDto>> CalculateBonificationDeductions(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var amount = CalculateBenefitAmount(employeeBenefit);
      return CreateSingleDeduction(BenefitConstants.DeductionTypeEmployer, amount);
    }

    private async Task<List<BenefitDeductionDto>> CalculateDiscountDeductions(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var amount = CalculateBenefitAmount(employeeBenefit);
      return CreateSingleDeduction(BenefitConstants.DeductionTypeEmployee, amount);
    }

    private async Task<List<BenefitDeductionDto>> CalculateBothDeductions(EmployeeBenefitCalculationDto employeeBenefit)
    {
      return await CalculateApiBenefitAmount(employeeBenefit);
    }

    private static bool IsApiCalculationType(string calculationType)
    {
      return calculationType.ToLower() == BenefitConstants.CalculationTypeApi;
    }

    private int CalculateBenefitAmount(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var calculationType = employeeBenefit.CalculationType.ToLower();
      
      return calculationType switch
      {
        BenefitConstants.CalculationTypeFixedAmount => employeeBenefit.BenefitValue ?? 0,
        BenefitConstants.CalculationTypePercentage => CalculatePercentageAmount(employeeBenefit),
        _ => HandleUnknownCalculationType(employeeBenefit.CalculationType)
      };
    }

    private int CalculatePercentageAmount(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var percentage = employeeBenefit.BenefitPercentage ?? 0;
      return (int)(employeeBenefit.EmployeeSalary * percentage / BenefitConstants.PercentageMultiplier);
    }

    private int HandleUnknownCalculationType(string calculationType)
    {
      _logger.LogWarning("Unknown calculation type: {CalculationType}", calculationType);
      return 0;
    }

    private async Task<List<BenefitDeductionDto>> CalculateApiBenefitAmount(EmployeeBenefitCalculationDto employeeBenefit)
    {
      try
      {
        _logger.LogInformation("Calculating API benefit amount for {BenefitName}", employeeBenefit.BenefitName);

        var benefitName = employeeBenefit.BenefitName.ToLower();
        List<BenefitDeductionDto> deductions = new List<BenefitDeductionDto>();

        if (IsInsuranceBenefit(benefitName))
        {
          deductions = await CalculateInsuranceDeductionsAsync(employeeBenefit);
        }
        else if (IsSolidarityAssociationBenefit(benefitName))
        {
          deductions = await CalculateSolidarityAssociationDeductionsAsync(employeeBenefit);
        }
        else if (IsVoluntaryPensionBenefit(benefitName))
        {
          deductions = await CalculateVoluntaryPensionDeductionsAsync(employeeBenefit);
        }
        else
        {
          deductions = HandleUnknownApiBenefit(employeeBenefit.BenefitName);
        }

        _logger.LogInformation("API benefit calculation completed. Found {Count} deductions", deductions.Count);
        return deductions;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error calculating API benefit amount for {BenefitName}", employeeBenefit.BenefitName);
        return new List<BenefitDeductionDto>();
      }
    }

    private static bool IsInsuranceBenefit(string benefitName)
    {
      return benefitName.Contains(BenefitNameKeywords.Insurance) || 
             benefitName.Contains(BenefitNameKeywords.InsuranceEnglish);
    }

    private static bool IsSolidarityAssociationBenefit(string benefitName)
    {
      return benefitName.Contains(BenefitNameKeywords.Association) || 
             benefitName.Contains(BenefitNameKeywords.Solidarity);
    }

    private static bool IsVoluntaryPensionBenefit(string benefitName)
    {
      return benefitName.Contains(BenefitNameKeywords.Pension) || 
             benefitName.Contains(BenefitNameKeywords.Voluntary);
    }

    private static List<BenefitDeductionDto> HandleUnknownApiBenefit(string benefitName)
    {
      return new List<BenefitDeductionDto>();
    }

    private async Task<List<BenefitDeductionDto>> CalculateInsuranceDeductionsAsync(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var employeeAge = await _employeeRepository.GetEmployeeAgeAsync(employeeBenefit.EmployeeId);
      var dependents = employeeBenefit.NumDependents ?? 0;
      var apiDeductions = await _privateInsuranceService.CalculateAsync(employeeAge, dependents);
      return ConvertDeductionItemsToBenefitDeductions(apiDeductions);
    }

    private async Task<List<BenefitDeductionDto>> CalculateSolidarityAssociationDeductionsAsync(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var companyId = employeeBenefit.CompanyId.ToString();
      var grossSalary = (decimal)employeeBenefit.EmployeeSalary;
      var solidarityDeductions = await _solidarityAssociationService.CalculateAsync(companyId, grossSalary);
      return ConvertDeductionItemsToBenefitDeductions(solidarityDeductions);
    }

    private async Task<List<BenefitDeductionDto>> CalculateVoluntaryPensionDeductionsAsync(EmployeeBenefitCalculationDto employeeBenefit)
    {
      var salary = (decimal)employeeBenefit.EmployeeSalary;
      const string defaultPensionType = "A";
      var pensionType = employeeBenefit.PensionType ?? defaultPensionType;
      var pensionDeductions = await _voluntaryPensionService.CalculateAsync(salary, pensionType);
      return ConvertDeductionItemsToBenefitDeductions(pensionDeductions);
    }

    private List<BenefitDeductionDto> ConvertDeductionItemsToBenefitDeductions(List<DeductionItem> deductionItems)
    {
      return deductionItems.Select(item => new BenefitDeductionDto
      {
        Type = item.Type,
        Amount = (int)item.Amount
      }).ToList();
    }

    private static List<BenefitDeductionDto> CreateSingleDeduction(string type, int amount)
    {
      return new List<BenefitDeductionDto> { CreateSingleDeductionItem(type, amount) };
    }

    private static BenefitDeductionDto CreateSingleDeductionItem(string type, int amount)
    {
      return new BenefitDeductionDto { Type = type, Amount = amount };
    }

    private BenefitDeductionItemDto ConvertToBenefitDeductionItemDto(BenefitDeductionDto deduction, string benefitName)
    {
      var code = GenerateBenefitCode(benefitName, deduction.Type);
      var role = MapDeductionTypeToRole(deduction.Type);

      return new BenefitDeductionItemDto
      {
        Code = code,
        Amount = deduction.Amount,
        Role = role
      };
    }

    private static string GenerateBenefitCode(string benefitName, string deductionType)
    {
      // Normalize benefit name: remove spaces, convert to uppercase, replace spaces with underscores
      var normalizedName = benefitName
        .ToUpper()
        .Replace(" ", "_")
        .Replace("-", "_");

      // Return code in format: BENEFITNAME_TYPE
      return $"{normalizedName}_{deductionType}";
    }

    private static string MapDeductionTypeToRole(string deductionType)
    {
      return deductionType.ToUpper() switch
      {
        BenefitConstants.DeductionTypeEmployer => DeductionRoleNames.EmployerDeduction,
        BenefitConstants.DeductionTypeEmployee => DeductionRoleNames.EmployeeDeduction,
        _ => DeductionRoleNames.EmployeeDeduction
      };
    }
  }
}