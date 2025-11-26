using backend.DTOs;

namespace backend.Services
{
    public interface IReportGenerationService
    {
        byte[] GeneratePdfReport(DetailedPayrollReportDto report);
        byte[] GenerateExcelReport(HistoricalPayrollReportDto report);
    }
}

