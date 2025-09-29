using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Persona
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Correo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? SegundoNombre { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Apellidos { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaNacimiento { get; set; }
        
        [Required]
        [MaxLength(11)]
        public string Cedula { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Rol { get; set; }
        
        public int? Telefono { get; set; }
        
        public int? IdDireccion { get; set; }
    }
}
