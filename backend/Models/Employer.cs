using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Employer
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
        [MaxLength(20)]
        public string Cedula { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaNacimiento { get; set; }
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public bool IsEmailVerified { get; set; } = false;
        
        public string? EmailVerificationToken { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
