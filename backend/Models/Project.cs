using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        public int CedulaJuridica { get; set; }

        [Required]
        public int EmployerId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string PeriodoPago { get; set; } = string.Empty;
        
        public int? Telefono { get; set; }
        
        [Required]
        public int IdDireccion { get; set; }
        
        // Navigation properties
        public Direccion? Direccion { get; set; }
        public List<Benefit>? Benefits { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int MaximoBeneficios { get; set; }
    }
}