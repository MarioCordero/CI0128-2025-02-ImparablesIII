using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Constants;
using BCrypt.Net;

namespace backend.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailVerificationService _verificationService;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(
            IEmployerRepository employerRepository,
            IPersonaRepository personaRepository,
            IUsuarioRepository usuarioRepository,
            IEmailVerificationService verificationService,
            ILogger<EmployerService> logger)
        {
            _employerRepository = employerRepository;
            _personaRepository = personaRepository;
            _usuarioRepository = usuarioRepository;
            _verificationService = verificationService;
            _logger = logger;
        }

        public async Task<bool> RegisterEmployerAsync(SignUpEmployerDto form)
        {
            try
            {
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

                var createdPersona = await _personaRepository.CreatePersonaAsync(persona);
                _logger.LogInformation($"Persona created with ID: {createdPersona}");

                var verificationEmailDto = new SendVerificationEmailDto
                {
                    Email = form.Email,
                    Nombre = form.Nombre,
                    PersonaId = createdPersona,
                    Rol = "Empleador"
                };

                var emailSent = await _verificationService.SendVerificationEmailAsync(verificationEmailDto);
                
                if (!emailSent)
                {
                    _logger.LogError($"Failed to send verification email to {form.Email}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering employer");
                return false;
            }
        }

        public async Task<bool> VerifyAndCreateUserAsync(int personaId, string password)
        {
            if (personaId <= 0 || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                var persona = await _personaRepository.GetByIdAsync(personaId);
                if (persona == null)
                {
                    _logger.LogWarning("Persona no encontrada (ID {PersonaId})", personaId);
                    return false;
                }

                var usuario = new Usuario
                {
                    IdPersona = personaId,
                    TipoUsuario = "Empleador",
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(password)
                };

                var created = await _employerRepository.CreateUserAsync(usuario);

                if (!created)
                {
                    _logger.LogError("No se pudo crear el usuario para la persona {PersonaId}", personaId);
                    return false;
                }

                _logger.LogInformation("Usuario creado para la persona {PersonaId}", personaId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario para la persona {PersonaId}", personaId);
                return false;
            }
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