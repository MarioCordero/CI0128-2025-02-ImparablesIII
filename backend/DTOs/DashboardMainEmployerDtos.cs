using backend.Models;

namespace backend.DTOs
{
    public class DashboardMainEmployerDto
    {
        public List<CompanyDashboardMainEmployerDto> Companies { get; set; } = new List<CompanyDashboardMainEmployerDto>();
    }

    public class CompanyDashboardMainEmployerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LegalId { get; set; } = string.Empty;
        public int ActiveEmployees { get; set; }
        public string PayPeriod { get; set; } = string.Empty;
        public decimal MonthlyPayroll { get; set; }
        public decimal CurrentProfitability { get; set; }
        public decimal LastMonthProfitability { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
    }

    public class NotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}