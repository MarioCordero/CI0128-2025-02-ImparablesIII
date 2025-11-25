using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace backend.Tests
{
    [TestClass]
    public class EmailVerificationServiceTests
    {
        private Mock<IUsuarioRepository> _usuarioRepoMock = null!;
        private Mock<IEmailService> _emailServiceMock = null!;
        private Mock<IEmailTemplates> _emailTemplatesMock = null!;
        private Mock<ILogger<EmailVerificationService>> _loggerMock = null!;
        private EmailVerificationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            _emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            _emailTemplatesMock = new Mock<IEmailTemplates>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<EmailVerificationService>>();

            _service = new EmailVerificationService(
                _usuarioRepoMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object,
                _emailTemplatesMock.Object
            );

            // Limpia el diccionario estático antes de cada test
            ClearVerificationTokens();
        }

        // ----------------- Helpers para el diccionario estático -----------------

        private Dictionary<string, (string token, DateTime expiry, int personaId)> GetVerificationTokens()
        {
            var field = typeof(EmailVerificationService).GetField(
                "_verificationTokens",
                BindingFlags.NonPublic | BindingFlags.Static
            );

            return (Dictionary<string, (string token, DateTime expiry, int personaId)>)field!.GetValue(null)!;
        }

        private void ClearVerificationTokens()
        {
            var dict = GetVerificationTokens();
            dict.Clear();
        }

        private void SetVerificationToken(string email, string token, DateTime expiry, int personaId)
        {
            var dict = GetVerificationTokens();
            dict[email] = (token, expiry, personaId);
        }

        // ----------------- SendVerificationEmailAsync -----------------

        [TestMethod]
        public async Task SendVerificationEmailAsync_Success_ReturnsTrue()
        {
            // Arrange
            var dto = new SendVerificationEmailDto
            {
                Email = "test@example.com",
                Nombre = "Juan",
                PersonaId = 10,
                Rol = "Empleado"
            };

            _emailTemplatesMock
                .Setup(t => t.GetVerificationTemplate(
                    dto.Nombre,
                    It.IsAny<string>(),
                    dto.Rol))
                .Returns("<html>contenido</html>");

            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.Is<SendEmailDto>(e =>
                    e.ReceiverEmail == dto.Email &&
                    e.IsHtml &&
                    e.Subject.Contains(dto.Rol))))
                .ReturnsAsync(new EmailResponseDto
                {
                    Success = true,
                    Message = "OK",
                    SentAt = DateTime.UtcNow
                });

            // Act
            var result = await _service.SendVerificationEmailAsync(dto);

            // Assert
            Assert.IsTrue(result);
            _emailTemplatesMock.VerifyAll();
            _emailServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task SendVerificationEmailAsync_EmailServiceFails_ReturnsFalse()
        {
            // Arrange
            var dto = new SendVerificationEmailDto
            {
                Email = "fail@example.com",
                Nombre = "Ana",
                PersonaId = 5,
                Rol = "Empleador"
            };

            _emailTemplatesMock
                .Setup(t => t.GetVerificationTemplate(
                    dto.Nombre,
                    It.IsAny<string>(),
                    dto.Rol))
                .Returns("template");

            _emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<SendEmailDto>()))
                .ReturnsAsync(new EmailResponseDto
                {
                    Success = false,
                    Message = "Error",
                    SentAt = DateTime.UtcNow
                });

            // Act
            var result = await _service.SendVerificationEmailAsync(dto);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task SendVerificationEmailAsync_Exception_ReturnsFalse()
        {
            // Arrange
            var dto = new SendVerificationEmailDto
            {
                Email = "exception@example.com",
                Nombre = "Pedro",
                PersonaId = 3,
                Rol = "Empleado"
            };

            _emailTemplatesMock
                .Setup(t => t.GetVerificationTemplate(
                    dto.Nombre,
                    It.IsAny<string>(),
                    dto.Rol))
                .Throws(new Exception("Template error"));

            // Act
            var result = await _service.SendVerificationEmailAsync(dto);

            // Assert
            Assert.IsFalse(result);
        }

        // ----------------- VerifyTokenAsync -----------------

        [TestMethod]
        public async Task VerifyTokenAsync_NoTokenForEmail_ReturnsFalseAndZeroId()
        {
            // Arrange
            var email = "notfound@example.com";
            var token = "ABC123";

            // Diccionario vacío por el Setup

            // Act
            var (isValid, personaId) = await _service.VerifyTokenAsync(email, token);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(0, personaId);
        }

        [TestMethod]
        public async Task VerifyTokenAsync_WrongToken_ReturnsFalse()
        {
            // Arrange
            var email = "user@example.com";
            SetVerificationToken(email, "CORRECT", DateTime.UtcNow.AddHours(1), 42);

            // Act
            var (isValid, personaId) = await _service.VerifyTokenAsync(email, "WRONG");

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(0, personaId);
        }

        [TestMethod]
        public async Task VerifyTokenAsync_ExpiredToken_ReturnsFalseAndRemovesEntry()
        {
            // Arrange
            var email = "expired@example.com";
            SetVerificationToken(email, "TOKEN", DateTime.UtcNow.AddHours(-1), 11);

            // Act
            var (isValid, personaId) = await _service.VerifyTokenAsync(email, "TOKEN");

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(0, personaId);

            // Debe haberse eliminado del diccionario
            var dict = GetVerificationTokens();
            Assert.IsFalse(dict.ContainsKey(email));
        }

        [TestMethod]
        public async Task VerifyTokenAsync_ValidToken_ReturnsTrueAndPersonaId()
        {
            // Arrange
            var email = "valid@example.com";
            var personaIdExpected = 99;
            var token = "OK1234";

            SetVerificationToken(email, token, DateTime.UtcNow.AddHours(1), personaIdExpected);

            // Act
            var (isValid, personaId) = await _service.VerifyTokenAsync(email, token);

            // Assert
            Assert.IsTrue(isValid);
            Assert.AreEqual(personaIdExpected, personaId);

            // Debe haberse eliminado del diccionario
            var dict = GetVerificationTokens();
            Assert.IsFalse(dict.ContainsKey(email));
        }

        // ----------------- IsTokenExpiredAsync -----------------

        [TestMethod]
        public async Task IsTokenExpiredAsync_NoEntry_ReturnsTrue()
        {
            // Arrange
            var email = "none@example.com";

            // Act
            var result = await _service.IsTokenExpiredAsync(email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsTokenExpiredAsync_NotExpired_ReturnsFalse()
        {
            // Arrange
            var email = "noexpired@example.com";
            SetVerificationToken(email, "TOKEN", DateTime.UtcNow.AddHours(2), 1);

            // Act
            var result = await _service.IsTokenExpiredAsync(email);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsTokenExpiredAsync_Expired_ReturnsTrue()
        {
            // Arrange
            var email = "expired2@example.com";
            SetVerificationToken(email, "TOKEN", DateTime.UtcNow.AddHours(-2), 1);

            // Act
            var result = await _service.IsTokenExpiredAsync(email);

            // Assert
            Assert.IsTrue(result);
        }

        // ----------------- VerifyLinkTokenAsync -----------------

        [TestMethod]
        public async Task VerifyLinkTokenAsync_EmptyToken_ReturnsFalse()
        {
            // Arrange
            var token = "";

            // Act
            var (success, personaId, rol) = await _service.VerifyLinkTokenAsync(token);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, personaId);
            Assert.AreEqual(string.Empty, rol);
        }

        [TestMethod]
        public async Task VerifyLinkTokenAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var token = "some-token";

            _usuarioRepoMock
                .Setup(r => r.GetByVerificationHash(It.IsAny<string>()))
                .Returns((Usuario?)null);

            // Act
            var (success, personaId, rol) = await _service.VerifyLinkTokenAsync(token);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, personaId);
            Assert.AreEqual(string.Empty, rol);
        }

        [TestMethod]
        public async Task VerifyLinkTokenAsync_UserAlreadyVerified_ReturnsFalse()
        {
            // Arrange
            var token = "already-verified";

            var user = new Usuario
            {
                IdPersona = 5,
                TipoUsuario = "Empleador",
                Contrasena = "hash",
                IsVerified = true,
                VerificationTokenExpires = DateTime.UtcNow.AddHours(1),
                VerificationTokenHash = "hash"
            };

            _usuarioRepoMock
                .Setup(r => r.GetByVerificationHash(It.IsAny<string>()))
                .Returns(user);

            // Act
            var (success, personaId, rol) = await _service.VerifyLinkTokenAsync(token);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, personaId);
            Assert.AreEqual(string.Empty, rol);
        }

        [TestMethod]
        public async Task VerifyLinkTokenAsync_TokenExpired_ReturnsFalse()
        {
            // Arrange
            var token = "expired-link";

            var user = new Usuario
            {
                IdPersona = 7,
                TipoUsuario = "Empleado",
                Contrasena = "hash",
                IsVerified = false,
                VerificationTokenExpires = DateTime.UtcNow.AddHours(-1),
                VerificationTokenHash = "hash"
            };

            _usuarioRepoMock
                .Setup(r => r.GetByVerificationHash(It.IsAny<string>()))
                .Returns(user);

            // Act
            var (success, personaId, rol) = await _service.VerifyLinkTokenAsync(token);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, personaId);
            Assert.AreEqual(string.Empty, rol);
        }

        [TestMethod]
        public async Task VerifyLinkTokenAsync_ValidToken_UpdatesUserAndReturnsTrue()
        {
            // Arrange
            var token = "valid-link-token";

            var user = new Usuario
            {
                IdPersona = 10,
                TipoUsuario = "Empleado",
                Contrasena = "hash",
                IsVerified = false,
                VerificationTokenExpires = DateTime.UtcNow.AddHours(1),
                VerificationTokenHash = "hash"
            };

            _usuarioRepoMock
                .Setup(r => r.GetByVerificationHash(It.IsAny<string>()))
                .Returns(user);

            _usuarioRepoMock
                .Setup(r => r.UpdateAsync(It.Is<Usuario>(u =>
                    u.IdPersona == user.IdPersona &&
                    u.IsVerified &&
                    u.VerificationTokenExpires == null)))
                .ReturnsAsync(true);

            // Act
            var (success, personaId, rol) = await _service.VerifyLinkTokenAsync(token);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(10, personaId);
            Assert.AreEqual("Empleado", rol);

            _usuarioRepoMock.VerifyAll();
        }
    }
}