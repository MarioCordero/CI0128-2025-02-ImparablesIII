using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    // Request DTOs
    public class SendEmailDto
    {
        [Required]
        [EmailAddress]
        public string ReceiverEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = false;
    }

    public class SendVerificationEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int PersonaId { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = string.Empty; // 'Empleador' o 'Empleado'
    }

    public class VerifyEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(32)]
        public string Token { get; set; } = string.Empty;
    }

    public class VerifyAndCreateUserDto
    {
        [Required]
        public int PersonaId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(32)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
    }

    // Response DTOs
    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class VerificationResponseDto
    {
        public bool IsValid { get; set; }
        public int PersonaId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}