using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Persona", Schema = "PlaniFy")]
    public class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? SegundoNombre { get; set; }

        [Required]
        [StringLength(20)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(9)]
        [Column(TypeName = "char(9)")]
        public string Cedula { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Rol { get; set; }

        public int? Telefono { get; set; }

        public int? idDireccion { get; set; }

        // Navigation property for address
        [ForeignKey("idDireccion")]
        public virtual Direccion? Direccion { get; set; }

        // Navigation property for employee details
        public virtual Empleado? Empleado { get; set; }

        // Computed property for full name
        [NotMapped]
        public string NombreCompleto 
        { 
            get 
            { 
                var nombre = !string.IsNullOrEmpty(SegundoNombre) 
                    ? $"{Nombre} {SegundoNombre}" 
                    : Nombre;
                
                return $"{nombre} {Apellidos}";
            } 
        }
    }
}
