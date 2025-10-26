using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public enum PeriodType { Monthly, Biweekly }
    public enum PaymentStatus { Pending, Paid, Processing, Failed }

    // --------------------------------- Requests ---------------------------------
    public class PayrollFiltersDto
    {
        [Required] public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }

        // Punto de referencia del período (ej. 2025-10-01)
        [Required] public DateTime Period { get; set; }

        [Required] public PeriodType PeriodType { get; set; } = PeriodType.Monthly;

        // Solo cuando PeriodType = Biweekly: 1 ó 2
        [Range(1, 2)] public int? Fortnight { get; set; }
    }

    public class PayrollCalculationRequestDto
    {
        [Required] public int CompanyId { get; set; }
        [Required] public int EmployeeId { get; set; }
        [Required] public DateTime Period { get; set; }
        [Required] public PeriodType PeriodType { get; set; }
        [Range(1, 2)] public int? Fortnight { get; set; }
    }

    // --------------------------------- Responses ---------------------------------
    public class PayrollSummaryDto
    {
        public List<EmployeePayrollDto> Employees { get; set; } = new();
        public PayrollTotalsDto Totals { get; set; } = new();
        public PeriodInfoDto PeriodInfo { get; set; } = new();
        // Tooltips centralizados (clave → texto). El front decide dónde mostrarlos.
        public Dictionary<string, string> Tooltips { get; set; } = new();
    }

    public class EmployeePayrollDto
    {
        public int PayrollId { get; set; }                 // si existe detalle generado
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string Department { get; set; } = string.Empty;

        // --- MONTOS: SIEMPRE DECIMAL EN BACKEND ---
        public decimal GrossSalary { get; set; }           // Bruto del período (mensual/quincenal)
        public decimal EmployeeDeductions { get; set; }    // EE (renta, CCSS EE, solidarista EE…)
        public decimal EmployerDeductions { get; set; }    // ER (CCSS patronal, solidarista ER…)
        public decimal Benefits { get; set; }              // voluntarios del empleado
        public decimal NetSalary { get; set; }             // Bruto - EE + Beneficios

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }

        // Desglose para tooltips
        public List<DeductionBreakdownDto> Deductions { get; set; } = new();
        public List<BenefitBreakdownDto> BenefitBreakdown { get; set; } = new();
    }

    public class DeductionBreakdownDto
    {
        public string Type { get; set; } = "EE";           // "EE" o "ER"
        public string Name { get; set; } = string.Empty;   // CCSS, Renta, Solidarista, etc.
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class BenefitBreakdownDto
    {
        public string Name { get; set; } = string.Empty;
        public string Calculation { get; set; } = string.Empty; // "5% del bruto", "₡25 000 fijo", etc.
        public decimal Amount { get; set; }
    }

    public class PayrollTotalsDto
    {
        public decimal TotalCompanyGrossSalary { get; set; }
        public decimal TotalEmployerDeductions { get; set; }
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal TotalBenefits { get; set; }
        public decimal TotalNetSalary { get; set; }

        // (opcional pero útil) costo total para la empresa
        public decimal TotalCompanyCost => TotalNetSalary + TotalEmployerDeductions;
        public int EmployeesCount { get; set; }
    }

    public class PeriodInfoDto
    {
        public PeriodType PeriodType { get; set; }
        public string Label { get; set; } = string.Empty;  // "Mensual 2025-10" / "Quincena 1 · 2025-10"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Fortnight { get; set; }                 // 1 ó 2 si aplica
    }
}