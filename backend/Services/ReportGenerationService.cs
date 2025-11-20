using backend.DTOs;

namespace backend.Services
{
    public class ReportGenerationService : IReportGenerationService
    {
        private readonly IPdfBuilder _pdfBuilder;

        public ReportGenerationService(IPdfBuilder pdfBuilder)
        {
            _pdfBuilder = pdfBuilder ?? throw new ArgumentNullException(nameof(pdfBuilder));
        }

        public byte[] GeneratePdfReport(DetailedPayrollReportDto report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            return _pdfBuilder.BuildPdf(report);
        }
    }
}

