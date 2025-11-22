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
        public string? VerificationTokenHash { get; set; }
        public DateTime? VerificationTokenExpires { get; set; }
        public bool IsVerified { get; set; }
    }
}