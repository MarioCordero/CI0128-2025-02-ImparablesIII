using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using BCrypt.Net;

namespace backend.Services
{
    public class PasswordSetupService : IPasswordSetupService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<PasswordSetupService> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPasswordRepository _passwordRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PasswordSetupService(
            IMemoryCache cache, 
            ILogger<PasswordSetupService> logger,
            IEmployeeRepository employeeRepository,
            IPasswordRepository passwordRepository,
            IUsuarioRepository usuarioRepository)
        {
            _cache = cache;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _passwordRepository = passwordRepository;
            _usuarioRepository = usuarioRepository;
        }

        public Task<string> GeneratePasswordSetupTokenAsync(int personaId, string email)
        {
            try
            {
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

                var cacheKey = $"password_setup_{token}";
                var tokenData = new { PersonaId = personaId, Email = email, CreatedAt = DateTime.UtcNow };
                _cache.Set(cacheKey, tokenData, TimeSpan.FromMinutes(30));

                _logger.LogInformation($"Generated password setup token for persona {personaId} with email {email}");
                return Task.FromResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating password setup token for persona {personaId}", personaId);
                throw;
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var cacheKey = $"password_setup_{token}";
                var tokenData = _cache.Get(cacheKey);
                return Task.FromResult(tokenData != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token {token}", token);
                return Task.FromResult(false);
            }
        }

        public async Task<PasswordSetupResponseDto> SetupPasswordAsync(PasswordSetupRequestDto request)
        {
            try
            {
                var cacheKey = $"password_setup_{request.Token}";
                if (!_cache.TryGetValue(cacheKey, out var tokenData))
                {
                    return new PasswordSetupResponseDto { Success = false, Message = "Token inválido o expirado" };
                }

                var tokenInfo = (dynamic)tokenData;
                int personaId = tokenInfo.PersonaId;
                string email = tokenInfo.Email;

                var plainPassword = request.Password;
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                var usuario = new Usuario
                {
                    IdPersona = personaId,
                    TipoUsuario = "Empleado",
                    Contrasena = hashedPassword
                };

                var userCreated = await _usuarioRepository.CreateUserAsync(usuario);
                var passwordUpdated = await _passwordRepository.UpdateEmployeePasswordAsync(personaId, hashedPassword);

                if (!userCreated || !passwordUpdated)
                {
                    return new PasswordSetupResponseDto { Success = false, Message = "Error al crear el usuario" };
                }

                _cache.Remove(cacheKey);
                _logger.LogInformation("Password setup completed for persona {PersonaId}", personaId);

                return new PasswordSetupResponseDto { Success = true, Message = "Contraseña configurada exitosamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up password for token {Token}", request.Token);
                return new PasswordSetupResponseDto { Success = false, Message = "Error interno del servidor" };
            }
        }
    }
}