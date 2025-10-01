using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class SignUpEmployerDto
    {
      [Required]
      [MaxLength(20)]
      public string Nombre { get; set; } = string.Empty;

      [Required]
      [MaxLength(20)]
      public string PrimerApellido { get; set; } = string.Empty;

      [MaxLength(20)]
      public string? SegundoApellido { get; set; }

      [Required]
      [MaxLength(9)]
      public string Cedula { get; set; } = string.Empty;

      [Required]
      [EmailAddress]
      [MaxLength(50)]
      public string Email { get; set; } = string.Empty;

      [Required]
      public int Telefono { get; set; } = 0;

      [Required]
      public DateTime FechaNacimiento { get; set; }

      [Required]
      [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
      [MaxLength(16, ErrorMessage = "La contraseña no puede exceder 16 caracteres")]
      public string Password { get; set; } = string.Empty;
      
      [Required]
      [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
      public string ConfirmPassword { get; set; } = string.Empty;

      // Address fields
      [Required]
      [MaxLength(12)]
      public string Provincia { get; set; } = string.Empty;

      [Required]
      [MaxLength(30)]
      public string Canton { get; set; } = string.Empty;

      [Required]
      [MaxLength(30)]
      public string Distrito { get; set; } = string.Empty;

      [MaxLength(150)]
      public string? DireccionParticular { get; set; }
    }

}