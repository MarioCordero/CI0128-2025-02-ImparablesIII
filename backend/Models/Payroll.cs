using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Payroll")]
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public DateTime Period { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PeriodType { get; set; } = "Monthly";
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossSalary { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CcssEmployee { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CcssEmployer { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal IncomeTax { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal OtherDeductions { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Benefits { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public virtual Empleado Employee { get; set; }
    }

    [Table("PayrollBenefits")]
    public class PayrollBenefit
    {
        [Key]
        public int PayrollBenefitId { get; set; }
        
        [Required]
        public int PayrollId { get; set; }
        
        [Required]
        public int BenefitId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        // Navigation properties
        public virtual Payroll Payroll { get; set; }
        public virtual Beneficio Benefit { get; set; }
    }
}