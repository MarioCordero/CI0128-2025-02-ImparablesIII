using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IDirectionRepository _directionRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(
            IEmployerRepository employerRepository,
            IPersonaRepository personaRepository,
            IUsuarioRepository usuarioRepository,
            IDirectionRepository directionRepository,
            IEmailHelper emailHelper,
            ILogger<EmployerService> logger)
        {
            _employerRepository = employerRepository;
            _personaRepository = personaRepository;
            _usuarioRepository = usuarioRepository;
            _directionRepository = directionRepository;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        // REGISTER EMPLOYER
        public async Task<bool> RegisterEmployerAsync(SignUpEmployerDto form)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(form.Password))
                {
                    _logger.LogWarning("Intento de registro con password vacío");
                    return false;
                }
                if (!await IsEmailAvailableAsync(form.Email))
                    return false;

                if (!await IsCedulaAvailableAsync(form.Cedula))
                    return false;
                var rawToken = _emailHelper.GenerateVerificationToken();
                var tokenHash = _emailHelper.HashToken(rawToken);
                var tokenExpires = DateTime.UtcNow.AddHours(24);
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(form.Password);
                var command = new EmployerRegistrationCommand(form, passwordHash, tokenHash, tokenExpires);
                int newPersonaId = await _employerRepository.RegisterEmployerTransactionalAsync(command);
                if (newPersonaId <= 0)
                {
                    _logger.LogError("El registro falló: La base de datos no retornó un ID válido.");
                    return false;
                }
                try 
                {
                    await _emailHelper.SendVerificationLinkAsync(form.Email, rawToken, "Empleador");
                    _logger.LogInformation("Registro de empleador exitoso. ID Persona: {PersonaId}", newPersonaId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Usuario {Id} creado correctamente, pero falló el envío del correo de verificación.", newPersonaId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante el proceso de registro de empleador.");
                return false;
            }
        }

        // VERIFY AND CREATE USER
        public async Task<bool> VerifyAndCreateUserAsync(int personaId, string password)
        {
            var persona = await _personaRepository.GetByIdAsync(personaId);
            if (persona == null) return false;

            var existing = await _usuarioRepository.GetUserByIdAsync(personaId);
            if (existing != null)
            {
                if (!existing.IsVerified)
                {
                    existing.IsVerified = true;
                    existing.VerificationTokenHash = null;
                    existing.VerificationTokenExpires = null;
                    return await _usuarioRepository.UpdateAsync(existing);
                }
                return true;
            }

            var usuario = new Usuario
            {
                IdPersona = personaId,
                TipoUsuario = persona.Rol,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(password),
                IsVerified = true
            };

            return await _usuarioRepository.CreateUserAsync(usuario);
        }

        // RESEND VERIFICATION EMAIL FUTURE USE, NOT IMPLEMENTED YET
        public async Task<bool> ResendVerificationAsync(string email)
        {
            var persona = await _personaRepository.GetByEmailAsync(email);
            if (persona == null) return false;

            var usuario = await _usuarioRepository.GetUserByIdAsync(persona.Id);
            if (usuario == null || usuario.IsVerified) return false;

            var rawToken = _emailHelper.GenerateVerificationToken();
            usuario.VerificationTokenHash = _emailHelper.HashToken(rawToken);
            usuario.VerificationTokenExpires = DateTime.UtcNow.AddHours(24);

            var updated = await _usuarioRepository.UpdateAsync(usuario);
            if (!updated) return false;

            await _emailHelper.SendVerificationLinkAsync(email, rawToken, "Empleador");
            return true;
        }

        // CHECK IF EMAIL IS AVAILABLE
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var persona = await _personaRepository.GetByEmailAsync(email);
            return persona == null;
        }

        // CHECK IF CEDULA IS AVAILABLE
        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            var empresa = await _personaRepository.GetByCedulaAsync(cedula);
            return empresa == null;
        }

        public async Task<KPIResponseDTO?> GetKPIAsync(int userId)
        {
            try
            {
                var kpiData = await _employerRepository.GetKPIDataAsync(userId);
                return kpiData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo KPI para Usuario {UserId}", userId);
                return null;
            }
        }
    }
}