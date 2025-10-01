using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Beneficio
    {
        [Required]
        public int IdEmpresa { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TipoCalculo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = string.Empty;
        
        // Navigation property - cambiar de Empresa a Project
        public Project? Project { get; set; }
    }
}