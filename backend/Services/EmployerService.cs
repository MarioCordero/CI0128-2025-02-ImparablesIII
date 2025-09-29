using backend.Models;
using backend_lab.Controllers;
using backend_lab.Data;
using backend_lab.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public interface IEmployerService
    {
        Task<ServiceResult<EmployerResponseDto>> RegisterEmployerAsync(SignUpEmployerDto employerDto);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
    }

    public class EmployerService : IEmployerService
    {
        private readonly PlaniFyDbContext _context;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(PlaniFyDbContext context, ILogger<EmployerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<EmployerResponseDto>> RegisterEmployerAsync(SignUpEmployerDto employerDto)
        {
            try
            {
                // Validate the input
                var validationResult = await ValidateEmployerDataAsync(employerDto);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<EmployerResponseDto>.Failure(validationResult.Message, validationResult.Errors);
                }

                // Check for duplicates (using email as unique identifier)
                if (!await IsEmailAvailableAsync(employerDto.Email))
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Email already exists");
                }

                if (!await IsCedulaAvailableAsync(employerDto.Cedula))
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Cedula already exists");
                }

                // Create ONLY the Persona record
                var persona = new Persona
                {
                    Correo = employerDto.Email,
                    Nombre = employerDto.Nombre,
                    SegundoNombre = null, // Not used in current DTO
                    Apellidos = !string.IsNullOrEmpty(employerDto.SegundoApellido) 
                        ? $"{employerDto.PrimerApellido} {employerDto.SegundoApellido}" 
                        : employerDto.PrimerApellido,
                    FechaNacimiento = employerDto.FechaNacimiento,
                    Cedula = employerDto.Cedula,
                    Rol = "Empleado",
                    Telefono = int.TryParse(employerDto.Telefono, out var phone) ? phone : null,
                    idDireccion = null
                };

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New persona registered with ID: {PersonaId}", persona.Id);

                var responseDto = new EmployerResponseDto
                {
                    Id = persona.Id,
                    Username = employerDto.Email, // Use email as username
                    Email = persona.Correo,
                    Nombre = persona.NombreCompleto,
                    CreatedAt = DateTime.UtcNow
                };

                return ServiceResult<EmployerResponseDto>.Success(responseDto, "Person registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering person: {Error}", ex.Message);
                return ServiceResult<EmployerResponseDto>.Failure("An error occurred while registering the person");
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            // Since we're only using Persona table and email as identifier
            // We'll check email availability instead using ToLower for case-insensitive comparison
            return await IsEmailAvailableAsync(username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            // Use EF.Functions.Like or simple string comparison for database translation
            return !await _context.Personas.AnyAsync(p => p.Correo.ToLower() == email.ToLower());
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            return !await _context.Personas.AnyAsync(p => p.Cedula == cedula);
        }

        private async Task<ServiceResult<EmployerResponseDto>> ValidateEmployerDataAsync(SignUpEmployerDto employerDto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(employerDto.Nombre))
                errors.Add("Name is required");

            if (string.IsNullOrWhiteSpace(employerDto.PrimerApellido))
                errors.Add("First surname is required");

            if (string.IsNullOrWhiteSpace(employerDto.Cedula))
                errors.Add("Cedula is required");

            if (employerDto.Cedula.Length != 9)
                errors.Add("Cedula must be exactly 9 characters");

            if (string.IsNullOrWhiteSpace(employerDto.Email) || !IsValidEmail(employerDto.Email))
                errors.Add("Valid email is required");

            if (employerDto.FechaNacimiento > DateTime.Now.AddYears(-18))
                errors.Add("Must be at least 18 years old");

            // Note: Removed password validation since we're only saving to Persona table

            if (errors.Any())
            {
                return ServiceResult<EmployerResponseDto>.Failure("Validation failed", errors);
            }

            return ServiceResult<EmployerResponseDto>.Success(null!, "Validation passed");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    // Service result wrapper
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static ServiceResult<T> Failure(string message, List<string>? errors = null)
        {
            return new ServiceResult<T> 
            { 
                IsSuccess = false, 
                Message = message, 
                Errors = errors ?? new List<string>() 
            };
        }
    }

    // Response DTO for returning employer data without sensitive information
    public class EmployerResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
