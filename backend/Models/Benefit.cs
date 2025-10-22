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
        
        // Navigation property - change from Empresa to Project
        public Project? Project { get; set; }
    }
}