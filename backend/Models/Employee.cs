using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? SegundoApellido { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Celular { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Puesto { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El salario debe ser mayor que 0")]
        public decimal Salario { get; set; }
        
        [Required]
        public EmployeeStatus Estado { get; set; } = EmployeeStatus.Pendiente;
        
        public int EmployerId { get; set; } // Foreign key to associate with employer
        
        public DateTime FechaContratacion { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Computed property for full name
        public string NombreCompleto => 
            $"{Nombre} {PrimerApellido}" + (string.IsNullOrEmpty(SegundoApellido) ? "" : $" {SegundoApellido}");
    }
    
    public enum EmployeeStatus
    {
        Activa = 1,
        Pendiente = 2,
        Desactivada = 3
    }
}
