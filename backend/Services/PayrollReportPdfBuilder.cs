using backend.DTOs;
using backend.Constants;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Linq;

namespace backend.Services
{
    public class PayrollReportPdfBuilder : IPdfBuilder
    {

        public byte[] BuildPdf(DetailedPayrollReportDto report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurePage(page);
                    BuildPageContent(page, report);
                });
            });

            return document.GeneratePdf();
        }

        private void ConfigurePage(PageDescriptor page)
        {
            page.Size(PageSizes.A4);
            page.Margin(PayrollReportConstants.ReportLayout.PageMarginCentimetres, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(PayrollReportConstants.ReportLayout.DefaultFontSize));
        }

        private void BuildPageContent(PageDescriptor page, DetailedPayrollReportDto report)
        {
            page.Content()
                .Column(column =>
                {
                    column.Spacing(PayrollReportConstants.ReportLayout.DefaultSpacing);
                    BuildHeader(column);
                    BuildEmployeeInfo(column, report);
                    BuildFinancialBreakdown(column, report);
                });
        }

        private void BuildHeader(ColumnDescriptor column)
        {
            column.Item().AlignCenter().Text(text =>
            {
                text.Span(PayrollReportConstants.ReportLabels.ReportTitle)
                    .FontSize(PayrollReportConstants.ReportLayout.HeaderFontSize)
                    .Bold()
                    .FontColor(Colors.Blue.Medium);
            });
        }

        private void BuildEmployeeInfo(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                BuildInfoRow(table, PayrollReportConstants.ReportLabels.CompanyNameLabel, report.NombreEmpresa);
                BuildInfoRow(table, PayrollReportConstants.ReportLabels.EmployeeNameLabel, report.NombreCompletoEmpleado);
                BuildInfoRow(table, PayrollReportConstants.ReportLabels.PaymentDateLabel, report.FechaGeneracion.ToString(PayrollReportConstants.ReportFormatting.DateFormat));
                BuildInfoRow(table, PayrollReportConstants.ReportLabels.ContractTypeLabel, report.TipoContrato);
            });
        }

        private void BuildInfoRow(TableDescriptor table, string label, string value)
        {
            BuildLabelCell(table, label);
            BuildValueCell(table, value);
        }

        private void BuildLabelCell(TableDescriptor table, string label)
        {
            table.Cell().Element(cell =>
            {
                cell.PaddingVertical(PayrollReportConstants.ReportLayout.VerticalPaddingMedium).Text(text =>
                {
                    text.Span(label)
                        .FontSize(PayrollReportConstants.ReportLayout.LabelFontSize)
                        .Italic()
                        .FontColor(Colors.Blue.Medium);
                });
            });
        }

        private void BuildValueCell(TableDescriptor table, string value)
        {
            table.Cell().Element(cell =>
            {
                cell.PaddingVertical(PayrollReportConstants.ReportLayout.VerticalPaddingMedium).Text(text =>
                {
                    text.Span(value)
                        .FontSize(PayrollReportConstants.ReportLayout.DefaultFontSize)
                        .SemiBold();
                });
            });
        }

        private void BuildFinancialBreakdown(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            column.Item().PaddingTop(PayrollReportConstants.ReportLayout.DefaultFontSize).Column(breakdownColumn =>
            {
                BuildGrossSalary(breakdownColumn, report);
                BuildObligatoryDeductions(breakdownColumn, report);
                BuildVoluntaryDeductions(breakdownColumn, report);
                BuildNetPay(breakdownColumn, report);
            });
        }

        private void BuildGrossSalary(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            column.Item().BorderBottom(PayrollReportConstants.ReportLayout.BorderWidth).PaddingVertical(PayrollReportConstants.ReportLayout.VerticalPaddingLarge).Row(row =>
            {
                BuildFinancialLabel(row, PayrollReportConstants.ReportLabels.GrossSalaryLabel, PayrollReportConstants.ReportLayout.SectionFontSize);
                BuildFinancialAmount(row, report.SalarioBruto, PayrollReportConstants.ReportLayout.SectionFontSize, false);
            });
        }

        private void BuildObligatoryDeductions(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            var deductionItems = report.DeduccionesObligatorias
                .Select(d => (d.Nombre, d.Monto))
                .ToList();
            
            BuildDeductionsSection(column, PayrollReportConstants.ReportLabels.ObligatoryDeductionsLabel, deductionItems, 
                report.TotalDeduccionesObligatorias, PayrollReportConstants.ReportLabels.TotalObligatoryDeductionsLabel);
        }

        private void BuildVoluntaryDeductions(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            var deductionItems = report.DeduccionesVoluntarias
                .Select(d => (d.Nombre, d.Monto))
                .ToList();
            
            BuildDeductionsSection(column, PayrollReportConstants.ReportLabels.VoluntaryDeductionsLabel, deductionItems, 
                report.TotalDeduccionesVoluntarias, PayrollReportConstants.ReportLabels.TotalVoluntaryDeductionsLabel);
        }

        private void BuildDeductionsSection(ColumnDescriptor column, string sectionTitle, 
            List<(string Nombre, decimal Monto)> deductionItems, decimal total, string totalLabel)
        {
            column.Item().PaddingTop(PayrollReportConstants.ReportLayout.VerticalPaddingExtraLarge).Column(deductionsColumn =>
            {
                BuildSectionTitle(deductionsColumn, sectionTitle);
                BuildDeductionItems(deductionsColumn, deductionItems);
                BuildDeductionTotal(deductionsColumn, total, totalLabel);
            });
        }

        private void BuildSectionTitle(ColumnDescriptor column, string title)
        {
            column.Item().Text(text =>
            {
                text.Span(title)
                    .SemiBold()
                    .FontSize(PayrollReportConstants.ReportLayout.SectionFontSize);
            });
        }

        private void BuildDeductionItems(ColumnDescriptor column, List<(string Nombre, decimal Monto)> deductionItems)
        {
            column.Item().PaddingLeft(PayrollReportConstants.ReportLayout.IndentationLeft).Column(itemsColumn =>
            {
                foreach (var deduction in deductionItems)
                {
                    BuildDeductionItem(itemsColumn, deduction.Nombre, deduction.Monto);
                }
            });
        }

        private void BuildDeductionItem(ColumnDescriptor column, string name, decimal amount)
        {
            column.Item().PaddingVertical(PayrollReportConstants.ReportLayout.VerticalPaddingSmall).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span(name)
                        .FontSize(PayrollReportConstants.ReportLayout.LabelFontSize);
                });

                row.ConstantItem(PayrollReportConstants.ReportLayout.AmountColumnWidth).AlignRight().Text(text =>
                {
                    text.Span($"{PayrollReportConstants.ReportFormatting.NegativePrefix}{PayrollReportConstants.ReportFormatting.CurrencySymbol}{FormatCurrency(amount)}")
                        .FontSize(PayrollReportConstants.ReportLayout.LabelFontSize);
                });
            });
        }

        private void BuildDeductionTotal(ColumnDescriptor column, decimal total, string label)
        {
            column.Item().BorderTop(PayrollReportConstants.ReportLayout.BorderWidth).PaddingTop(PayrollReportConstants.ReportLayout.VerticalPaddingMedium).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span(label)
                        .Bold()
                        .FontSize(PayrollReportConstants.ReportLayout.DefaultFontSize);
                });

                row.ConstantItem(PayrollReportConstants.ReportLayout.AmountColumnWidth).AlignRight().Text(text =>
                {
                    text.Span($"{PayrollReportConstants.ReportFormatting.NegativePrefix}{PayrollReportConstants.ReportFormatting.CurrencySymbol}{FormatCurrency(total)}")
                        .Bold()
                        .FontSize(PayrollReportConstants.ReportLayout.DefaultFontSize);
                });
            });
        }

        private void BuildNetPay(ColumnDescriptor column, DetailedPayrollReportDto report)
        {
            column.Item().BorderTop(PayrollReportConstants.ReportLayout.BorderWidthThick).PaddingTop(PayrollReportConstants.ReportLayout.VerticalPaddingExtraLarge).Row(row =>
            {
                BuildFinancialLabel(row, PayrollReportConstants.ReportLabels.NetPayLabel, PayrollReportConstants.ReportLayout.NetPayFontSize);
                BuildFinancialAmount(row, report.SalarioNeto, PayrollReportConstants.ReportLayout.NetPayFontSize, false);
            });
        }

        private void BuildFinancialLabel(RowDescriptor row, string label, int fontSize)
        {
            row.ConstantItem(PayrollReportConstants.ReportLayout.LabelColumnWidth).Text(text =>
            {
                text.Span(label)
                    .Bold()
                    .FontSize(fontSize);
            });
        }

        private void BuildFinancialAmount(RowDescriptor row, decimal amount, int fontSize, bool isNegative)
        {
            row.RelativeItem().AlignRight().Text(text =>
            {
                var prefix = isNegative ? PayrollReportConstants.ReportFormatting.NegativePrefix : string.Empty;
                text.Span($"{prefix}{PayrollReportConstants.ReportFormatting.CurrencySymbol}{FormatCurrency(amount)}")
                    .Bold()
                    .FontSize(fontSize);
            });
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString(PayrollReportConstants.ReportFormatting.CurrencyFormat, new CultureInfo(PayrollReportConstants.ReportFormatting.CultureCode));
        }
    }
}


