using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Empleado", Schema = "PlaniFy")]
    public class Empleado
    {
        [Key]
        [ForeignKey("Persona")]
        public int idPersona { get; set; }

        [Required]
        [StringLength(20)]
        public string Departamento { get; set; } = string.Empty;

        [Required]
        [StringLength(25)]
        public string TipoContrato { get; set; } = string.Empty;

        [StringLength(10)]
        public string? TipoSalario { get; set; }

        [Required]
        [StringLength(20)]
        public string Puesto { get; set; } = string.Empty;

        [Required]
        public DateTime FechaContratacion { get; set; }

        [Required]
        public int Salario { get; set; }

        [Required]
        [StringLength(30)]
        public string iban { get; set; } = string.Empty;

        [StringLength(16)]
        public string? Contrasena { get; set; }

        public int? idEmpresa { get; set; }

        // Navigation properties
        public virtual Persona Persona { get; set; } = null!;
        
        [ForeignKey("idEmpresa")]
        public virtual Empresa? Empresa { get; set; }
    }
}
