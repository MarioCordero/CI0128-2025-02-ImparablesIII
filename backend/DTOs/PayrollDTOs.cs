namespace backend.DTOs
{
    public enum PeriodType { Monthly, Biweekly }

    public class PayrollFiltersDto
    {
        public int CompanyId { get; set; }
        public DateTime Period { get; set; }
        public PeriodType PeriodType { get; set; } = PeriodType.Monthly;
        public int? Fortnight { get; set; } // 1 or 2 if Biweekly
    }
    public class PayrollTotalsDto
    {
        public decimal TotalGross { get; set; }
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal TotalEmployerDeductions { get; set; }
        public decimal TotalBenefits { get; set; }
        public decimal TotalNet { get; set; }
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
    }
    // ----------------------------DTOs FIJOS PARA DEDUCCIONES DE EMPLEADOR---------------------------
    public class EmployerDeductionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Rate { get; set; }
        public decimal MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsActive { get; set; }
    }
    public class EmployerDeductionLineDto
    {
        public string Code { get; set; } = "";
        public decimal Amount { get; set; }
        public string Role { get; set; } = ""; // "EmployerDeduction"
    }
    public class EmployerDeductionResultDto
    {
        public int IdEmpleado { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public decimal SalarioBruto { get; set; }
        public decimal TotalEmployerDeductions { get; set; }
        public List<EmployerDeductionLineDto> DeductionLines { get; set; } = new();
    }
    // ----------------------------DTOs FIJOS PARA DEDUCCIONES DE EMPLEADO----------------------------
    public class EmployeeDeductionsRequest
    {
        public decimal GrossSalary { get; set; }
        public int EmployeeId { get; set; } // Opcional, si necesitas identificar al empleado
        public List<BenefitContributionDto>? BenefitContributions { get; set; } // Aportes voluntarios o beneficios
    }
    public class BenefitContributionDto
    {
        public string Code { get; set; } = "";
        public decimal Amount { get; set; }
    }
    public class EmployeeDeductionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Rate { get; set; }
        public decimal MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsActive { get; set; }
    }
    public class EmployeeDeductionsResponse
    {
        public decimal GrossSalary { get; set; }
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public List<EmployeeDeductionLineDto> Lines { get; set; } = new();
    }
    public class EmployeeDeductionLineDto
    {
        public string Code { get; set; } = "";
        public decimal Amount { get; set; }
        public string Role { get; set; } = ""; // Ej: "EmployeeDeduction"
    }
        // Para deserializar el JSON raíz del SP
    public class EmpleadosRoot
    {
        public List<EmployeePayrollDto> Empleados { get; set; } = new();
    }

    // DTO para cada empleado con desglose de deducciones
    public class EmployeePayrollDto
    {
        public int IdEmpleado { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public decimal SalarioBruto { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string TipoPago { get; set; } = "";
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public List<EmployeeDeductionLineDto> DeductionLines { get; set; } = new();
    }

    // DTOs para inserción de planilla
    public class PayrollInsertDto
    {
        public DateTime FechaGeneracion { get; set; }
        public int Horas { get; set; }
        public int IdResponsable { get; set; }
        public int IdEmpresa { get; set; }
    }

    public class PayrollDetailInsertDto
    {
        public int IdEmpleado { get; set; }
        public int SalarioBruto { get; set; }
        public int DeduccionesEmpleado { get; set; }
        public int DeduccionesEmpresa { get; set; }
        public int TotalBeneficios { get; set; }
        public int SalarioNeto { get; set; }
        public string Puesto { get; set; } = string.Empty;
    }

    public class PayrollHistoryItemDto
    {
        public int PayrollId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalEmployeeDeductions { get; set; }
        public decimal TotalEmployerDeductions { get; set; }
        public decimal TotalBenefits { get; set; }
        public decimal TotalNet { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class GeneratePayrollRequestDto
    {
        public int CompanyId { get; set; }
        public int ResponsibleEmployeeId { get; set; }
        public int Hours { get; set; }
        public string? PeriodType { get; set; } // "Mensual" | "Quincenal"
        public int? Fortnight { get; set; } // 1 | 2 when Quincenal
    }

    public class GeneratePayrollResponseDto
    {
        public int PayrollId { get; set; }
        public string Message { get; set; } = "";
        public DateTime GeneratedAt { get; set; }
    }

    public class DetailedPayrollReportDto
    {
        public int PayrollId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string NombreCompletoEmpleado { get; set; } = string.Empty;
        public string TipoContrato { get; set; } = string.Empty;
        public decimal SalarioBruto { get; set; }
        public List<MandatoryDeductionDto> DeduccionesObligatorias { get; set; } = new();
        public decimal TotalDeduccionesObligatorias { get; set; }
        public List<VoluntaryDeductionDto> DeduccionesVoluntarias { get; set; } = new();
        public decimal TotalDeduccionesVoluntarias { get; set; }
        public decimal SalarioNeto { get; set; }
    }

    public class MandatoryDeductionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }

    public class VoluntaryDeductionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}