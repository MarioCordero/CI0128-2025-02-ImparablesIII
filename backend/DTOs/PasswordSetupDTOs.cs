using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class PasswordSetupRequestDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [MaxLength(16, ErrorMessage = "La contraseña no puede exceder 16 caracteres")]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class PasswordSetupResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PasswordTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
