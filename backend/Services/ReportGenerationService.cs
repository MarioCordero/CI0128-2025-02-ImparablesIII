using backend.DTOs;
using ClosedXML.Excel;

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

        public byte[] GenerateExcelReport(HistoricalPayrollReportDto report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Histórico");

            SetHeaderRow(worksheet);
            SetDataRows(worksheet, report);
            SetTotalsRow(worksheet, report);
            FormatWorksheet(worksheet, report.Items.Count);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void SetHeaderRow(IXLWorksheet worksheet)
        {
            var headers = new[]
            {
                "Tipo de contrato",
                "Posición",
                "Fecha de pago",
                "Salario Bruto",
                "Deducciones obligatorias empleado",
                "Deducciones voluntarias",
                "Salario neto"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }
        }

        private void SetDataRows(IXLWorksheet worksheet, HistoricalPayrollReportDto report)
        {
            int row = 2;
            foreach (var item in report.Items)
            {
                worksheet.Cell(row, 1).Value = item.ContractType;
                worksheet.Cell(row, 2).Value = item.Position;
                worksheet.Cell(row, 3).Value = item.PaymentDate;
                worksheet.Cell(row, 4).Value = item.GrossSalary;
                worksheet.Cell(row, 5).Value = item.MandatoryEmployeeDeductions;
                worksheet.Cell(row, 6).Value = item.VoluntaryDeductions;
                worksheet.Cell(row, 7).Value = item.NetSalary;
                row++;
            }
        }

        private void SetTotalsRow(IXLWorksheet worksheet, HistoricalPayrollReportDto report)
        {
            int totalsRow = report.Items.Count + 2;
            worksheet.Cell(totalsRow, 1).Value = "Total";
            worksheet.Cell(totalsRow, 3).Value = "Total";
            worksheet.Cell(totalsRow, 4).Value = report.Totals.TotalGrossSalary;
            worksheet.Cell(totalsRow, 5).Value = report.Totals.TotalMandatoryEmployeeDeductions;
            worksheet.Cell(totalsRow, 6).Value = report.Totals.TotalVoluntaryDeductions;
            worksheet.Cell(totalsRow, 7).Value = report.Totals.TotalNetSalary;
        }

        private void FormatWorksheet(IXLWorksheet worksheet, int dataRowCount)
        {
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var totalsRow = dataRowCount + 2;
            var totalsRange = worksheet.Range(totalsRow, 1, totalsRow, 7);
            totalsRange.Style.Font.Bold = true;
            totalsRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();
        }
    }
}

