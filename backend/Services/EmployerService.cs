using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Constants;

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
                _logger.LogInformation($"Persona created with ID: {createdPersona.Id}");

                var verificationEmailDto = new SendVerificationEmailDto
                {
                    Email = form.Email,
                    Nombre = form.Nombre,
                    PersonaId = createdPersona.Id,
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
            try
            {
                var persona = await _personaRepository.GetByIdAsync(personaId);
                if (persona == null)
                    throw new Exception("Persona no encontrada");

                // Crear Usuario
                var usuario = new Usuario
                {
                    idPersona = personaId,
                    NombreUsuario = persona.Email,
                    Contraseña = BCrypt.Net.BCrypt.HashPassword(password),
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true
                };

                await _usuarioRepository.AddAsync(usuario);
                _logger.LogInformation($"User created for persona ID: {personaId}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user for employer");
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