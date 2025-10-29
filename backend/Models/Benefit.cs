using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Benefit
    {
        [Required]
        public int CompanyId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string CalculationType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue, ErrorMessage = "El Valor debe ser mayor a 0")]
        public int? Value { get; set; }

        [Range(0, 100, ErrorMessage = "El Porcentaje debe estar entre 0 y 100")]
        public int? Percentage { get; set; }

        [MaxLength(200)]
        public string? Descripcion { get; set; }
        
        // Navigation property - change from Empresa to Project
        public Project? Project { get; set; }
    }
}