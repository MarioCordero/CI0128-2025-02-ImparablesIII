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
    }
}