using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Direccion", Schema = "PlaniFy")]
    public class Direccion
    {
        [Key]
        public int Id { get; set; }

        // Add other properties as needed based on the Direccion table structure
        // We can query the structure later if needed
        
        // Navigation property
        public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
    }
}
