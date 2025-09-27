// Models/Deduccion.cs
namespace Backend.Models
{
    public class Deduccion
    {
        public string Type { get; set; } // "EE" = empleado, "ER" = empleador
        public decimal Amount { get; set; }
    }

    public class DeduccionResponse
    {
        public List<Deduccion> Deductions { get; set; }
    }
}