using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class EmployeeRow
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Departamento { get; set; } = "";
        public decimal Salario { get; set; }
    }

    public class BenefitRow
    {
        [Key]
        public int PayrollBenefitId { get; set; }
        
        [Required]
        public int PayrollId { get; set; }
        
        [Required]
        public int BenefitId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }

    public class BenefitEmployeeRow
    {
        public int idEmpleado { get; set; }
        public string NombreBeneficio { get; set; } = "";
        public int idEmpresa { get; set; }
        public string TipoBeneficio { get; set; } = "";
    }

    public class DeductionEmployeeRow
    {
        public int idPlanilla { get; set; }
        public int idEmpleado { get; set; }
        public string Tipo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal Monto { get; set; }
    }

    public class PayrollRow
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaPago { get; set; }
    }

    public class PayrollDetailRow
    {
        public int idEmpleado { get; set; }
        public int idPlanilla { get; set; }
        public decimal salarioBruto { get; set; }
    }
}