
namespace backend.DTOs
{ 
    public class DashboardMainEmployerDto
    {
        public string EmployerName { get; set; }
        public List<CompanyDashboardMainEmployerDto> Companies { get; set; }
    }

    public class CompanyDashboardMainEmployerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LegalId { get; set; }
        public int ActiveEmployees { get; set; }
        public string PayPeriod { get; set; }
        public decimal MonthlyPayroll { get; set; }

        public decimal CurrentProfitability { get; set; }
        public decimal LastMonthProfitability { get; set; }

        public List<NotificationDto> Notifications { get; set; }
    }

    public class NotificationDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "alert", "info", "success"
    }
}
