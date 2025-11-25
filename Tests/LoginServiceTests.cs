using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace backend.Tests
{
    [TestClass]
    public class LoginServiceTests
    {
        private Mock<IUsuarioRepository> _usuarioRepoMock = null!;
        private Mock<IEmployeeRepository> _employeeRepoMock = null!;
        private Mock<IConfiguration> _configMock = null!;
        private LoginService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            _employeeRepoMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
            _configMock = new Mock<IConfiguration>();

            _service = new LoginService(
                _usuarioRepoMock.Object,
                _employeeRepoMock.Object,
                _configMock.Object
            );
        }

        // ---------------------------------------------------------------
        // USER NOT FOUND
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_UserNotFound_ReturnsError()
        {
            var request = new LoginRequestDto
            {
                Correo = "noexiste@example.com",
                Contrasena = "1234"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync((Usuario?)null);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Usuario no encontrado", result.Message);
        }

        // ---------------------------------------------------------------
        // WRONG PASSWORD
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_WrongPassword_ReturnsError()
        {
            var request = new LoginRequestDto
            {
                Correo = "test@example.com",
                Contrasena = "badpassword"
            };

            var storedHash = BCrypt.Net.BCrypt.HashPassword("correctPassword");

            var user = new Usuario
            {
                IdPersona = 1,
                Contrasena = storedHash,
                IsVerified = true,
                TipoUsuario = "Empleador"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync(user);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Contraseña incorrecta", result.Message);
        }

        // ---------------------------------------------------------------
        // ACCOUNT NOT VERIFIED
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_AccountNotVerified_ReturnsError()
        {
            var request = new LoginRequestDto
            {
                Correo = "test@example.com",
                Contrasena = "password"
            };

            var storedHash = BCrypt.Net.BCrypt.HashPassword("password");

            var user = new Usuario
            {
                IdPersona = 1,
                Contrasena = storedHash,
                IsVerified = false,
                TipoUsuario = "Empleado"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync(user);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Cuenta no verificada", result.Message);
        }

        // ---------------------------------------------------------------
        // EMPLOYEE DELETED (logical deletion)
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_EmployeeDeleted_ReturnsError()
        {
            var request = new LoginRequestDto
            {
                Correo = "employee@example.com",
                Contrasena = "password"
            };

            var storedHash = BCrypt.Net.BCrypt.HashPassword("password");

            var user = new Usuario
            {
                IdPersona = 10,
                Contrasena = storedHash,
                IsVerified = true,
                TipoUsuario = "Empleado"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync(user);

            _employeeRepoMock.Setup(r => r.IsEmployeeDeletedAsync(10))
                             .ReturnsAsync(true);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Esta cuenta ha sido desactivada y ya no tiene acceso al sistema.", result.Message);
        }

        // ---------------------------------------------------------------
        // ERROR GETTING USER DATA
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_GetUserDataReturnsNull_ReturnsError()
        {
            var request = new LoginRequestDto
            {
                Correo = "test@example.com",
                Contrasena = "password"
            };

            var storedHash = BCrypt.Net.BCrypt.HashPassword("password");

            var user = new Usuario
            {
                IdPersona = 100,
                Contrasena = storedHash,
                IsVerified = true,
                TipoUsuario = "Empleador"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync(user);

            _employeeRepoMock.Setup(r => r.IsEmployeeDeletedAsync(It.IsAny<int>()))
                             .ReturnsAsync(false);

            _usuarioRepoMock.Setup(r => r.GetUserDataAsync(100))
                            .ReturnsAsync((UserDataDto?)null);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Error al obtener datos del usuario", result.Message);
        }

        // ---------------------------------------------------------------
        // SUCCESSFUL LOGIN
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccessAndToken()
        {
            var request = new LoginRequestDto
            {
                Correo = "good@example.com",
                Contrasena = "password"
            };

            var storedHash = BCrypt.Net.BCrypt.HashPassword("password");

            var user = new Usuario
            {
                IdPersona = 5,
                Contrasena = storedHash,
                IsVerified = true,
                TipoUsuario = "Empleador"
            };

            var userData = new UserDataDto
            {
                IdPersona = 5,
                Nombre = "Luis"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ReturnsAsync(user);

            _employeeRepoMock.Setup(r => r.IsEmployeeDeletedAsync(It.IsAny<int>()))
                             .ReturnsAsync(false);

            _usuarioRepoMock.Setup(r => r.GetUserDataAsync(5))
                            .ReturnsAsync(userData);

            var result = await _service.AuthenticateAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Login exitoso", result.Message);
            Assert.IsNotNull(result.Token);
            Assert.IsNotNull(result.UserData);
            Assert.AreEqual(5, result.UserData!.IdPersona);
        }

        // ---------------------------------------------------------------
        // INTERNAL ERROR
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task AuthenticateAsync_ExceptionThrown_ReturnsServerError()
        {
            var request = new LoginRequestDto
            {
                Correo = "boom@example.com",
                Contrasena = "password"
            };

            _usuarioRepoMock.Setup(r => r.GetUserByEmailAsync(request.Correo))
                            .ThrowsAsync(new Exception("DB broke"));

            var result = await _service.AuthenticateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("Error interno del servidor"));
        }

        // ---------------------------------------------------------------
        // GetUserDataAsync
        // ---------------------------------------------------------------
        [TestMethod]
        public async Task GetUserDataAsync_ValidId_ReturnsUserData()
        {
            var dto = new UserDataDto
            {
                IdPersona = 10,
                Nombre = "Maria"
            };

            _usuarioRepoMock.Setup(r => r.GetUserDataAsync(10))
                            .ReturnsAsync(dto);

            var result = await _service.GetUserDataAsync(10);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result!.IdPersona);
        }

        [TestMethod]
        public async Task GetUserDataAsync_ExceptionThrown_ReturnsNull()
        {
            _usuarioRepoMock.Setup(r => r.GetUserDataAsync(It.IsAny<int>()))
                            .ThrowsAsync(new Exception("Error"));

            var result = await _service.GetUserDataAsync(1);

            Assert.IsNull(result);
        }
    }
}