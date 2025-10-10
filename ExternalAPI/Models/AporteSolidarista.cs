namespace ExternalAPI.Models
{
    public class AporteSolidarista
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
    public class AporteSolidaristaResponse
    {
        public List<AporteSolidarista> Deductions { get; set; } = new List<AporteSolidarista>();
    }
}