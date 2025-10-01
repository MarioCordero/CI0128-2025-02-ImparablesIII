using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        public int CedulaJuridica { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string PeriodoPago { get; set; } = string.Empty;
        
        public int? Telefono { get; set; }
        
        [Required]
        public int IdDireccion { get; set; }
        
        // Navigation properties
        public Direccion? Direccion { get; set; }
        public List<Beneficio>? Beneficios { get; set; }
    }
}
