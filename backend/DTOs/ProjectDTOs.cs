using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(80, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 80 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula jurídica es obligatoria")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La cédula jurídica debe tener exactamente 9 dígitos")]
        public string LegalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\u00f1\u00d10-9\s,.-]+$", ErrorMessage = "La dirección contiene caracteres no válidos")]
        public string? Address { get; set; }

        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El formato del teléfono no es válido")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El máximo de beneficios es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El máximo de beneficios debe ser mayor a 0")]
        public int MaxBenefits { get; set; }

        [Required(ErrorMessage = "El período de pago es obligatorio")]
        [RegularExpression(@"^(quincenal|mensual)$", ErrorMessage = "El período de pago debe ser 'quincenal' o 'mensual'")]
        public string PaymentPeriod { get; set; } = string.Empty;
    }

    public class CompanyResponseDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string LegalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int MaxBenefits { get; set; }
        public string PaymentPeriod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CompanyListDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string LegalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}