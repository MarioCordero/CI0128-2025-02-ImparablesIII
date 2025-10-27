using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;

namespace backend.Tests
{
    [TestClass]
    public class EmployeeBenefitServiceTests
    {
        private Mock<IEmployeeBenefitRepository> _mockEmployeeBenefitRepository = null!;
        private Mock<IBenefitRepository> _mockBenefitRepository = null!;
        private Mock<ILogger<EmployeeBenefitService>> _mockLogger = null!;
        private EmployeeBenefitService _employeeBenefitService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockEmployeeBenefitRepository = new Mock<IEmployeeBenefitRepository>();
            _mockBenefitRepository = new Mock<IBenefitRepository>();
            _mockLogger = new Mock<ILogger<EmployeeBenefitService>>();
            _employeeBenefitService = new EmployeeBenefitService(
                _mockEmployeeBenefitRepository.Object,
                _mockBenefitRepository.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetEmployeeBenefitsAsync_ValidRequest_ReturnsSummary()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var expectedSummary = new EmployeeBenefitsSummaryDto
            {
                AvailableBenefits = new List<EmployeeBenefitDto>
                {
                    new EmployeeBenefitDto { BenefitName = "Vacaciones", IsSelected = false }
                },
                SelectedBenefits = new List<EmployeeBenefitDto>
                {
                    new EmployeeBenefitDto { BenefitName = "Seguro", IsSelected = true }
                },
                CurrentSelections = 1,
                MaxSelections = 5
            };

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, null))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _employeeBenefitService.GetEmployeeBenefitsAsync(employeeId, companyId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CurrentSelections);
            Assert.AreEqual(5, result.MaxSelections);
            Assert.AreEqual(1, result.AvailableBenefits.Count);
            Assert.AreEqual(1, result.SelectedBenefits.Count);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, null), Times.Once);
        }

        [TestMethod]
        public async Task GetEmployeeBenefitsAsync_WithFilter_AppliesFilter()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var filter = new BenefitFilterDto { SearchTerm = "vacaciones" };
            var expectedSummary = new EmployeeBenefitsSummaryDto
            {
                AvailableBenefits = new List<EmployeeBenefitDto>(),
                SelectedBenefits = new List<EmployeeBenefitDto>(),
                CurrentSelections = 0,
                MaxSelections = 5
            };

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, filter))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _employeeBenefitService.GetEmployeeBenefitsAsync(employeeId, companyId, filter);

            // Assert
            Assert.IsNotNull(result);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, filter), Times.Once);
        }

        [TestMethod]
        public async Task GetEmployeeBenefitsAsync_RepositoryThrows_PropagatesException()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetEmployeeBenefitsSummaryAsync(employeeId, companyId, null))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _employeeBenefitService.GetEmployeeBenefitsAsync(employeeId, companyId));
        }

        [TestMethod]
        public async Task SelectBenefitAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "Vacaciones" };
            var benefit = new Benefit 
            { 
                CompanyId = companyId, 
                Name = "Vacaciones", 
                CalculationType = "Porcentaje" 
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ReturnsAsync(benefit);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.IsBenefitSelectedAsync(employeeId, companyId, request.BenefitName))
                .ReturnsAsync(false);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(2);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.AddBenefitToEmployeeAsync(employeeId, companyId, request.BenefitName, benefit.CalculationType))
                .ReturnsAsync((true, "Success"));

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Beneficio agregado exitosamente", result.Message);
            _mockBenefitRepository.Verify(x => x.GetByIdAsync(companyId, request.BenefitName), Times.AtLeastOnce);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.AddBenefitToEmployeeAsync(employeeId, companyId, request.BenefitName, benefit.CalculationType), Times.Once);
        }

        [TestMethod]
        public async Task SelectBenefitAsync_BenefitNotFound_ReturnsError()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "NonExistentBenefit" };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ReturnsAsync((Benefit?)null);

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("El beneficio no existe", result.Message);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.AddBenefitToEmployeeAsync(It.IsAny<int>(), It.IsAny<int>(), 
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task SelectBenefitAsync_BenefitAlreadySelected_ReturnsError()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "Seguro" };
            var benefit = new Benefit 
            { 
                CompanyId = companyId, 
                Name = "Seguro", 
                CalculationType = "Monto Fijo" 
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ReturnsAsync(benefit);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.IsBenefitSelectedAsync(employeeId, companyId, request.BenefitName))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("El beneficio ya está agregado", result.Message);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.AddBenefitToEmployeeAsync(It.IsAny<int>(), It.IsAny<int>(), 
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task SelectBenefitAsync_MaxSelectionsReached_ReturnsError()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "Vacaciones" };
            var benefit = new Benefit 
            { 
                CompanyId = companyId, 
                Name = "Vacaciones", 
                CalculationType = "Porcentaje" 
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ReturnsAsync(benefit);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.IsBenefitSelectedAsync(employeeId, companyId, request.BenefitName))
                .ReturnsAsync(false);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(5);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("límite máximo"));
            Assert.AreEqual(5, result.CurrentSelections);
            Assert.AreEqual(5, result.MaxSelections);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.AddBenefitToEmployeeAsync(It.IsAny<int>(), It.IsAny<int>(), 
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task SelectBenefitAsync_RepositoryAddFails_ReturnsError()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "Vacaciones" };
            var benefit = new Benefit 
            { 
                CompanyId = companyId, 
                Name = "Vacaciones", 
                CalculationType = "Porcentaje" 
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ReturnsAsync(benefit);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.IsBenefitSelectedAsync(employeeId, companyId, request.BenefitName))
                .ReturnsAsync(false);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(2);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.AddBenefitToEmployeeAsync(employeeId, companyId, request.BenefitName, benefit.CalculationType))
                .ReturnsAsync((false, "Error adding benefit"));

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Error adding benefit", result.Message);
        }

        [TestMethod]
        public async Task SelectBenefitAsync_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;
            var request = new SelectBenefitRequestDto { BenefitName = "Vacaciones" };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, request.BenefitName))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("Error al seleccionar el beneficio"));
        }

        [TestMethod]
        public async Task ValidateBenefitSelectionAsync_UnderLimit_ReturnsTrue()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(2);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);

            // Act
            var result = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId);

            // Assert
            Assert.IsTrue(result);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId), Times.Once);
            _mockEmployeeBenefitRepository.Verify(x => 
                x.GetMaxBenefitLimitAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task ValidateBenefitSelectionAsync_AtLimit_ReturnsFalse()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(5);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);

            // Act
            var result = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateBenefitSelectionAsync_OverLimit_ReturnsFalse()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(6);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(5);

            // Act
            var result = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateBenefitSelectionAsync_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateBenefitSelectionAsync_NoSelections_MaxZero_ReturnsTrue()
        {
            // Arrange
            var employeeId = 1;
            var companyId = 1;

            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetSelectedBenefitsCountAsync(employeeId, companyId))
                .ReturnsAsync(0);
            _mockEmployeeBenefitRepository.Setup(x => 
                x.GetMaxBenefitLimitAsync(companyId))
                .ReturnsAsync(3);

            // Act
            var result = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId);

            // Assert
            Assert.IsTrue(result);
        }
    }
}

