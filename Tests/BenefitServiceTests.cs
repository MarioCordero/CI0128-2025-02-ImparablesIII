using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;

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

            var company = new Project { Id = 1, Nombre = "Test Company" };
            var createdBenefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación",
                Value = null,
                Percentage = 10
            };

            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(1)).ReturnsAsync(true);
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

            _mockProjectRepository.Verify(x => x.ExistsByIdAsync(1), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(1, "Vacaciones"), Times.Once);
            _mockBenefitRepository.Verify(x => x.CreateAsync(It.IsAny<Benefit>()), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_CompanyDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var createBenefitDto = new CreateBenefitDto
            {
                CompanyId = 999,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(999)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.CreateBenefitAsync(createBenefitDto));

            Assert.AreEqual("La empresa especificada no existe", exception.Message);
            _mockProjectRepository.Verify(x => x.ExistsByIdAsync(999), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_BenefitAlreadyExists_ThrowsArgumentException()
        {
            // Arrange
            var createBenefitDto = new CreateBenefitDto
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(1)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.ExistsAsync(1, "Vacaciones")).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.CreateBenefitAsync(createBenefitDto));

            Assert.AreEqual("Ya existe un beneficio con este nombre para esta empresa", exception.Message);
            _mockProjectRepository.Verify(x => x.ExistsByIdAsync(1), Times.Once);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(1, "Vacaciones"), Times.Once);
            _mockBenefitRepository.Verify(x => x.CreateAsync(It.IsAny<Benefit>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateBenefitAsync_TrimsName_TrimsWhitespace()
        {
            // Arrange
            var createBenefitDto = new CreateBenefitDto
            {
                CompanyId = 1,
                Name = "  Vacaciones  ",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            var company = new Project { Id = 1, Nombre = "Test Company" };
            var createdBenefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(1)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.ExistsAsync(1, "Vacaciones")).ReturnsAsync(false);
            _mockBenefitRepository.Setup(x => x.CreateAsync(It.IsAny<Benefit>())).ReturnsAsync(createdBenefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _benefitService.CreateBenefitAsync(createBenefitDto);

            // Assert
            Assert.AreEqual("Vacaciones", result.Name);
            _mockBenefitRepository.Verify(x => x.CreateAsync(It.Is<Benefit>(b => b.Name == "Vacaciones")), Times.Once);
        }

        [TestMethod]
        public async Task GetAllBenefitsAsync_ReturnsAllBenefitsWithCompanyNames()
        {
            // Arrange
            var benefits = new List<Benefit>
            {
                new Benefit { CompanyId = 1, Name = "Vacaciones", CalculationType = "Porcentaje", Type = "Bonificación", Percentage = 10 },
                new Benefit { CompanyId = 2, Name = "Seguro", CalculationType = "Monto Fijo", Type = "Prestación", Value = 50000 }
            };

            var company1 = new Project { Id = 1, Nombre = "Company 1" };
            var company2 = new Project { Id = 2, Nombre = "Company 2" };

            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(benefits);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(company1);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(company2);

            // Act
            var result = await _benefitService.GetAllBenefitsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            
            var firstBenefit = result.First();
            Assert.AreEqual(1, firstBenefit.CompanyId);
            Assert.AreEqual("Vacaciones", firstBenefit.Name);
            Assert.AreEqual("Company 1", firstBenefit.CompanyName);
            Assert.AreEqual(10, firstBenefit.Percentage);

            var secondBenefit = result.Last();
            Assert.AreEqual(2, secondBenefit.CompanyId);
            Assert.AreEqual("Seguro", secondBenefit.Name);
            Assert.AreEqual("Company 2", secondBenefit.CompanyName);
            Assert.AreEqual(50000, secondBenefit.Value);

            _mockBenefitRepository.Verify(x => x.GetAllAsync(), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [TestMethod]
        public async Task GetAllBenefitsAsync_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Benefit>());

            // Act
            var result = await _benefitService.GetAllBenefitsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
            _mockBenefitRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetAllBenefitsAsync_CompanyNotFound_ReturnsEmpresaNoEncontrada()
        {
            // Arrange
            var benefits = new List<Benefit>
            {
                new Benefit { CompanyId = 999, Name = "Vacaciones", CalculationType = "Porcentaje", Type = "Bonificación" }
            };

            _mockBenefitRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(benefits);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Project?)null);

            // Act
            var result = await _benefitService.GetAllBenefitsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Empresa no encontrada", result.First().CompanyName);
        }

        [TestMethod]
        public async Task GetBenefitsByCompanyIdAsync_ValidCompanyId_ReturnsBenefitsForCompany()
        {
            // Arrange
            var companyId = 1;
            var expectedBenefits = new List<BenefitResponseDto>
            {
                new BenefitResponseDto { CompanyId = 1, Name = "Vacaciones", CompanyName = "Test Company" }
            };

            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(companyId)).ReturnsAsync(true);
            _mockBenefitRepository.Setup(x => x.GetBenefitsWithCompanyNameAsync(companyId)).ReturnsAsync(expectedBenefits);

            // Act
            var result = await _benefitService.GetBenefitsByCompanyIdAsync(companyId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Vacaciones", result.First().Name);
            Assert.AreEqual("Test Company", result.First().CompanyName);

            _mockProjectRepository.Verify(x => x.ExistsByIdAsync(companyId), Times.Once);
            _mockBenefitRepository.Verify(x => x.GetBenefitsWithCompanyNameAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetBenefitsByCompanyIdAsync_CompanyDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var companyId = 999;
            _mockProjectRepository.Setup(x => x.ExistsByIdAsync(companyId)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _benefitService.GetBenefitsByCompanyIdAsync(companyId));

            Assert.AreEqual("La empresa especificada no existe", exception.Message);
            _mockProjectRepository.Verify(x => x.ExistsByIdAsync(companyId), Times.Once);
            _mockBenefitRepository.Verify(x => x.GetBenefitsWithCompanyNameAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task GetBenefitByIdAsync_BenefitExists_ReturnsBenefitResponseDto()
        {
            // Arrange
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
            var company = new Project { Id = 1, Nombre = "Test Company" };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name)).ReturnsAsync(benefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(companyId)).ReturnsAsync(company);

            // Act
            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CompanyId);
            Assert.AreEqual("Vacaciones", result.Name);
            Assert.AreEqual("Porcentaje", result.CalculationType);
            Assert.AreEqual("Bonificación", result.Type);
            Assert.AreEqual("Test Company", result.CompanyName);
            Assert.AreEqual(10, result.Percentage);

            _mockBenefitRepository.Verify(x => x.GetByIdAsync(companyId, name), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetBenefitByIdAsync_BenefitDoesNotExist_ReturnsNull()
        {
            // Arrange
            var companyId = 1;
            var name = "NonExistentBenefit";

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name)).ReturnsAsync((Benefit?)null);

            // Act
            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            // Assert
            Assert.IsNull(result);
            _mockBenefitRepository.Verify(x => x.GetByIdAsync(companyId, name), Times.Once);
            _mockProjectRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task GetBenefitByIdAsync_CompanyNotFound_ReturnsEmpresaNoEncontrada()
        {
            // Arrange
            var companyId = 1;
            var name = "Vacaciones";
            var benefit = new Benefit
            {
                CompanyId = 1,
                Name = "Vacaciones",
                CalculationType = "Porcentaje",
                Type = "Bonificación"
            };

            _mockBenefitRepository.Setup(x => x.GetByIdAsync(companyId, name)).ReturnsAsync(benefit);
            _mockProjectRepository.Setup(x => x.GetByIdAsync(companyId)).ReturnsAsync((Project?)null);

            // Act
            var result = await _benefitService.GetBenefitByIdAsync(companyId, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Empresa no encontrada", result.CompanyName);
        }

        [TestMethod]
        public async Task ExistsBenefitAsync_BenefitExists_ReturnsTrue()
        {
            // Arrange
            var companyId = 1;
            var name = "Vacaciones";

            _mockBenefitRepository.Setup(x => x.ExistsAsync(companyId, name)).ReturnsAsync(true);

            // Act
            var result = await _benefitService.ExistsBenefitAsync(companyId, name);

            // Assert
            Assert.IsTrue(result);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(companyId, name), Times.Once);
        }

        [TestMethod]
        public async Task ExistsBenefitAsync_BenefitDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var companyId = 1;
            var name = "NonExistentBenefit";

            _mockBenefitRepository.Setup(x => x.ExistsAsync(companyId, name)).ReturnsAsync(false);

            // Act
            var result = await _benefitService.ExistsBenefitAsync(companyId, name);

            // Assert
            Assert.IsFalse(result);
            _mockBenefitRepository.Verify(x => x.ExistsAsync(companyId, name), Times.Once);
        }
    }
}
