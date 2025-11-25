using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Tests
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _employeeRepoMock = null!;
        private Mock<IProjectRepository> _projectRepoMock = null!;
        private Mock<IUsuarioRepository> _usuarioRepoMock = null!;
        private Mock<IEmailHelper> _emailHelperMock = null!;
        private Mock<ILogger<EmployeeService>> _loggerMock = null!;
        private EmployeeService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
            _projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            _usuarioRepoMock = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            _emailHelperMock = new Mock<IEmailHelper>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<EmployeeService>>();

            _service = new EmployeeService(
                _employeeRepoMock.Object,
                _projectRepoMock.Object,
                _usuarioRepoMock.Object,
                _emailHelperMock.Object,
                _loggerMock.Object
            );
        }

        // --------------------------------------------------------
        // REGISTER EMPLOYEE TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task RegisterEmployeeAsync_ValidEmployee_ReturnsEmployeeId()
        {
            // Arrange
            var dto = new RegisterEmployeeDto
            {
                Correo = "juan@test.com"
                // No usamos PrimerNombre/PrimerApellido en el servicio,
                // así que no es obligatorio setearlos para el test.
            };

            int expectedEmployeeId = 5;
            string rawToken = "raw_token";
            string hashedToken = "hashed_token";

            _employeeRepoMock.Setup(r => r.RegisterEmployeeAsync(dto))
                             .ReturnsAsync(expectedEmployeeId);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken())
                            .Returns(rawToken);

            _emailHelperMock.Setup(e => e.HashToken(rawToken))
                            .Returns(hashedToken);

            _usuarioRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<Usuario>()))
                            .ReturnsAsync(true);

            // Asumiendo que la firma es Task<bool> SendVerificationLinkAsync(...)
            _emailHelperMock.Setup(e =>
                e.SendVerificationLinkAsync(dto.Correo, rawToken, "Empleado"))
                .ReturnsAsync(true);

            // Act
            var result = await _service.RegisterEmployeeAsync(dto);

            // Assert
            Assert.AreEqual(expectedEmployeeId, result);
        }

        [TestMethod]
        public async Task RegisterEmployeeAsync_RegisterFails_Returns0()
        {
            // Arrange
            var dto = new RegisterEmployeeDto
            {
                Correo = "test@test.com"
            };

            _employeeRepoMock.Setup(r => r.RegisterEmployeeAsync(dto))
                             .ReturnsAsync(0);

            // Act
            var result = await _service.RegisterEmployeeAsync(dto);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task RegisterEmployeeAsync_UserCreationFails_Returns0()
        {
            // Arrange
            var dto = new RegisterEmployeeDto
            {
                Correo = "test@correo.com"
            };

            _employeeRepoMock.Setup(r => r.RegisterEmployeeAsync(dto))
                             .ReturnsAsync(10);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken())
                            .Returns("abc");

            _emailHelperMock.Setup(e => e.HashToken("abc"))
                            .Returns("hashed");

            _usuarioRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<Usuario>()))
                            .ReturnsAsync(false);

            // Act
            var result = await _service.RegisterEmployeeAsync(dto);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task RegisterEmployeeAsync_ExceptionThrown_Returns0()
        {
            // Arrange
            var dto = new RegisterEmployeeDto
            {
                Correo = "test@test.com"
            };

            _employeeRepoMock.Setup(r => r.RegisterEmployeeAsync(dto))
                             .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.RegisterEmployeeAsync(dto);

            // Assert
            Assert.AreEqual(0, result);
        }

        // --------------------------------------------------------
        // VALIDATE CEDULA / EMAIL TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task ValidateCedulaExistsAsync_CedulaExists_ReturnsTrue()
        {
            _employeeRepoMock.Setup(r => r.ValidateCedulaExistsAsync("123"))
                             .ReturnsAsync(true);

            var result = await _service.ValidateCedulaExistsAsync("123");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateEmailExistsAsync_EmailExists_ReturnsTrue()
        {
            _employeeRepoMock.Setup(r => r.ValidateEmailExistsAsync("test@test.com"))
                             .ReturnsAsync(true);

            var result = await _service.ValidateEmailExistsAsync("test@test.com");

            Assert.IsTrue(result);
        }

        // --------------------------------------------------------
        // GET EMPLOYEE BY ID TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task GetEmployeeByIdAsync_ValidId_ReturnsEmployee()
        {
            var employee = new Empleado { IdPersona = 10 };

            _employeeRepoMock.Setup(r => r.GetEmployeeByIdAsync(10))
                             .ReturnsAsync(employee);

            var result = await _service.GetEmployeeByIdAsync(10);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result!.IdPersona);
        }

        [TestMethod]
        public async Task GetEmployeeByIdAsync_InvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await _service.GetEmployeeByIdAsync(0)
            );
        }

        // --------------------------------------------------------
        // GET EMPLOYEE COMPANY ID TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task GetEmployeeCompanyIdAsync_ValidEmployee_ReturnsCompanyId()
        {
            _employeeRepoMock.Setup(r => r.GetEmployeeCompanyIdAsync(5))
                             .ReturnsAsync(20);

            var result = await _service.GetEmployeeCompanyIdAsync(5);

            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public async Task GetEmployeeCompanyIdAsync_InvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await _service.GetEmployeeCompanyIdAsync(-1)
            );
        }

        // --------------------------------------------------------
        // GET EMPLOYEE COMPANY (PROJECT) TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task GetEmployeeCompanyAsync_ValidEmployee_ReturnsProject()
        {
            int employeeId = 1;
            int companyId = 10;

            _employeeRepoMock.Setup(r => r.GetEmployeeCompanyIdAsync(employeeId))
                             .ReturnsAsync(companyId);

            var project = new ProjectResponseDTO { Id = companyId, Nombre = "Empresa XY" };

            _projectRepoMock.Setup(r => r.GetByIdAsync(companyId))
                            .ReturnsAsync(project);

            var result = await _service.GetEmployeeCompanyAsync(employeeId);

            Assert.IsNotNull(result);
            Assert.AreEqual(companyId, result!.Id);
        }

        [TestMethod]
        public async Task GetEmployeeCompanyAsync_NoCompanyFound_ReturnsNull()
        {
            _employeeRepoMock.Setup(r => r.GetEmployeeCompanyIdAsync(1))
                             .ReturnsAsync((int?)null);

            var result = await _service.GetEmployeeCompanyAsync(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetEmployeeCompanyAsync_InvalidEmployeeId_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await _service.GetEmployeeCompanyAsync(0)
            );
        }

        // --------------------------------------------------------
        // GET EMPLOYEES BY COMPANY TESTS
        // --------------------------------------------------------

        [TestMethod]
        public async Task GetEmployeesByCompanyAsync_ValidCompanyId_ReturnsEmployees()
        {
            int companyId = 10;

            var list = new List<EmployeeListDto>
            {
                new EmployeeListDto { Id = 1, NombreCompleto = "Juan Pérez" },
                new EmployeeListDto { Id = 2, NombreCompleto = "Ana López" }
            };

            _employeeRepoMock.Setup(r => r.GetEmployeesByCompanyAsync(companyId))
                             .ReturnsAsync(list);

            var result = await _service.GetEmployeesByCompanyAsync(companyId);

            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual("Juan Pérez", result.Employees[0].NombreCompleto);
        }

        [TestMethod]
        public async Task GetEmployeesByCompanyAsync_InvalidCompanyId_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await _service.GetEmployeesByCompanyAsync(0)
            );
        }
    }
}