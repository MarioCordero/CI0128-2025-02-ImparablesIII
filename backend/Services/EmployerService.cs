using backend.Models;
using backend_lab_c28730.Controllers;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public interface IEmployerService
    {
        Task<ServiceResult<Employer>> RegisterEmployerAsync(SignUpEmployerDto employerDto);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
    }

    public class EmployerService : IEmployerService
    {
        private readonly List<Employer> _employers; // In-memory storage for now
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(ILogger<EmployerService> logger)
        {
            _logger = logger;
            _employers = new List<Employer>(); // This would be replaced with a database context
        }

        public async Task<ServiceResult<Employer>> RegisterEmployerAsync(SignUpEmployerDto employerDto)
        {
            try
            {
                // Validate the input
                var validationResult = await ValidateEmployerDataAsync(employerDto);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                // Check for duplicates
                if (!await IsUsernameAvailableAsync(employerDto.Username))
                {
                    return ServiceResult<Employer>.Failure("Username already exists");
                }

                if (!await IsEmailAvailableAsync(employerDto.Email))
                {
                    return ServiceResult<Employer>.Failure("Email already exists");
                }

                if (!await IsCedulaAvailableAsync(employerDto.Cedula))
                {
                    return ServiceResult<Employer>.Failure("Cedula already exists");
                }

                // Create new employer
                var employer = new Employer
                {
                    Id = _employers.Count + 1, // Simple ID generation for demo
                    Nombre = employerDto.Nombre,
                    PrimerApellido = employerDto.PrimerApellido,
                    SegundoApellido = employerDto.SegundoApellido,
                    Cedula = employerDto.Cedula,
                    Email = employerDto.Email,
                    Telefono = employerDto.Telefono,
                    Username = employerDto.Username,
                    FechaNacimiento = employerDto.FechaNacimiento,
                    PasswordHash = HashPassword(employerDto.Password),
                    EmailVerificationToken = GenerateVerificationToken(),
                    CreatedAt = DateTime.UtcNow
                };

                // Save to "database" (in-memory list for now)
                _employers.Add(employer);

                _logger.LogInformation("New employer registered: {Username}", employer.Username);

                return ServiceResult<Employer>.Success(employer, "Employer registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering employer");
                return ServiceResult<Employer>.Failure("An error occurred while registering the employer");
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return await Task.FromResult(!_employers.Any(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await Task.FromResult(!_employers.Any(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            return await Task.FromResult(!_employers.Any(e => e.Cedula.Equals(cedula)));
        }

        private async Task<ServiceResult<Employer>> ValidateEmployerDataAsync(SignUpEmployerDto employerDto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(employerDto.Nombre))
                errors.Add("Name is required");

            if (string.IsNullOrWhiteSpace(employerDto.PrimerApellido))
                errors.Add("First surname is required");

            if (string.IsNullOrWhiteSpace(employerDto.Cedula))
                errors.Add("Cedula is required");

            if (string.IsNullOrWhiteSpace(employerDto.Email) || !IsValidEmail(employerDto.Email))
                errors.Add("Valid email is required");

            if (string.IsNullOrWhiteSpace(employerDto.Username))
                errors.Add("Username is required");

            if (string.IsNullOrWhiteSpace(employerDto.Password))
                errors.Add("Password is required");

            if (employerDto.Password != employerDto.ConfirmPassword)
                errors.Add("Passwords do not match");

            if (employerDto.Password.Length < 6)
                errors.Add("Password must be at least 6 characters long");

            if (employerDto.FechaNacimiento > DateTime.Now.AddYears(-18))
                errors.Add("Must be at least 18 years old");

            if (errors.Any())
            {
                return ServiceResult<Employer>.Failure(string.Join("; ", errors));
            }

            return ServiceResult<Employer>.Success(null!, "Validation passed");
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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt")); // Simple salt for demo
            return Convert.ToBase64String(hashedBytes);
        }

        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString();
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
