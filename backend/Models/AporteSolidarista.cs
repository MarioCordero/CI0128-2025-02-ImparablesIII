/// <summary>
/// Modelos para la respuesta de la API de Asociación Solidarista.
/// </summary>
namespace backend.Models
{
    /// <summary>
    /// Representa una deducción solidarista.
    /// Type: "EE" para empleado, "ER" para empleador.
    /// Amount: Monto de la deducción.
    /// </summary>
    public class AporteSolidarista
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }


    /// <summary>
    /// Modelo de respuesta principal para la API de Asociación Solidarista.
    /// Contiene una lista de deducciones calculadas.
    /// </summary>
    public class AporteSolidaristaResponse
    {
        public List<AporteSolidarista> Deductions { get; set; } = new List<AporteSolidarista>();
    }
}