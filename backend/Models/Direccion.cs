using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Direccion
    {
        public int Id { get; set; }
        
        [MaxLength(12)]
        public string? Provincia { get; set; }
        
        [MaxLength(30)]
        public string? Canton { get; set; }
        
        [MaxLength(30)]
        public string? Distrito { get; set; }
        
        [MaxLength(150)]
        public string? DireccionParticular { get; set; }
    }
}
