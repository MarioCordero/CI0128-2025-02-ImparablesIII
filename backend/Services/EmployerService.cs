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
        private readonly IEmailHelper _emailHelper;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(
            IEmployerRepository employerRepository,
            IPersonaRepository personaRepository,
            IUsuarioRepository usuarioRepository,
            IEmailHelper emailHelper,
            ILogger<EmployerService> logger)
        {
            _employerRepository = employerRepository;
            _personaRepository = personaRepository;
            _usuarioRepository = usuarioRepository;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        public async Task<bool> RegisterEmployerAsync(SignUpEmployerDto form)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(form.Password))
                {
                    _logger.LogWarning("Password vacío");
                    return false;
                }
                if (!await IsEmailAvailableAsync(form.Email))
                    return false;
                if (!await IsCedulaAvailableAsync(form.Cedula))
                    return false;

                var apellidos = form.PrimerApellido;
                if (!string.IsNullOrEmpty(form.SegundoApellido))
                    apellidos += $" {form.SegundoApellido}";

                var persona = new Persona
                {
                    Nombre = form.Nombre,
                    Apellidos = apellidos,
                    Correo = form.Email,
                    Cedula = form.Cedula,
                    Telefono = form.Telefono,
                    FechaNacimiento = form.FechaNacimiento,
                    Rol = "Empleador"
                };

                var personaId = await _personaRepository.CreatePersonaAsync(persona);
                if (personaId <= 0)
                {
                    _logger.LogError("Error creando Persona");
                    return false;
                }

                var rawToken = _emailHelper.GenerateVerificationToken();
                var hash = _emailHelper.HashToken(rawToken);

                var usuario = new Usuario
                {
                    IdPersona = personaId,
                    TipoUsuario = "Empleador",
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(form.Password),
                    VerificationTokenHash = hash,
                    VerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                    IsVerified = false
                };

                var created = await _usuarioRepository.CreateUserAsync(usuario);
                if (!created)
                {
                    _logger.LogError("Error creando Usuario para Persona {PersonaId}", personaId);
                    return false;
                }

                await _emailHelper.SendVerificationLinkAsync(form.Email, rawToken);
                _logger.LogInformation("Registro empleador OK Persona {PersonaId}", personaId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando empleador");
                return false;
            }
        }

        // Implementación requerida por la interfaz
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

            await _emailHelper.SendVerificationLinkAsync(email, rawToken);
            return true;
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var persona = await _personaRepository.GetByEmailAsync(email);
            return persona == null;
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            var empresa = await _employerRepository.GetByCedulaAsync(cedula);
            return empresa == null;
        }
    }
}