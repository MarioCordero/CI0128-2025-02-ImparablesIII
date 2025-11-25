using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace backend.Tests
{
    [TestClass]
    public class EmployerServiceTests
    {
        private Mock<IEmployerRepository> _employerRepoMock = null!;
        private Mock<IPersonaRepository> _personaRepoMock = null!;
        private Mock<IUsuarioRepository> _usuarioRepoMock = null!;
        private Mock<IDirectionRepository> _directionRepoMock = null!;
        private Mock<IEmailHelper> _emailHelperMock = null!;
        private Mock<ILogger<EmployerService>> _loggerMock = null!;
        private EmployerService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _employerRepoMock = new Mock<IEmployerRepository>(MockBehavior.Strict);
            _personaRepoMock = new Mock<IPersonaRepository>(MockBehavior.Strict);
            _usuarioRepoMock = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            _directionRepoMock = new Mock<IDirectionRepository>(MockBehavior.Strict);
            _emailHelperMock = new Mock<IEmailHelper>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<EmployerService>>();

            _service = new EmployerService(
                _employerRepoMock.Object,
                _personaRepoMock.Object,
                _usuarioRepoMock.Object,
                _directionRepoMock.Object,
                _emailHelperMock.Object,
                _loggerMock.Object
            );
        }

        // ------------------------------------------------------------
        // REGISTER EMPLOYER TESTS
        // ------------------------------------------------------------

        [TestMethod]
        public async Task RegisterEmployerAsync_EmptyPassword_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto { Password = "" };

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_EmailNotAvailable_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "correo@test.com",
                Password = "123456"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                            .ReturnsAsync(new Persona()); // email ocupado

            _personaRepoMock.Setup(r => r.GetByCedulaAsync(It.IsAny<string>()))
                            .ReturnsAsync((Persona?)null);

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_CedulaNotAvailable_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "new@test.com",
                Cedula = "123",
                Password = "123456"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Persona?)null);
            _personaRepoMock.Setup(r => r.GetByCedulaAsync(dto.Cedula)).ReturnsAsync(new Persona());

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_DireccionCreationFails_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "mail@test.com",
                Cedula = "123",
                Password = "abc",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Persona?)null);
            _personaRepoMock.Setup(r => r.GetByCedulaAsync(dto.Cedula)).ReturnsAsync((Persona?)null);

            _directionRepoMock.Setup(r => r.CreateDireccionAsync(
                dto.Provincia, dto.Canton, dto.Distrito, dto.DireccionParticular
            )).ReturnsAsync(0); // falla

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_PersonaCreationFails_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "mail@test.com",
                Cedula = "123",
                Password = "abc",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1",
                Nombre = "Juan",
                PrimerApellido = "Perez"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Persona?)null);
            _personaRepoMock.Setup(r => r.GetByCedulaAsync(dto.Cedula)).ReturnsAsync((Persona?)null);

            _directionRepoMock.Setup(r => r.CreateDireccionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(5);

            _personaRepoMock.Setup(r => r.CreatePersonaAsync(It.IsAny<Persona>()))
                            .ReturnsAsync(0); // falla

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_UserCreationFails_ReturnsFalse()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "mail@test.com",
                Cedula = "123",
                Password = "abc",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1",
                Nombre = "Juan",
                PrimerApellido = "Perez"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Persona?)null);
            _personaRepoMock.Setup(r => r.GetByCedulaAsync(dto.Cedula)).ReturnsAsync((Persona?)null);

            _directionRepoMock.Setup(r => r.CreateDireccionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(5);

            _personaRepoMock.Setup(r => r.CreatePersonaAsync(It.IsAny<Persona>()))
                            .ReturnsAsync(10);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken()).Returns("raw");
            _emailHelperMock.Setup(e => e.HashToken("raw")).Returns("hash");

            _usuarioRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<Usuario>()))
                            .ReturnsAsync(false);  // falla

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterEmployerAsync_ValidRequest_ReturnsTrue()
        {
            var dto = new SignUpEmployerDto
            {
                Email = "mail@test.com",
                Cedula = "123",
                Password = "abc",
                Provincia = "SJ",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 1",
                Nombre = "Juan",
                PrimerApellido = "Perez"
            };

            _personaRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Persona?)null);
            _personaRepoMock.Setup(r => r.GetByCedulaAsync(dto.Cedula)).ReturnsAsync((Persona?)null);

            _directionRepoMock.Setup(r => r.CreateDireccionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(5);

            _personaRepoMock.Setup(r => r.CreatePersonaAsync(It.IsAny<Persona>()))
                            .ReturnsAsync(10);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken()).Returns("raw");
            _emailHelperMock.Setup(e => e.HashToken("raw")).Returns("hash");

            _usuarioRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<Usuario>()))
                            .ReturnsAsync(true);

            _emailHelperMock.Setup(e => e.SendVerificationLinkAsync(dto.Email, "raw", "Empleador"))
                            .ReturnsAsync(true);

            var result = await _service.RegisterEmployerAsync(dto);

            Assert.IsTrue(result);
        }

        // ------------------------------------------------------------
        // VERIFY & CREATE USER TESTS
        // ------------------------------------------------------------

        [TestMethod]
        public async Task VerifyAndCreateUserAsync_PersonNotFound_ReturnsFalse()
        {
            _personaRepoMock.Setup(r => r.GetByIdAsync(5))
                            .ReturnsAsync((Persona?)null);

            var result = await _service.VerifyAndCreateUserAsync(5, "pass");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task VerifyAndCreateUserAsync_ExistingUserNotVerified_UpdatesAndReturnsTrue()
        {
            var persona = new Persona { Id = 5, Rol = "Empleador" };
            var usuario = new Usuario { IdPersona = 5, IsVerified = false };

            _personaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(usuario);
            _usuarioRepoMock.Setup(r => r.UpdateAsync(usuario)).ReturnsAsync(true);

            var result = await _service.VerifyAndCreateUserAsync(5, "pass");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task VerifyAndCreateUserAsync_ExistingUserAlreadyVerified_ReturnsTrue()
        {
            var persona = new Persona { Id = 5, Rol = "Empleador" };
            var usuario = new Usuario { IdPersona = 5, IsVerified = true };

            _personaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(usuario);

            var result = await _service.VerifyAndCreateUserAsync(5, "pass");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task VerifyAndCreateUserAsync_NoExistingUser_CreatesUser()
        {
            var persona = new Persona { Id = 5, Rol = "Empleador" };

            _personaRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync((Usuario?)null);
            _usuarioRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<Usuario>()))
                            .ReturnsAsync(true);

            var result = await _service.VerifyAndCreateUserAsync(5, "pass");

            Assert.IsTrue(result);
        }

        // ------------------------------------------------------------
        // RESEND VERIFICATION TESTS
        // ------------------------------------------------------------

        [TestMethod]
        public async Task ResendVerificationAsync_PersonNotFound_ReturnsFalse()
        {
            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail"))
                            .ReturnsAsync((Persona?)null);

            var result = await _service.ResendVerificationAsync("mail");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ResendVerificationAsync_UserNotFound_ReturnsFalse()
        {
            var persona = new Persona { Id = 5 };

            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail")).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync((Usuario?)null);

            var result = await _service.ResendVerificationAsync("mail");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ResendVerificationAsync_UserAlreadyVerified_ReturnsFalse()
        {
            var persona = new Persona { Id = 5 };
            var usuario = new Usuario { IdPersona = 5, IsVerified = true };

            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail")).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(usuario);

            var result = await _service.ResendVerificationAsync("mail");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ResendVerificationAsync_UpdateFails_ReturnsFalse()
        {
            var persona = new Persona { Id = 5 };
            var usuario = new Usuario { IdPersona = 5, IsVerified = false };

            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail")).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(usuario);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken()).Returns("raw");
            _emailHelperMock.Setup(e => e.HashToken("raw")).Returns("hash");

            _usuarioRepoMock.Setup(r => r.UpdateAsync(usuario)).ReturnsAsync(false);

            var result = await _service.ResendVerificationAsync("mail");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ResendVerificationAsync_Success_ReturnsTrue()
        {
            var persona = new Persona { Id = 5 };
            var usuario = new Usuario { IdPersona = 5, IsVerified = false };

            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail")).ReturnsAsync(persona);
            _usuarioRepoMock.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(usuario);

            _emailHelperMock.Setup(e => e.GenerateVerificationToken()).Returns("raw");
            _emailHelperMock.Setup(e => e.HashToken("raw")).Returns("hash");

            _usuarioRepoMock.Setup(r => r.UpdateAsync(usuario)).ReturnsAsync(true);

            _emailHelperMock.Setup(e => e.SendVerificationLinkAsync("mail", "raw", "Empleador"))
                            .ReturnsAsync(true);

            var result = await _service.ResendVerificationAsync("mail");

            Assert.IsTrue(result);
        }

        // ------------------------------------------------------------
        // VALIDATION HELPERS
        // ------------------------------------------------------------

        [TestMethod]
        public async Task IsEmailAvailableAsync_EmailNotFound_ReturnsTrue()
        {
            _personaRepoMock.Setup(r => r.GetByEmailAsync("mail"))
                            .ReturnsAsync((Persona?)null);

            var result = await _service.IsEmailAvailableAsync("mail");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsCedulaAvailableAsync_CedulaNotFound_ReturnsTrue()
        {
            _personaRepoMock.Setup(r => r.GetByCedulaAsync("123"))
                            .ReturnsAsync((Persona?)null);

            var result = await _service.IsCedulaAvailableAsync("123");

            Assert.IsTrue(result);
        }
    }
}