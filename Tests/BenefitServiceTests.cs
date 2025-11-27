using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using backend.Constants;

namespace backend.Tests
{
    [TestClass]
    public class BenefitServiceTests
    {
        private Mock<IBenefitRepository> _mockBenefitRepository = null!;
        private Mock<IProjectRepository> _mockProjectRepository = null!;
        private BenefitService _benefitService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockBenefitRepository = new Mock<IBenefitRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _benefitService = new BenefitService(_mockBenefitRepository.Object, _mockProjectRepository.Object);
        }

        // ------------------------------
        // CREATE BENEFIT TESTS
        // ------------------------------

        [TestMethod]
        public async Task CreateBenefitAsync_ValidInput_ReturnsBenefitResponseDto()
        {
            // Arrange
            var createBenefitDto = new CreateBenefitDto
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación",
                Value = null,
                Percentage = 10
            };

            var createdBenefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación",
                Value = null,
                Percentage = 10
            };

            var company = new ProjectResponseDTO { Id = 1, Nombre = "Test Company" };

            _mockProjectRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.ExistsAsync(1, "Vacaciones")).ReturnsAsync(false);
            _mockBenefitRepository.Setup(x => x.CreateAsync(It.IsAny<Benefit>())).ReturnsAsync(createdBenefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _benefitService.CreateBenefitAsync(createBenefitDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CompanyId);
            Assert.AreEqual("Vacaciones", result.Name);
            Assert.AreEqual("Porcentaje", result.CalculationType);
            Assert.AreEqual("Bonificación", result.Type);
            Assert.AreEqual("Test Company", result.CompanyName);
            Assert.AreEqual(10, result.Percentage);
            Assert.IsNull(result.Value);
            Assert.IsFalse(result.IsDeleted);

            _mockProjectRepository.Verify(x => x.ExistsAsync(1), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(1, "Vacaciones"), Times.Once);
            _mockBenefitRepository.Verify(x => x.CreateAsync(It.IsAny<Benefit>()), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_CompanyDoesNotExist_ThrowsArgumentException()
        {
            var dto = new CreateBenefitDto
            {
                CompanyId = 999,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockProjectRepository.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.CreateBenefitAsync(dto));

            Assert.AreEqual("La empresa especificada no existe", ex.Message);

            _mockProjectRepository.Verify(x => x.ExistsAsync(999), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_BenefitAlreadyExists_ThrowsArgumentException()
        {
            var dto = new CreateBenefitDto
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockProjectRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.ExistsAsync(1, "Vacaciones")).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.CreateBenefitAsync(dto));

            Assert.AreEqual("Ya existe un beneficio con este nombre para esta empresa", ex.Message);

            _mockProjectRepository.Verify(x => x.ExistsAsync(1), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(1, "Vacaciones"), Times.Once);
            _mockBenefitRepository.Verify(x => x.CreateAsync(It.IsAny<Benefit>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_TrimsName_TrimsWhitespace()
        {
            var dto = new CreateBenefitDto
            {
                CompanyId = 1,
                Name = "  Vacaciones  ",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            var createdBenefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación",
            };

            var company = new ProjectResponseDTO { Id = 1, Nombre = "Test Company" };

            _mockProjectRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.ExistsAsync(1, "Vacaciones")).ReturnsAsync(false);
            _mockBenefitRepository.Setup(x => x.CreateAsync(It.IsAny<Benefit>())).ReturnsAsync(createdBenefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(company);

            var result = await _benefitService.CreateBenefitAsync(dto);

            Assert.AreEqual("Vacaciones", result.Name);

            _mockBenefitRepository.Verify(
                x => x.CreateAsync(It.Is<Benefit>(b => b.Name == "Vacaciones")),
                Times.Once);
        }

        // ------------------------------
        // GET ALL BENEFITS TESTS
        // ------------------------------

        [TestMethod]
        public async Task GetAllBenefitsAsync_ReturnsAllBenefitsWithCompanyNames()
        {
            var benefits = new List<Benefit>
            {
                new Benefit { CompanyId = 1, Name = "Vacaciones", CalculationType = "Porcentaje", Type = "Bonificación", Percentage = 10 },
                new Benefit { CompanyId = 2, Name = "Seguro", CalculationType = "Monto Fijo", Type = "Prestación", Value = 50000 }
            };

            var company1 = new ProjectResponseDTO { Id = 1, Nombre = "Company 1" };
            var company2 = new ProjectResponseDTO { Id = 2, Nombre = "Company 2" };

            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(benefits);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(company1);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(company2);

            var result = await _benefitService.GetAllBenefitsAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Company 1", result[0].CompanyName);
            Assert.AreEqual("Company 2", result[1].CompanyName);
            Assert.IsFalse(result[0].IsDeleted);
            Assert.IsFalse(result[1].IsDeleted);

            _mockBenefitRepository.Verify(x => x.GetAllAsync(), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [TestMethod]
        public async Task GetAllBenefitsAsync_EmptyList_ReturnsEmptyList()
        {
            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Benefit>());

            var result = await _benefitService.GetAllBenefitsAsync();

            Assert.AreEqual(0, result.Count);
            _mockBenefitRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllBenefitsAsync_CompanyNotFound_ReturnsEmpresaNoEncontrada()
        {
            var benefits = new List<Benefit>
            {
                new Benefit { CompanyId = 999, Name = "Vacaciones", CalculationType = "Porcentaje", Type = "Bonificación" }
            };

            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(benefits);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((ProjectResponseDTO?)null);

            var result = await _benefitService.GetAllBenefitsAsync();

            Assert.AreEqual("Empresa no encontrada", result[0].CompanyName);
        }

        // ------------------------------
        // GET BENEFITS BY COMPANY ID
        // ------------------------------

        [TestMethod]
        public async Task GetBenefitsByCompanyIdAsync_ValidCompanyId_ReturnsBenefitsForCompany()
        {
            var companyId = 1;
            var expectedBenefits = new List<BenefitResponseDto>
            {
                new BenefitResponseDto { CompanyId = 1, Name = "Vacaciones", CompanyName = "Test Company" }
            };

            _mockProjectRepository.Setup(x => x.ExistsAsync(companyId)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.GetBenefitsWithCompanyNameAsync(companyId))
                                  .ReturnsAsync(expectedBenefits);

            var result = await _benefitService.GetBenefitsByCompanyIdAsync(companyId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Vacaciones", result[0].Name);
            Assert.IsFalse(result[0].IsDeleted);

            _mockProjectRepository.Verify(x => x.ExistsAsync(companyId), Times.Once);
            _mockBenefitRepository.Verify(x => x.GetBenefitsWithCompanyNameAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetBenefitsByCompanyIdAsync_CompanyDoesNotExist_ThrowsArgumentException()
        {
            var companyId = 999;
            _mockProjectRepository.Setup(x => x.ExistsAsync(companyId)).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.GetBenefitsByCompanyIdAsync(companyId));

            Assert.AreEqual("La empresa especificada no existe", ex.Message);

            _mockProjectRepository.Verify(x => x.ExistsAsync(companyId), Times.Once);
            _mockBenefitRepository.Verify(x => x.GetBenefitsWithCompanyNameAsync(It.IsAny<int>()), Times.Never);
        }

        // ------------------------------
        // GET BENEFIT BY ID
        // ------------------------------

        [TestMethod]
        public async Task GetBenefitByIdAsync_BenefitExists_ReturnsBenefitResponseDto()
        {
            var companyId = 1;
            var name = "Vacaciones";

            var benefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación",
                Percentage = 10
            };

            var company = new ProjectResponseDTO { Id = 1, Nombre = "Test Company" };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name)).ReturnsAsync(benefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(companyId)).ReturnsAsync(company);

            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            Assert.IsNotNull(result);
            Assert.AreEqual("Vacaciones", result.Name);
            Assert.AreEqual("Test Company", result.CompanyName);
            Assert.IsFalse(result.IsDeleted);

            _mockBenefitRepository.Verify(x => x.GetByIdAsync(companyId, name), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetBenefitByIdAsync_BenefitDoesNotExist_ReturnsNull()
        {
            var companyId = 1;
            var name = "NoExiste";

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name))
                                  .ReturnsAsync((Benefit?)null);

            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            Assert.IsNull(result);
            _mockBenefitRepository.Verify(x => x.GetByIdAsync(companyId, name), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task GetBenefitByIdAsync_CompanyNotFound_ReturnsEmpresaNoEncontrada()
        {
            var companyId = 1;
            var name = "Vacaciones";

            var benefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                IsDeleted = false
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name)).ReturnsAsync(benefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(companyId))
                                  .ReturnsAsync((ProjectResponseDTO?)null);

            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            Assert.AreEqual("Empresa no encontrada", result.CompanyName);
            Assert.IsFalse(result.IsDeleted);

        }


        [TestMethod]
        public async Task GetBenefitByIdAsync_BenefitDeleted_ReturnsNull()
        {
            var benefit = new Benefit { CompanyId = 1, Name = "Vacaciones", IsDeleted = true };
            _mockBenefitRepository.Setup(x => x.GetByIdAsync(1, "Vacaciones")).ReturnsAsync(benefit);

            var result = await _benefitService.GetBenefitByIdAsync(1, "Vacaciones");

            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task DeleteBenefitAsync_WithPayrollAssociations_PerformsLogicalDelete()
        {
            var companyId = 1;
            var benefitName = "Seguro";
            var benefit = new Benefit { CompanyId = companyId, Name = benefitName, IsDeleted = false };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, benefitName)).ReturnsAsync(benefit);
            _mockBenefitRepository.Setup(x => x.HasPayrollAssociationsAsync(companyId, benefitName)).ReturnsAsync(true);

            var result = await _benefitService.DeleteBenefitAsync(companyId, benefitName);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.IsLogicalDeletion);
            Assert.AreEqual(ReturnMessagesConstants.Benefit.LogicalDeletionSuccess, result.Message);

            _mockBenefitRepository.Verify(x => x.RemoveEmployeeAssociationsAsync(companyId, benefitName), Times.Once);
            _mockBenefitRepository.Verify(x => x.MarkBenefitAsDeletedAsync(companyId, benefitName), Times.Once);
            _mockBenefitRepository.Verify(x => x.DeleteBenefitAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteBenefitAsync_WithoutPayrollAssociations_PerformsPhysicalDelete()
        {
            var companyId = 2;
            var benefitName = "Pension";
            var benefit = new Benefit { CompanyId = companyId, Name = benefitName, IsDeleted = false };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, benefitName)).ReturnsAsync(benefit);
            _mockBenefitRepository.Setup(x => x.HasPayrollAssociationsAsync(companyId, benefitName)).ReturnsAsync(false);

            var result = await _benefitService.DeleteBenefitAsync(companyId, benefitName);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result.IsLogicalDeletion);
            Assert.AreEqual(ReturnMessagesConstants.Benefit.PhysicalDeletionSuccess, result.Message);

            _mockBenefitRepository.Verify(x => x.RemoveEmployeeAssociationsAsync(companyId, benefitName), Times.Once);
            _mockBenefitRepository.Verify(x => x.DeleteBenefitAsync(companyId, benefitName), Times.Once);
            _mockBenefitRepository.Verify(x => x.MarkBenefitAsDeletedAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteBenefitAsync_BenefitNotFound_ThrowsArgumentException()
        {
            _mockBenefitRepository.Setup(x => x.GetByIdAsync(1, "Seguro")).ReturnsAsync((Benefit?)null);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _benefitService.DeleteBenefitAsync(1, "Seguro"));
            Assert.AreEqual(ReturnMessagesConstants.Benefit.BenefitNotFound, ex.Message);

            _mockBenefitRepository.Verify(x => x.RemoveEmployeeAssociationsAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        // ------------------------------
        // EXISTS BENEFIT
        // ------------------------------

        [TestMethod]
        public async Task ExistsBenefitAsync_BenefitExists_ReturnsTrue()
        {
            var companyId = 1;
            var name = "Vacaciones";

            _mockBenefitRepository.Setup(x => x.ExistsAsync(companyId, name)).ReturnsAsync(true);

            var result = await _benefitService.ExistsBenefitAsync(companyId, name);

            Assert.IsTrue(result);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(companyId, name), Times.Once);
        }

        [TestMethod]
        public async Task ExistsBenefitAsync_BenefitDoesNotExist_ReturnsFalse()
        {
            var companyId = 1;
            var name = "NoExiste";

            _mockBenefitRepository.Setup(x => x.ExistsAsync(companyId, name)).ReturnsAsync(false);

            var result = await _benefitService.ExistsBenefitAsync(companyId, name);

            Assert.IsFalse(result);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(companyId, name), Times.Once);
        }
    }
}