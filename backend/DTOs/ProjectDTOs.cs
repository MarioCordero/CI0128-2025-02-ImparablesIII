using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula jurídica es obligatoria")]
        [Range(100000000, 999999999, ErrorMessage = "La cédula jurídica debe tener exactamente 9 dígitos")]
        public int CedulaJuridica { get; set; }

        // Agregar esta propiedad
        public string LegalId => CedulaJuridica.ToString();

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(50, ErrorMessage = "El correo no puede exceder los 50 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El período de pago es obligatorio")]
        [RegularExpression(@"^(Mensual|Quincenal)$", ErrorMessage = "El período de pago debe ser 'Mensual' o 'Quincenal'")]
        [StringLength(20)]
        public string PeriodoPago { get; set; } = string.Empty;

        [Range(10000000, 99999999, ErrorMessage = "El teléfono debe tener 8 dígitos")]
        public int? Telefono { get; set; }

        // Address fields
        [Required(ErrorMessage = "La provincia es obligatoria")]
        [RegularExpression(@"^(San José|Alajuela|Cartago|Heredia|Guanacaste|Puntarenas|Limón)$",
            ErrorMessage = "La provincia debe ser una de las 7 provincias de Costa Rica")]
        [StringLength(12)]
        public string Provincia { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El cantón no puede exceder los 30 caracteres")]
        public string? Canton { get; set; }

        [StringLength(30, ErrorMessage = "El distrito no puede exceder los 30 caracteres")]
        public string? Distrito { get; set; }

        [StringLength(150, ErrorMessage = "La dirección particular no puede exceder los 150 caracteres")]
        public string? DireccionParticular { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 20 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(50, ErrorMessage = "El correo no puede exceder los 50 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El período de pago es obligatorio")]
        [RegularExpression(@"^(Mensual|Quincenal)$", ErrorMessage = "El período de pago debe ser 'Mensual' o 'Quincenal'")]
        [StringLength(20)]
        public string PeriodoPago { get; set; } = string.Empty;

        [Range(10000000, 99999999, ErrorMessage = "El teléfono debe tener 8 dígitos")]
        public int? Telefono { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CedulaJuridica { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PeriodoPago { get; set; } = string.Empty;
        public int? Telefono { get; set; }
        public int IdDireccion { get; set; }
        public DireccionDto? Direccion { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProjectListDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CedulaJuridica { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PeriodoPago { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DireccionDto
    {
        public int Id { get; set; }
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? DireccionParticular { get; set; }
    }
}