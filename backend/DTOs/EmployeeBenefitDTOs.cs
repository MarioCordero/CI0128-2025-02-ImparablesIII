using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class EmployeeBenefitDto
    {
        public int CompanyId { get; set; }
        public string BenefitName { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public string BenefitType { get; set; } = string.Empty;
        public int? Value { get; set; }
        public int? Percentage { get; set; }
        public bool IsSelected { get; set; }
        public int EmployeeCount { get; set; }
        public double UsagePercentage { get; set; }
    }

    public class BenefitFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? CalculationType { get; set; }
        public string? Status { get; set; }
    }

    public class EmployeeBenefitsSummaryDto
    {
        public List<EmployeeBenefitDto> AvailableBenefits { get; set; } = new();
        public List<EmployeeBenefitDto> SelectedBenefits { get; set; } = new();
        public int CurrentSelections { get; set; }
        public int MaxSelections { get; set; }
    }

    public class SelectBenefitRequestDto
    {
        [Required(ErrorMessage = "El nombre del beneficio es obligatorio")]
        public string BenefitName { get; set; } = string.Empty;
    }

    public class EmployeeBenefitSelectionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CurrentSelections { get; set; }
        public int MaxSelections { get; set; }
    }
}
