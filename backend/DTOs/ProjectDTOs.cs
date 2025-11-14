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

        [Required(ErrorMessage = "El máximo de beneficios es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El máximo de beneficios debe ser mayor a 0")]
        public int MaximoBeneficios { get; set; }
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
        public string? Direccion { get; set; }
        public int MaximoBeneficios { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // DASHBOARD FIELDS
        public int ActiveEmployees { get; set; }
        public decimal MonthlyPayroll { get; set; }
        public decimal CurrentProfitability { get; set; }
        public decimal LastMonthProfitability { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
    }

    public class NotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
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

        [Required(ErrorMessage = "El máximo de beneficios es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El máximo de beneficios debe ser mayor a 0")]
        public int MaximoBeneficios { get; set; }
    }

    public class DireccionDto
    {
        public int Id { get; set; }
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? DireccionParticular { get; set; }
    }

    public class UpdateProjectDTO
    {
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; }

        [Required]
        [Range(100000000, 999999999, ErrorMessage = "La cédula jurídica debe tener 9 dígitos.")]
        public int CedulaJuridica { get; set; }

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Email { get; set; }

        [Required]
        [Range(10000000, 99999999, ErrorMessage = "El teléfono debe tener 8 dígitos.")]
        public int Telefono { get; set; }

        [Required]
        [RegularExpression(@"^(Mensual|Quincenal)$", ErrorMessage = "El período de pago debe ser 'Mensual' o 'Quincenal'.")]
        public string PeriodoPago { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El máximo de beneficios debe ser mayor a 0.")]
        public int MaximoBeneficios { get; set; }

        [Required]
        public DireccionDTO Direccion { get; set; }
    }

    public class DireccionDTO
    {
        [Required]
        public string Provincia { get; set; }

        [MaxLength(30)]
        public string Canton { get; set; }

        [MaxLength(30)]
        public string Distrito { get; set; }

        [MaxLength(150)]
        public string DireccionParticular { get; set; }
    }

    public class UpdateProjectResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public ProjectResponseDto Project { get; set; }
    }

    // Alias para compatibilidad durante la transición
    public class ProjectListDto : ProjectResponseDto { }
    public class CompanyDashboardMainEmployerDto : ProjectResponseDto { }
    public class DashboardMainEmployerDto : ProjectResponseDto { }
}