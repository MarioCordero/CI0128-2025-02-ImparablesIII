using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class EmployeeBenefit
    {
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string BenefitName { get; set; } = string.Empty;
        
        [Required]
        public int CompanyId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string BenefitType { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        
        // Additional fields for API benefits - employee-specific data
        [Range(0, int.MaxValue, ErrorMessage = "El número de dependientes debe ser mayor o igual a 0")]
        public int? NumDependents { get; set; }
        
        [RegularExpression(@"^[ABC]$", ErrorMessage = "El tipo de pensión debe ser A, B o C")]
        public string? PensionType { get; set; }
        
        // Navigation properties
        public Empleado? Employee { get; set; }
        public Benefit? Benefit { get; set; }
    }
}
