using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    // DTOs para Request
    public class PayrollFiltersDto
    {
        public int? DepartmentId { get; set; }
        public DateTime Period { get; set; }
        public string PeriodType { get; set; } = "Monthly"; // Monthly, Biweekly
    }

    public class PayrollCalculationRequestDto
    {
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public DateTime Period { get; set; }
        
        [Required]
        public string PeriodType { get; set; }
    }

    // DTOs para Response
    public class PayrollSummaryDto
    {
        public List<EmployeePayrollDto> Employees { get; set; } = new();
        public PayrollTotalsDto Totals { get; set; } = new();
        public PeriodInfoDto PeriodInfo { get; set; } = new();
    }

    public class EmployeePayrollDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string GrossSalary { get; set; } = string.Empty;
        public string EmployeeDeductions { get; set; } = string.Empty;
        public string EmployerDeductions { get; set; } = string.Empty;
        public string Benefits { get; set; } = string.Empty;
        public string NetSalary { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Pending";
        public string StatusColor { get; set; } = "warning";
        public List<DeductionTooltipDto> DeductionTooltips { get; set; } = new();
    }

    public class PayrollTotalsDto
    {
        public string TotalCompanyGrossSalary { get; set; } = "₡0";
        public string TotalEmployerDeductions { get; set; } = "₡0";
        public string TotalEmployeeDeductions { get; set; } = "₡0";
        public string TotalBenefits { get; set; } = "₡0";
        public string TotalNetSalary { get; set; } = "₡0";
    }

    public class PeriodInfoDto
    {
        public string Period { get; set; } = string.Empty;
        public string PeriodType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class DeductionTooltipDto
    {
        public string Concept { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }

    public class PayrollDetailDto
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal GrossSalary { get; set; }
        public decimal CcssEmployee { get; set; }
        public decimal CcssEmployer { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal Benefits { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime Period { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Department { get; set; }

    }
}