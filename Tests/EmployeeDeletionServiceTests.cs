using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using backend.Constants;
using Microsoft.Extensions.Logging;

namespace backend.Tests
{
    [TestClass]
    public class EmployeeDeletionServiceTests
    {
        private Mock<IEmployeeRepository> _mockEmployeeRepository = null!;
        private Mock<IUsuarioRepository> _mockUsuarioRepository = null!;
        private Mock<ILogger<EmployeeDeletionService>> _mockLogger = null!;
        private EmployeeDeletionService _employeeDeletionService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockLogger = new Mock<ILogger<EmployeeDeletionService>>();
            _employeeDeletionService = new EmployeeDeletionService(
                _mockEmployeeRepository.Object,
                _mockUsuarioRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_ValidRequestWithPayrollRecords_PerformsLogicalDeletion()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword",
                MotivoBaja = "Renuncia voluntaria"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            var payrollCount = 5;

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);
            _mockEmployeeRepository
                .Setup(x => x.HasPayrollRecordsAsync(employeeId))
                .ReturnsAsync(true);
            _mockEmployeeRepository
                .Setup(x => x.GetPayrollRecordsCountAsync(employeeId))
                .ReturnsAsync(payrollCount);
            _mockEmployeeRepository
                .Setup(x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, request.MotivoBaja))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.IsLogicalDeletion);
            Assert.AreEqual(payrollCount, result.PayrollRecordsCount);
            Assert.IsTrue(result.Message.Contains(payrollCount.ToString()));

            _mockEmployeeRepository.Verify(x => x.GetEmployeeByIdAsync(employeeId), Times.Once);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
            _mockEmployeeRepository.Verify(x => x.HasPayrollRecordsAsync(employeeId), Times.Once);
            _mockEmployeeRepository.Verify(x => x.GetPayrollRecordsCountAsync(employeeId), Times.Once);
            _mockEmployeeRepository.Verify(
                x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, request.MotivoBaja),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_ValidRequestWithoutPayrollRecords_PerformsPhysicalDeletion()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword",
                MotivoBaja = "Renuncia voluntaria"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);
            _mockEmployeeRepository
                .Setup(x => x.HasPayrollRecordsAsync(employeeId))
                .ReturnsAsync(false);
            _mockEmployeeRepository
                .Setup(x => x.DeleteEmployeePhysicallyAsync(employeeId))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsFalse(result.IsLogicalDeletion);
            Assert.AreEqual(0, result.PayrollRecordsCount);
            Assert.IsTrue(result.Message.Contains(ReturnMessagesConstants.Employee.PhysicalDeletionSuccess));

            _mockEmployeeRepository.Verify(x => x.GetEmployeeByIdAsync(employeeId), Times.Once);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
            _mockEmployeeRepository.Verify(x => x.HasPayrollRecordsAsync(employeeId), Times.Once);
            _mockEmployeeRepository.Verify(x => x.DeleteEmployeePhysicallyAsync(employeeId), Times.Once);
            _mockEmployeeRepository.Verify(
                x => x.DeleteEmployeeLogicallyAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_EmployeeNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 999;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword"
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync((Empleado?)null);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ReturnMessagesConstants.Employee.EmployeeNotFound, result.Message);

            _mockEmployeeRepository.Verify(x => x.GetEmployeeByIdAsync(employeeId), Times.Once);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_InvalidPassword_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "wrongPassword"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ReturnMessagesConstants.Employee.InvalidPassword, result.Message);

            _mockEmployeeRepository.Verify(x => x.GetEmployeeByIdAsync(employeeId), Times.Once);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
            _mockEmployeeRepository.Verify(x => x.HasPayrollRecordsAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_EmployerNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 999;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ReturnMessagesConstants.Employee.InvalidPassword, result.Message);

            _mockEmployeeRepository.Verify(x => x.GetEmployeeByIdAsync(employeeId), Times.Once);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_LogicalDeletionFails_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword",
                MotivoBaja = "Renuncia voluntaria"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            var payrollCount = 3;

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);
            _mockEmployeeRepository
                .Setup(x => x.HasPayrollRecordsAsync(employeeId))
                .ReturnsAsync(true);
            _mockEmployeeRepository
                .Setup(x => x.GetPayrollRecordsCountAsync(employeeId))
                .ReturnsAsync(payrollCount);
            _mockEmployeeRepository
                .Setup(x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, request.MotivoBaja))
                .ReturnsAsync(false);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ReturnMessagesConstants.Employee.DeletionError, result.Message);

            _mockEmployeeRepository.Verify(x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, request.MotivoBaja), Times.Once);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_PhysicalDeletionFails_ReturnsErrorResponse()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword"
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);
            _mockEmployeeRepository
                .Setup(x => x.HasPayrollRecordsAsync(employeeId))
                .ReturnsAsync(false);
            _mockEmployeeRepository
                .Setup(x => x.DeleteEmployeePhysicallyAsync(employeeId))
                .ReturnsAsync(false);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ReturnMessagesConstants.Employee.DeletionError, result.Message);

            _mockEmployeeRepository.Verify(x => x.DeleteEmployeePhysicallyAsync(employeeId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteEmployeeAsync_NullMotivoBaja_UsesDefaultReason()
        {
            // Arrange
            var employeeId = 1;
            var employerId = 2;
            var request = new DeleteEmployeeRequestDto
            {
                Contrasena = "correctPassword",
                MotivoBaja = null
            };

            var employee = new Empleado
            {
                IdPersona = employeeId,
                Estado = EmployeeConstants.StatusActive
            };

            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = "correctPassword",
                TipoUsuario = "Empleador"
            };

            var payrollCount = 2;

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);
            _mockEmployeeRepository
                .Setup(x => x.HasPayrollRecordsAsync(employeeId))
                .ReturnsAsync(true);
            _mockEmployeeRepository
                .Setup(x => x.GetPayrollRecordsCountAsync(employeeId))
                .ReturnsAsync(payrollCount);
            _mockEmployeeRepository
                .Setup(x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, EmployeeConstants.DefaultDeletionReason))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);

            _mockEmployeeRepository.Verify(
                x => x.DeleteEmployeeLogicallyAsync(employeeId, employerId, EmployeeConstants.DefaultDeletionReason),
                Times.Once);
        }

        [TestMethod]
        public async Task GetEmployeeDeletionInfoAsync_ValidEmployeeId_ReturnsDeletionInfo()
        {
            // Arrange
            var employeeId = 1;
            var expectedInfo = new EmployeeDeletionInfoDto
            {
                HasPayrollRecords = true,
                PayrollRecordsCount = 5,
                EmployeeName = "Juan Pérez"
            };

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeDeletionInfoAsync(employeeId))
                .ReturnsAsync(expectedInfo);

            // Act
            var result = await _employeeDeletionService.GetEmployeeDeletionInfoAsync(employeeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedInfo.HasPayrollRecords, result.HasPayrollRecords);
            Assert.AreEqual(expectedInfo.PayrollRecordsCount, result.PayrollRecordsCount);
            Assert.AreEqual(expectedInfo.EmployeeName, result.EmployeeName);

            _mockEmployeeRepository.Verify(x => x.GetEmployeeDeletionInfoAsync(employeeId), Times.Once);
        }

        [TestMethod]
        public async Task GetEmployeeDeletionInfoAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var employeeId = 999;

            _mockEmployeeRepository
                .Setup(x => x.GetEmployeeDeletionInfoAsync(employeeId))
                .ThrowsAsync(new KeyNotFoundException($"Employee with ID {employeeId} not found"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _employeeDeletionService.GetEmployeeDeletionInfoAsync(employeeId));

            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.Message.Contains(employeeId.ToString()));

            _mockEmployeeRepository.Verify(x => x.GetEmployeeDeletionInfoAsync(employeeId), Times.Once);
        }

        [TestMethod]
        public async Task ValidateEmployerPasswordAsync_ValidPassword_ReturnsTrue()
        {
            // Arrange
            var employerId = 2;
            var password = "correctPassword";
            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = password,
                TipoUsuario = "Empleador"
            };

            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);

            // Act
            var result = await _employeeDeletionService.ValidateEmployerPasswordAsync(employerId, password);

            // Assert
            Assert.IsTrue(result);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
        }

        [TestMethod]
        public async Task ValidateEmployerPasswordAsync_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var employerId = 2;
            var correctPassword = "correctPassword";
            var wrongPassword = "wrongPassword";
            var employer = new Usuario
            {
                IdPersona = employerId,
                Contrasena = correctPassword,
                TipoUsuario = "Empleador"
            };

            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync(employer);

            // Act
            var result = await _employeeDeletionService.ValidateEmployerPasswordAsync(employerId, wrongPassword);

            // Assert
            Assert.IsFalse(result);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
        }

        [TestMethod]
        public async Task ValidateEmployerPasswordAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var employerId = 999;
            var password = "anyPassword";

            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _employeeDeletionService.ValidateEmployerPasswordAsync(employerId, password);

            // Assert
            Assert.IsFalse(result);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
        }

        [TestMethod]
        public async Task ValidateEmployerPasswordAsync_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var employerId = 2;
            var password = "anyPassword";

            _mockUsuarioRepository
                .Setup(x => x.GetUserByIdAsync(employerId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _employeeDeletionService.ValidateEmployerPasswordAsync(employerId, password);

            // Assert
            Assert.IsFalse(result);
            _mockUsuarioRepository.Verify(x => x.GetUserByIdAsync(employerId), Times.Once);
        }
    }
}

