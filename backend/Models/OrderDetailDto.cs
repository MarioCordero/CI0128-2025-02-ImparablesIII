// TEST
namespace backend.Models
{
    public class OrderDetailDto
    {
        public int OrderID { get; set; }
        public DateTime SubmitDate { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}