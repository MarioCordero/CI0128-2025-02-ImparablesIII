using backend.Extensions;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserController> _logger;

        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Example endpoint demonstrating how to send emails using the class method approach
        /// This simulates a user registration process that sends a welcome email
        /// </summary>
        /// <param name="userEmail">User's email address</param>
        /// <param name="userName">User's name</param>
        /// <returns>Registration result</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                // Simulate user registration logic here
                _logger.LogInformation($"Registering user: {registrationDto.UserName} with email: {registrationDto.UserEmail}");
                
                // Example of using the class method to send welcome email with HTML template
                var htmlEmailBody = EmailTemplates.GetWelcomeEmailTemplate(registrationDto.UserName);

                // Using the extension method with HTML support
                var emailSent = await _serviceProvider.SendEmailAsync(
                    registrationDto.UserEmail,
                    "¡Bienvenido a la plataforma Imparables!",
                    htmlEmailBody,
                    isHtml: true
                );

                if (emailSent)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "User registered successfully and welcome email sent",
                        UserEmail = registrationDto.UserEmail,
                        UserName = registrationDto.UserName,
                        RegisteredAt = DateTime.UtcNow
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "User registered successfully but email sending failed",
                        UserEmail = registrationDto.UserEmail,
                        UserName = registrationDto.UserName,
                        RegisteredAt = DateTime.UtcNow,
                        Warning = "Welcome email could not be sent"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Registration failed",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Example endpoint for password reset that uses the detailed response method
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <returns>Password reset result</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                // Simulate password reset logic here
                var resetToken = GenerateAlphanumericToken(8); // Generate alphanumeric token in caps
                
                _logger.LogInformation($"Processing password reset for: {forgotPasswordDto.Email}");
                
                // Generate HTML email using template
                var htmlEmailBody = EmailTemplates.GetPasswordResetTemplate(resetToken);

                // Using the detailed response method with HTML support
                var emailResult = await _serviceProvider.SendEmailWithResponseAsync(
                    forgotPasswordDto.Email,
                    "Restablecimiento de Contraseña - Plataforma Imparables",
                    htmlEmailBody,
                    isHtml: true
                );

                return Ok(new
                {
                    Success = emailResult.Success,
                    Message = emailResult.Success 
                        ? "Password reset email sent successfully" 
                        : "Failed to send password reset email",
                    Email = forgotPasswordDto.Email,
                    ResetToken = emailResult.Success ? resetToken : null,
                    SentAt = emailResult.SentAt,
                    EmailDetails = emailResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Password reset failed",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Generate an alphanumeric token in all caps
        /// </summary>
        /// <param name="length">Length of the token</param>
        /// <returns>Alphanumeric token in uppercase</returns>
        private string GenerateAlphanumericToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    // DTOs for the example endpoints
    public class UserRegistrationDto
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
