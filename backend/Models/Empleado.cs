using System.ComponentModel.DataAnnotations;
using backend.Constants;

namespace backend.Models
{
    public class Empleado
    {
        [Required]
        public int IdPersona { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Departamento { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TipoContrato { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? TipoSalario { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Puesto { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaContratacion { get; set; }
        
        [Required]
        public int Salario { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string Iban { get; set; } = string.Empty;
        
        [MaxLength(16)]
        public string? Contrasena { get; set; }
        
        public int? IdEmpresa { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = EmployeeConstants.StatusActive;

        public DateTime? FechaBaja { get; set; }

        [MaxLength(500)]
        public string? MotivoBaja { get; set; }

        public int? UsuarioBajaId { get; set; }

        public Persona? Persona { get; set; }

        public bool Activo => Estado == EmployeeConstants.StatusActive;

    }
}
