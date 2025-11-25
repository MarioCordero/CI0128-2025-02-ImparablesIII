using System;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace backend.Tests
{
    [TestClass]
    public class PasswordSetupServiceTests
    {
        private IMemoryCache _cache = null!;
        private Mock<ILogger<PasswordSetupService>> _loggerMock = null!;
        private Mock<IEmployeeRepository> _employeeRepoMock = null!;
        private Mock<IPasswordRepository> _passwordRepoMock = null!;
        private Mock<IUsuarioRepository> _usuarioRepoMock = null!;
        private Mock<IEmailVerificationService> _emailVerificationMock = null!;
        private PasswordSetupService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // Cache real, sin Moq
            _cache = new MemoryCache(new MemoryCacheOptions());

            _loggerMock = new Mock<ILogger<PasswordSetupService>>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _passwordRepoMock = new Mock<IPasswordRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _emailVerificationMock = new Mock<IEmailVerificationService>();

            _service = new PasswordSetupService(
                _cache,
                _loggerMock.Object,
                _employeeRepoMock.Object,
                _passwordRepoMock.Object,
                _usuarioRepoMock.Object,
                _emailVerificationMock.Object
            );
        }

        // ----------------- GeneratePasswordSetupTokenAsync -----------------

        [TestMethod]
        public async Task GeneratePasswordSetupTokenAsync_StoresTokenInCache_ReturnsToken()
        {
            var personaId = 5;
            var email = "user@example.com";

            var token = await _service.GeneratePasswordSetupTokenAsync(personaId, email);

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            Assert.IsTrue(token.Length > 10);

            var cacheKey = $"password_setup_{token}";
            var exists = _cache.TryGetValue(cacheKey, out var cached);
            Assert.IsTrue(exists);
            Assert.IsNotNull(cached);
        }

        // ----------------- ValidateTokenAsync -----------------

        [TestMethod]
        public async Task ValidateTokenAsync_ValidToken_ReturnsTrue()
        {
            var token = "abc123";
            var cacheKey = $"password_setup_{token}";
            _cache.Set(cacheKey, new { PersonaId = 1, Email = "x@y.com" });

            var result = await _service.ValidateTokenAsync(token);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateTokenAsync_InvalidToken_ReturnsFalse()
        {
            var result = await _service.ValidateTokenAsync("nonexistent");
            Assert.IsFalse(result);
        }

        // ----------------- SetupPasswordAsync: validaciones básicas -----------------

        [TestMethod]
        public async Task SetupPasswordAsync_RequestNull_ReturnsError()
        {
            var result = await _service.SetupPasswordAsync(null!);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Request inválido", result.Message);
        }

        [TestMethod]
        public async Task SetupPasswordAsync_TokenNullOrEmpty_ReturnsError()
        {
            var request = new PasswordSetupRequestDto
            {
                Token = "",
                Password = "12345"
            };

            var result = await _service.SetupPasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Token es requerido", result.Message);
        }

        [TestMethod]
        public async Task SetupPasswordAsync_InvalidVerificationToken_ReturnsError()
        {
            var request = new PasswordSetupRequestDto
            {
                Token = "invalid",
                Password = "StrongPass123"
            };

            _emailVerificationMock
                .Setup(s => s.VerifyLinkTokenAsync(request.Token))
                .ReturnsAsync((false, 0, string.Empty));

            var result = await _service.SetupPasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Token inválido o expirado", result.Message);
        }

        // ----------------- SetupPasswordAsync: actualización de contraseña -----------------

        [TestMethod]
        public async Task SetupPasswordAsync_UpdatePasswordFails_ReturnsError()
        {
            var request = new PasswordSetupRequestDto
            {
                Token = "valid",
                Password = "StrongPass123"
            };

            _emailVerificationMock
                .Setup(s => s.VerifyLinkTokenAsync(request.Token))
                .ReturnsAsync((true, 10, "Empleado"));

            _passwordRepoMock
                .Setup(r => r.UpdateEmployeePasswordAsync(10, It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _service.SetupPasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Error al actualizar la contraseña", result.Message);
        }

        [TestMethod]
        public async Task SetupPasswordAsync_ValidFlow_ReturnsSuccess()
        {
            var request = new PasswordSetupRequestDto
            {
                Token = "validToken",
                Password = "NewPassword123"
            };

            _emailVerificationMock
                .Setup(s => s.VerifyLinkTokenAsync(request.Token))
                .ReturnsAsync((true, 20, "Empleado"));

            _passwordRepoMock
                .Setup(r => r.UpdateEmployeePasswordAsync(20, It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _service.SetupPasswordAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Contraseña configurada exitosamente", result.Message);
        }

        // ----------------- SetupPasswordAsync: excepciones -----------------

        [TestMethod]
        public async Task SetupPasswordAsync_ExceptionThrown_ReturnsServerError()
        {
            var request = new PasswordSetupRequestDto
            {
                Token = "boom",
                Password = "12345"
            };

            _emailVerificationMock
                .Setup(s => s.VerifyLinkTokenAsync(request.Token))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.SetupPasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Error interno del servidor", result.Message);
        }
    }
}