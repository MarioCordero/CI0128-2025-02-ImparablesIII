using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CreateBenefitDto
    {
        [Required(ErrorMessage = "El ID de la empresa es obligatorio")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "El Nombre del beneficio es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Nombre debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$", ErrorMessage = "El Nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tipo de cálculo es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Tipo de cálculo debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^(Porcentaje|Monto Fijo|API)$", ErrorMessage = "El Tipo de cálculo debe ser 'Porcentaje', 'Monto Fijo' o 'API'")]
        public string CalculationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tipo de beneficio es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Tipo de beneficio debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^(Bonificación|Descuento|Prestación)$", ErrorMessage = "El Tipo debe ser 'Bonificación', 'Descuento' o 'Prestación'")]
        public string Type { get; set; } = string.Empty;
    }

    public class BenefitResponseDto
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
    }

    public class UpdateBenefitDto
    {
        [Required(ErrorMessage = "El Nombre del beneficio es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Nombre debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\u00f1\u00d1\s]+$", ErrorMessage = "El Nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tipo de cálculo es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Tipo de cálculo debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^(Porcentaje|Monto Fijo|API)$", ErrorMessage = "El Tipo de cálculo debe ser 'Porcentaje', 'Monto Fijo' o 'API'")]
        public string CalculationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tipo de beneficio es obligatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El Tipo de beneficio debe tener entre 1 y 20 caracteres")]
        [RegularExpression(@"^(Bonificación|Descuento|Prestación)$", ErrorMessage = "El Tipo debe ser 'Bonificación', 'Descuento' o 'Prestación'")]
        public string Type { get; set; } = string.Empty;
    }

    public class BenefitListDto
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
