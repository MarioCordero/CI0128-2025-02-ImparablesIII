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
        private readonly IEmailVerificationService _emailVerificationService;

        public PasswordSetupService(
            IMemoryCache cache, 
            ILogger<PasswordSetupService> logger,
            IEmployeeRepository employeeRepository,
            IPasswordRepository passwordRepository,
            IUsuarioRepository usuarioRepository,
            IEmailVerificationService emailVerificationService)
        {
            _cache = cache;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _passwordRepository = passwordRepository;
            _usuarioRepository = usuarioRepository;
            _emailVerificationService = emailVerificationService;
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

        // AYUDA
        public async Task<PasswordSetupResponseDto> SetupPasswordAsync(PasswordSetupRequestDto request)
        {
            try
            {
                _logger.LogInformation("Received request with token: '{Token}' and password length: {PasswordLength}", 
                    request?.Token ?? "NULL", request?.Password?.Length ?? 0);

                if (request == null)
                {
                    _logger.LogError("Request is null");
                    return new PasswordSetupResponseDto { Success = false, Message = "Request inválido" };
                }

                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    _logger.LogError("Token is null or empty");
                    return new PasswordSetupResponseDto { Success = false, Message = "Token es requerido" };
                }

                var (isValid, personaId, rol) = await _emailVerificationService.VerifyLinkTokenAsync(request.Token);
                _logger.LogInformation("VerifyLinkTokenAsync result - IsValid: {IsValid}, PersonaId: {PersonaId}, Rol: '{Rol}'", 
                    isValid, personaId, rol ?? "NULL");
                
                if (!isValid)
                {
                    _logger.LogWarning("Token validation failed for token: {Token}", request.Token);
                    return new PasswordSetupResponseDto { Success = false, Message = "Token inválido o expirado" };
                }

                var plainPassword = request.Password;
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                // Solo actualizar la contraseña, ya que VerifyLinkTokenAsync ya marcó como verificado
                var passwordUpdated = await _passwordRepository.UpdateEmployeePasswordAsync(personaId, hashedPassword);

                if (!passwordUpdated)
                {
                    return new PasswordSetupResponseDto { Success = false, Message = "Error al actualizar la contraseña" };
                }

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