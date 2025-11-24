using backend.Extensions;
using backend.Services;
using backend.Constants;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailTemplates _emailTemplates;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IServiceProvider serviceProvider, 
            IEmailTemplates emailTemplates,
            ILogger<UserController> logger)
        {
            _serviceProvider = serviceProvider;
            _emailTemplates = emailTemplates;
            _logger = logger;
        }

        // DELETE
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                _logger.LogInformation($"Registering user: {registrationDto.UserName} with email: {registrationDto.UserEmail}");
                var htmlEmailBody = _emailTemplates.GetWelcomeEmailTemplate(registrationDto.UserName);
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
                        Message = ReturnMessagesConstants.User.UserRegisteredSuccessfully,
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
                        Message = ReturnMessagesConstants.User.UserRegisteredButEmailFailed,
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
                    Message = ReturnMessagesConstants.User.RegistrationFailed,
                    Error = ex.Message
                });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var resetToken = GenerateAlphanumericToken(8);
                _logger.LogInformation($"Processing password reset for: {forgotPasswordDto.Email}");
                var htmlEmailBody = _emailTemplates.GetPasswordResetTemplate(resetToken);
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
                        ? ReturnMessagesConstants.User.PasswordResetEmailSent 
                        : ReturnMessagesConstants.User.PasswordResetEmailFailed,
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
                    Message = ReturnMessagesConstants.User.PasswordResetFailed,
                    Error = ex.Message
                });
            }
        }

        private string GenerateAlphanumericToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}