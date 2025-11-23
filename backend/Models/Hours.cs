using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("HorasTrabajadas", Schema = "PlaniFy")]
    public class Hours
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("idEmpleado")]
        public int EmployeeId { get; set; }

        [Required]
        [Column("Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [Column("Detalle")]
        [MaxLength(300)]
        public string Detail { get; set; } = string.Empty;

        [Required]
        [Column("Fecha")]
        public DateTime Date { get; set; }

        [Required]
        [Column("Estado")]
        [MaxLength(9)]
        public string Status { get; set; } = "Pendiente"; // Pendiente | Aprobado | Rechazado

        [Required]
        [Column("idAprobador")]
        public int ApproverId { get; set; }
    }
}