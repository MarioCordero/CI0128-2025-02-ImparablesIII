using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class BenefitDeductionDto
    {
        [Required]
        [RegularExpression(@"^(ER|EE)$", ErrorMessage = "El tipo debe ser 'ER' (Empleador) o 'EE' (Empleado)")]
        public string Type { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public int Amount { get; set; }
    }

    public class BenefitDeductionCalculationDto
    {
        public List<BenefitDeductionItemDto> Deductions { get; set; } = new List<BenefitDeductionItemDto>();
    }

    public class BenefitDeductionItemDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public int Amount { get; set; }
        
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class EmployeeBenefitCalculationDto
    {
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string BenefitName { get; set; } = string.Empty;
        public string BenefitType { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public int? BenefitValue { get; set; }
        public int? BenefitPercentage { get; set; }
        public int EmployeeSalary { get; set; }
        public int? NumDependents { get; set; }
        public string? PensionType { get; set; }
    }
}
