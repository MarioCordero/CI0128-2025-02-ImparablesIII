using backend.DTOs;

namespace backend.Services
{
    public interface IPdfBuilder
    {
        byte[] BuildPdf(DetailedPayrollReportDto report);
    }
}

