using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class RegisterEmployeeDto
    {
        // Personal Information
        [Required]
        [MaxLength(20)]
        public string PrimerNombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? SegundoNombre { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaNacimiento { get; set; }
        
        [Required]
        [MaxLength(9)]
        public string Cedula { get; set; } = string.Empty;
        
        [Required]
        public int Telefono { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Correo { get; set; } = string.Empty;
        
        // Address Information
        [Required]
        [MaxLength(12)]
        public string Provincia { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string Canton { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string Distrito { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(150)]
        public string DireccionParticular { get; set; } = string.Empty;
        
        // Employment Information
        [Required]
        [MaxLength(20)]
        public string Departamento { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TipoContrato { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Puesto { get; set; } = string.Empty;
        
        [Required]
        public int Salario { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string NumeroCuentaIban { get; set; } = string.Empty;
        
        [MaxLength(16)]
        public string? Contrasena { get; set; }
        
        public int? IdEmpresa { get; set; }
    }
}
