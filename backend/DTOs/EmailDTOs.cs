using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
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

    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class SendVerificationEmailDto
    {
        public string Email { get; set; }
        public string Nombre { get; set; }
        public int PersonaId { get; set; }
        public string Rol { get; set; } // 'Empleador' o 'Empleado'
    }

    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class VerifyAndCreateUserDto
    {
        public int PersonaId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}