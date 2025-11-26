using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.DTOs;
using System;

namespace backend.Tests
{
    [TestClass]
    public class ReportGenerationServiceTests
    {
        private Mock<IPdfBuilder> _pdfBuilderMock = null!;
        private ReportGenerationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _pdfBuilderMock = new Mock<IPdfBuilder>(MockBehavior.Strict);
            _service = new ReportGenerationService(_pdfBuilderMock.Object);
        }

        // ------------------------------------------------------------
        // Constructor Tests
        // ------------------------------------------------------------
        [TestMethod]
        public void Constructor_NullPdfBuilder_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ReportGenerationService(null!));
        }

        // ------------------------------------------------------------
        // GeneratePdfReport Tests
        // ------------------------------------------------------------
        [TestMethod]
        public void GeneratePdfReport_ValidReport_ReturnsPdfBytes()
        {
            // Arrange
            var report = new DetailedPayrollReportDto(); // no usamos propiedades específicas

            byte[] expectedPdf = new byte[] { 1, 2, 3, 4 };

            _pdfBuilderMock
                .Setup(x => x.BuildPdf(report))
                .Returns(expectedPdf);

            // Act
            var result = _service.GeneratePdfReport(report);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPdf.Length, result.Length);
            CollectionAssert.AreEqual(expectedPdf, result);

            _pdfBuilderMock.Verify(x => x.BuildPdf(report), Times.Once);
        }

        [TestMethod]
        public void GeneratePdfReport_NullReport_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _service.GeneratePdfReport(null!));
        }

        // ------------------------------------------------------------
        // GenerateExcelReport Tests
        // ------------------------------------------------------------
        [TestMethod]
        public void GenerateExcelReport_ValidReport_ReturnsExcelBytes()
        {
            var report = new HistoricalPayrollReportDto
            {
                Items = new List<HistoricalPayrollReportItemDto>
                {
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 1,
                        ContractType = "Tiempo Completo",
                        Position = "Desarrollador",
                        PaymentDate = new DateTime(2024, 1, 15),
                        GrossSalary = 500000m,
                        MandatoryEmployeeDeductions = 50000m,
                        VoluntaryDeductions = 10000m,
                        NetSalary = 440000m
                    },
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 2,
                        ContractType = "Medio Tiempo",
                        Position = "Analista",
                        PaymentDate = new DateTime(2024, 2, 15),
                        GrossSalary = 300000m,
                        MandatoryEmployeeDeductions = 30000m,
                        VoluntaryDeductions = 5000m,
                        NetSalary = 265000m
                    }
                },
                Totals = new HistoricalPayrollReportTotalsDto
                {
                    TotalGrossSalary = 800000m,
                    TotalMandatoryEmployeeDeductions = 80000m,
                    TotalVoluntaryDeductions = 15000m,
                    TotalNetSalary = 705000m
                }
            };

            var result = _service.GenerateExcelReport(report);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void GenerateExcelReport_NullReport_ThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                _service.GenerateExcelReport(null!));
        }

        [TestMethod]
        public void GenerateExcelReport_EmptyItems_ReturnsExcelWithHeadersAndTotals()
        {
            var report = new HistoricalPayrollReportDto
            {
                Items = new List<HistoricalPayrollReportItemDto>(),
                Totals = new HistoricalPayrollReportTotalsDto
                {
                    TotalGrossSalary = 0m,
                    TotalMandatoryEmployeeDeductions = 0m,
                    TotalVoluntaryDeductions = 0m,
                    TotalNetSalary = 0m
                }
            };

            var result = _service.GenerateExcelReport(report);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void GenerateExcelReport_SingleItem_ReturnsValidExcel()
        {
            var report = new HistoricalPayrollReportDto
            {
                Items = new List<HistoricalPayrollReportItemDto>
                {
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 1,
                        ContractType = "Tiempo Completo",
                        Position = "Desarrollador",
                        PaymentDate = new DateTime(2024, 1, 15),
                        GrossSalary = 500000m,
                        MandatoryEmployeeDeductions = 50000m,
                        VoluntaryDeductions = 10000m,
                        NetSalary = 440000m
                    }
                },
                Totals = new HistoricalPayrollReportTotalsDto
                {
                    TotalGrossSalary = 500000m,
                    TotalMandatoryEmployeeDeductions = 50000m,
                    TotalVoluntaryDeductions = 10000m,
                    TotalNetSalary = 440000m
                }
            };

            var result = _service.GenerateExcelReport(report);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void GenerateExcelReport_MultipleItems_IncludesAllData()
        {
            var report = new HistoricalPayrollReportDto
            {
                Items = new List<HistoricalPayrollReportItemDto>
                {
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 1,
                        ContractType = "Tiempo Completo",
                        Position = "Desarrollador",
                        PaymentDate = new DateTime(2024, 1, 15),
                        GrossSalary = 500000m,
                        MandatoryEmployeeDeductions = 50000m,
                        VoluntaryDeductions = 10000m,
                        NetSalary = 440000m
                    },
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 2,
                        ContractType = "Medio Tiempo",
                        Position = "Analista",
                        PaymentDate = new DateTime(2024, 2, 15),
                        GrossSalary = 300000m,
                        MandatoryEmployeeDeductions = 30000m,
                        VoluntaryDeductions = 5000m,
                        NetSalary = 265000m
                    },
                    new HistoricalPayrollReportItemDto
                    {
                        PayrollId = 3,
                        ContractType = "Servicios Profesionales",
                        Position = "Consultor",
                        PaymentDate = new DateTime(2024, 3, 15),
                        GrossSalary = 400000m,
                        MandatoryEmployeeDeductions = 40000m,
                        VoluntaryDeductions = 8000m,
                        NetSalary = 352000m
                    }
                },
                Totals = new HistoricalPayrollReportTotalsDto
                {
                    TotalGrossSalary = 1200000m,
                    TotalMandatoryEmployeeDeductions = 120000m,
                    TotalVoluntaryDeductions = 23000m,
                    TotalNetSalary = 1057000m
                }
            };

            var result = _service.GenerateExcelReport(report);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }
    }
}