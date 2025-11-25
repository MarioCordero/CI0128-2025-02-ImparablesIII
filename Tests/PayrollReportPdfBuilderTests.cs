using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend.Services;
using backend.DTOs;
using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace backend.Tests
{
    [TestClass]
    public class PayrollReportPdfBuilderTests
    {
        private PayrollReportPdfBuilder _builder = null!;

        [TestInitialize]
        public void Setup()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _builder = new PayrollReportPdfBuilder();
        }
        // -------------------------------------------------------------
        // Null report - should throw ArgumentNullException
        // -------------------------------------------------------------
        [TestMethod]
        public void BuildPdf_ReportNull_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _builder.BuildPdf(null!)
            );
        }

        // -------------------------------------------------------------
        // Valid report - should return a non-empty PDF byte array
        // -------------------------------------------------------------
        [TestMethod]
        public void BuildPdf_ValidReport_ReturnsPdfBytes()
        {
            var report = CreateValidReport();

            var pdfBytes = _builder.BuildPdf(report);

            Assert.IsNotNull(pdfBytes);
            Assert.IsTrue(pdfBytes.Length > 1000, "PDF should have content (more than 1000 bytes)");
        }

        // -------------------------------------------------------------
        // Works with empty deduction lists
        // -------------------------------------------------------------
        [TestMethod]
        public void BuildPdf_ReportWithEmptyDeductions_ReturnsPdf()
        {
            var report = CreateValidReport();
            report.DeduccionesObligatorias = new List<MandatoryDeductionDto>();
            report.DeduccionesVoluntarias = new List<VoluntaryDeductionDto>();

            var pdfBytes = _builder.BuildPdf(report);

            Assert.IsNotNull(pdfBytes);
            Assert.IsTrue(pdfBytes.Length > 1000);
        }

        // -------------------------------------------------------------
        // Zero values shouldn't break the PDF
        // -------------------------------------------------------------
        [TestMethod]
        public void BuildPdf_ReportWithZeroValues_ReturnsPdf()
        {
            var report = CreateValidReport();
            report.SalarioBruto = 0;
            report.SalarioNeto = 0;
            report.TotalDeduccionesObligatorias = 0;
            report.TotalDeduccionesVoluntarias = 0;

            var pdfBytes = _builder.BuildPdf(report);

            Assert.IsNotNull(pdfBytes);
            Assert.IsTrue(pdfBytes.Length > 1000);
        }

        // -------------------------------------------------------------
        // Helper to create a full valid report
        // -------------------------------------------------------------
        private DetailedPayrollReportDto CreateValidReport()
        {
            return new DetailedPayrollReportDto
            {
                PayrollId = 1,
                FechaGeneracion = DateTime.UtcNow,
                NombreEmpresa = "Empresa XYZ",
                NombreCompletoEmpleado = "Juan Pérez",
                TipoContrato = "Tiempo completo",

                SalarioBruto = 1500m,
                SalarioNeto = 1200m,

                TotalDeduccionesObligatorias = 200m,
                TotalDeduccionesVoluntarias = 100m,

                DeduccionesObligatorias = new List<MandatoryDeductionDto>
                {
                    new MandatoryDeductionDto { Nombre = "CCSS", Monto = 100m },
                    new MandatoryDeductionDto { Nombre = "Impuesto", Monto = 100m }
                },

                DeduccionesVoluntarias = new List<VoluntaryDeductionDto>
                {
                    new VoluntaryDeductionDto { Nombre = "Ahorro", Monto = 50m },
                    new VoluntaryDeductionDto { Nombre = "Pensión voluntaria", Monto = 50m }
                }
            };
        }
    }
}