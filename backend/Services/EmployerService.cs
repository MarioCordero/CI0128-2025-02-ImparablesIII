using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    public interface IEmployerService
    {
        Task<ServiceResult<EmployerResponseDto>> RegisterEmployerAsync(SignUpEmployerDto employerDto);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
    }

    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(IEmployerRepository employerRepository, IEmailService emailService, ILogger<EmployerService> logger)
        {
            _employerRepository = employerRepository;
            _emailService = emailService;
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

                // Check for duplicates
                if (!await _employerRepository.IsEmailAvailableAsync(employerDto.Email))
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Email already exists");
                }

                if (!await _employerRepository.IsCedulaAvailableAsync(employerDto.Cedula))
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Cedula already exists");
                }

                // 1. Create Direccion record first
                var direccionId = await _employerRepository.CreateDireccionAsync(
                    employerDto.Provincia,
                    employerDto.Canton,
                    employerDto.Distrito,
                    employerDto.DireccionParticular
                );

                if (direccionId == -1)
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Failed to create address");
                }

                // 2. Create Persona record
                var apellidos = !string.IsNullOrEmpty(employerDto.SegundoApellido) 
                    ? $"{employerDto.PrimerApellido} {employerDto.SegundoApellido}" 
                    : employerDto.PrimerApellido;

                var personaId = await _employerRepository.CreatePersonaAsync(
                    employerDto.Email,
                    employerDto.Nombre,
                    apellidos,
                    employerDto.FechaNacimiento,
                    employerDto.Cedula,
                    "Empleador", // Set as Empleador
                    employerDto.Telefono,
                    direccionId
                );

                if (personaId == -1)
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Failed to create person record");
                }

                // 3. Create Usuario record
                var usuarioCreated = await _employerRepository.CreateUsuarioAsync(
                    personaId,
                    employerDto.Password,
                    "Empleador" // Set as Empleador
                );

                if (!usuarioCreated)
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Failed to create user record");
                }

                _logger.LogInformation("New employer registered with Persona ID: {PersonaId}", personaId);

                // 4. Get the created employer data
                var responseDto = await _employerRepository.GetEmployerByIdAsync(personaId);
                if (responseDto == null)
                {
                    return ServiceResult<EmployerResponseDto>.Failure("Failed to retrieve created employer data");
                }

                // 5. Send welcome email
                await SendWelcomeEmailAsync(responseDto);

                return ServiceResult<EmployerResponseDto>.Success(responseDto, "Employer registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering employer: {Error}", ex.Message);
                return ServiceResult<EmployerResponseDto>.Failure("An error occurred while registering the employer");
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await _employerRepository.IsEmailAvailableAsync(email);
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            return await _employerRepository.IsCedulaAvailableAsync(cedula);
        }

        private Task<ServiceResult<EmployerResponseDto>> ValidateEmployerDataAsync(SignUpEmployerDto employerDto)
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

            if (string.IsNullOrWhiteSpace(employerDto.Password))
                errors.Add("Password is required");

            if (employerDto.Password.Length < 8)
                errors.Add("Password must be at least 8 characters");

            if (employerDto.Password != employerDto.ConfirmPassword)
                errors.Add("Passwords do not match");

            if (string.IsNullOrWhiteSpace(employerDto.Provincia))
                errors.Add("Provincia is required");

            if (string.IsNullOrWhiteSpace(employerDto.Canton))
                errors.Add("Canton is required");

            if (string.IsNullOrWhiteSpace(employerDto.Distrito))
                errors.Add("Distrito is required");

            if (errors.Any())
            {
                return Task.FromResult(ServiceResult<EmployerResponseDto>.Failure("Validation failed", errors));
            }

            return Task.FromResult(ServiceResult<EmployerResponseDto>.Success(null!, "Validation passed"));
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

        private async Task SendWelcomeEmailAsync(EmployerResponseDto employer)
        {
            try
            {
                var fullName = $"{employer.Nombre} {employer.Apellidos}";
                var emailBody = EmailTemplates.GetWelcomeEmailTemplate(fullName);
                
                var emailDto = new SendEmailDto
                {
                    ReceiverEmail = employer.Correo,
                    Subject = "¡Bienvenido a Imparables!",
                    Body = emailBody,
                    IsHtml = true
                };

                var emailResult = await _emailService.SendEmailAsync(emailDto);
                
                if (emailResult.Success)
                {
                    _logger.LogInformation("Welcome email sent successfully to {Email}", employer.Correo);
                }
                else
                {
                    _logger.LogWarning("Failed to send welcome email to {Email}: {Message}", employer.Correo, emailResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email to {Email}", employer.Correo);
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

}
