using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Empresa", Schema = "PlaniFy")]
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        // Add other properties as needed based on the Empresa table structure
        // We can query the structure later if needed
        
        // Navigation property
        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
