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
        
        // Employee-specific fields for API benefits
        public int? NumDependents { get; set; }
        public string? PensionType { get; set; }
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
        
        // Employee-specific fields for API benefits
        [Range(0, int.MaxValue, ErrorMessage = "El número de dependientes debe ser mayor o igual a 0")]
        public int? NumDependents { get; set; }
        
        [RegularExpression(@"^[ABC]$", ErrorMessage = "El tipo de pensión debe ser A, B o C")]
        public string? PensionType { get; set; }
    }

    public class EmployeeBenefitSelectionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CurrentSelections { get; set; }
        public int MaxSelections { get; set; }
    }
}
