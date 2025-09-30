using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        public int CedulaJuridica { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string PeriodoPago { get; set; } = string.Empty; // "Mensual" or "Quincenal"
        
        public int? Telefono { get; set; }
        
        [Required]
        public int IdDireccion { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}