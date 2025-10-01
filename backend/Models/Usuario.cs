using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Usuario
    {
        [Required]
        public int IdPersona { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TipoUsuario { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(16)]
        public string Contrasena { get; set; } = string.Empty;
    }
}
