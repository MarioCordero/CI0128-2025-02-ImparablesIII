using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace backend.Services
{
    public class PasswordSetupService : IPasswordSetupService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<PasswordSetupService> _logger;
        private readonly EmployeeRepository _employeeRepository;
        private readonly string _connectionString;

        public PasswordSetupService(
            IMemoryCache cache, 
            ILogger<PasswordSetupService> logger,
            EmployeeRepository employeeRepository,
            IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _employeeRepository = employeeRepository;
            _connectionString = configuration.GetConnectionString("CountryContext") ?? throw new ArgumentNullException("Connection string not found");
        }

        public Task<string> GeneratePasswordSetupTokenAsync(int personaId, string email)
        {
            try
            {
                // Generate a secure random token
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

                // Store token in cache with expiration (30 minutes)
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
                // Validate token
                var cacheKey = $"password_setup_{request.Token}";
                var tokenData = _cache.Get(cacheKey);
                
                if (tokenData == null)
                {
                    return new PasswordSetupResponseDto
                    {
                        Success = false,
                        Message = "Token inválido o expirado"
                    };
                }

                // Get token data
                var tokenInfo = (dynamic)tokenData;
                int personaId = tokenInfo.PersonaId;
                string email = tokenInfo.Email;

                // Save password without hash (as requested by user)
                var plainPassword = request.Password;

                // Create user in database
                var userCreated = await CreateUserAsync(personaId, plainPassword, "Empleado");
                
                if (!userCreated)
                {
                    return new PasswordSetupResponseDto
                    {
                        Success = false,
                        Message = "Error al crear el usuario en la base de datos"
                    };
                }

                // Remove token from cache
                _cache.Remove(cacheKey);

                _logger.LogInformation($"Password setup completed successfully for persona {personaId}");
                
                return new PasswordSetupResponseDto
                {
                    Success = true,
                    Message = "Contraseña configurada exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up password for token {token}", request.Token);
                return new PasswordSetupResponseDto
                {
                    Success = false,
                    Message = "Error interno del servidor"
                };
            }
        }

        public async Task<bool> CreateUserAsync(int personaId, string password, string tipoUsuario = "Empleado")
        {
            try
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.Usuario (idPersona, TipoUsuario, Contrasena)
                    VALUES (@idPersona, @tipoUsuario, @contrasena)";

                using var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", personaId);
                command.Parameters.AddWithValue("@tipoUsuario", tipoUsuario);
                command.Parameters.AddWithValue("@contrasena", password);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user for persona {personaId}", personaId);
                return false;
            }
        }

    }
}
