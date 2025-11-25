using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace backend.Tests
{
    [TestClass]
    public class ProfileEmployeeServiceTests
    {
        private Mock<IProfileEmployeeRepository> _profileRepoMock = null!;
        private Mock<ILogger<ProfileEmployeeService>> _loggerMock = null!;
        private ProfileEmployeeService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _profileRepoMock = new Mock<IProfileEmployeeRepository>();
            _loggerMock = new Mock<ILogger<ProfileEmployeeService>>();

            _service = new ProfileEmployeeService(
                _profileRepoMock.Object,
                _loggerMock.Object
            );
        }

        // ------------------------------------------------------------------
        // GetEmployeeProfileAsync Tests
        // ------------------------------------------------------------------

        [TestMethod]
        public async Task GetEmployeeProfileAsync_InvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _service.GetEmployeeProfileAsync(0));
        }

        [TestMethod]
        public async Task GetEmployeeProfileAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            int employeeId = 5;

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ReturnsAsync(false);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetEmployeeProfileAsync(employeeId));
        }

        [TestMethod]
        public async Task GetEmployeeProfileAsync_ValidId_ReturnsProfile()
        {
            int employeeId = 5;

            var expectedProfile = new ProfileEmployeeResponseDto
            {
                User = new UserProfileDto
                {
                    Nombre = "Juan"
                }
            };

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ReturnsAsync(true);
            _profileRepoMock.Setup(r => r.GetProfileByIdAsync(employeeId))
                            .ReturnsAsync(expectedProfile);

            var result = await _service.GetEmployeeProfileAsync(employeeId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);
            Assert.AreEqual("Juan", result.User.Nombre);
        }

        // ------------------------------------------------------------------
        // UpdateEmployeeProfileAsync Tests
        // ------------------------------------------------------------------

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_InvalidId_ReturnsErrorResponse()
        {
            var updateDto = new UpdateEmployeeProfileRequestDto();

            var result = await _service.UpdateEmployeeProfileAsync(0, updateDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("El ID del empleado debe ser mayor a 0", result.Message);
        }

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_MissingRequiredFields_ReturnsErrorResponse()
        {
            var updateDto = new UpdateEmployeeProfileRequestDto
            {
                Nombre = "", // requerido pero vacío
                Apellidos = "Test",
                Provincia = "Prov",
                Canton = "Cant",
                Distrito = "Dist",
                DireccionParticular = "Dir"
            };

            var result = await _service.UpdateEmployeeProfileAsync(1, updateDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Todos los campos son requeridos", result.Message);
        }

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_EmployeeNotFound_ReturnsErrorResponse()
        {
            int employeeId = 5;

            var updateDto = new UpdateEmployeeProfileRequestDto
            {
                Nombre = "Juan",
                Apellidos = "Pérez",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1"
            };

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ReturnsAsync(false);

            var result = await _service.UpdateEmployeeProfileAsync(employeeId, updateDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("No se encontró el empleado con ID 5", result.Message);
        }

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_UpdateFails_ReturnsErrorResponse()
        {
            int employeeId = 5;

            var updateDto = new UpdateEmployeeProfileRequestDto
            {
                Nombre = "Juan",
                Apellidos = "Pérez",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1"
            };

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ReturnsAsync(true);

            _profileRepoMock.Setup(r => r.UpdateEmployeeProfileAsync(employeeId, updateDto))
                            .ReturnsAsync(false);

            var result = await _service.UpdateEmployeeProfileAsync(employeeId, updateDto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("No se pudo actualizar el perfil", result.Message);
        }

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_ValidUpdate_ReturnsSuccess()
        {
            int employeeId = 5;

            var updateDto = new UpdateEmployeeProfileRequestDto
            {
                Nombre = "Juan",
                Apellidos = "Pérez",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1"
            };

            var updatedProfile = new ProfileEmployeeResponseDto
            {
                User = new UserProfileDto
                {
                    Nombre = "Juan"
                }
            };

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ReturnsAsync(true);

            _profileRepoMock.Setup(r => r.UpdateEmployeeProfileAsync(employeeId, updateDto))
                            .ReturnsAsync(true);

            _profileRepoMock.Setup(r => r.GetProfileByIdAsync(employeeId))
                            .ReturnsAsync(updatedProfile);

            var result = await _service.UpdateEmployeeProfileAsync(employeeId, updateDto);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Perfil actualizado exitosamente", result.Message);
            Assert.IsNotNull(result.UpdatedProfile);
            Assert.AreEqual("Juan", result.UpdatedProfile!.Nombre);
        }

        [TestMethod]
        public async Task UpdateEmployeeProfileAsync_ExceptionThrown_ReturnsServerError()
        {
            int employeeId = 5;

            var updateDto = new UpdateEmployeeProfileRequestDto
            {
                Nombre = "Juan",
                Apellidos = "Pérez",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1"
            };

            _profileRepoMock.Setup(r => r.ExistsAsync(employeeId))
                            .ThrowsAsync(new Exception("DB crash"));

            var result = await _service.UpdateEmployeeProfileAsync(employeeId, updateDto);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("Error interno del servidor"));
        }
    }
}