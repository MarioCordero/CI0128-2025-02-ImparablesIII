namespace backend.DTOs
{
    public enum PeriodType { Monthly, Biweekly }
    public enum PaymentStatus { Pending, Paid }

    public class PayrollFiltersDto
    {
        public int CompanyId { get; set; }
        public string? Department { get; set; }
        public DateTime Period { get; set; }
        public PeriodType PeriodType { get; set; } = PeriodType.Monthly;
        public int? Fortnight { get; set; } // 1 or 2 if Biweekly
    }

    public class DeductionBreakdownDto
    {
        public string Type { get; set; } = "EE"; // EE | ER
        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class BenefitBreakdownDto
    {
        public string Name { get; set; } = "";
        public string Calculation { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class EmployeePayrollDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = "";
        public string Department { get; set; } = "";
        public decimal GrossSalary { get; set; }
        public decimal EmployeeDeductions { get; set; }
        public decimal EmployerDeductions { get; set; }
        public decimal Benefits { get; set; }
        public decimal NetSalary { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }
        public List<DeductionBreakdownDto> Deductions { get; set; } = new();
        public List<BenefitBreakdownDto> BenefitsList { get; set; } = new();
    }

    public class PayrollTotalsDto
    {
        public decimal TotalGross { get; set; }
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal TotalEmployerDeductions { get; set; }
        public decimal TotalBenefits { get; set; }
        public decimal TotalNet { get; set; }
        public decimal CompanyCost => TotalNet + TotalEmployerDeductions;
        public int EmployeeCount { get; set; }
    }

    public class PeriodInfoDto
    {
        public PeriodType PeriodType { get; set; }
        public string Label { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Fortnight { get; set; }
    }

    public class PayrollSummaryDto
    {
        public List<EmployeePayrollDto> Employees { get; set; } = new();
        public PayrollTotalsDto Totals { get; set; } = new();
        public PeriodInfoDto PeriodInfo { get; set; } = new();
        public Dictionary<string, string> Tooltips { get; set; } = new();
    }
}