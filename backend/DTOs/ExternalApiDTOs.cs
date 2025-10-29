namespace backend.DTOs
{
    public class ExternalApiDeductionResponse
    {
        public List<DeductionItem> Deductions { get; set; } = new();
    }

    public class DeductionItem
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
