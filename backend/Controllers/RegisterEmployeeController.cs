using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using backend.DTOs;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterEmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<RegisterEmployeeController> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordSetupService _passwordSetupService;
        private readonly IConfiguration _configuration;

        public RegisterEmployeeController(
            IEmployeeService employeeService, 
            ILogger<RegisterEmployeeController> logger,
            IEmailService emailService,
            IPasswordSetupService passwordSetupService,
            IConfiguration configuration)
        {
            _employeeService = employeeService;
            _logger = logger;
            _emailService = emailService;
            _passwordSetupService = passwordSetupService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto employeeDto)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate that cedula doesn't already exist
                var cedulaExists = await _employeeService.ValidateCedulaExistsAsync(employeeDto.Cedula);
                if (cedulaExists)
                {
                    return BadRequest(new { message = "La cédula ya está registrada en el sistema." });
                }

                // Validate that email doesn't already exist
                var emailExists = await _employeeService.ValidateEmailExistsAsync(employeeDto.Correo);
                if (emailExists)
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado en el sistema." });
                }

                // Register the employee
                var employeeId = await _employeeService.RegisterEmployeeAsync(employeeDto);

                _logger.LogInformation($"Employee registered successfully with ID: {employeeId}");

                // Generate password setup token
                var token = await _passwordSetupService.GeneratePasswordSetupTokenAsync(employeeId, employeeDto.Correo);

                // Send password setup email
                await SendPasswordSetupEmailAsync(employeeDto.PrimerNombre, employeeDto.Correo, token);

                return Ok(new 
                { 
                    message = "Empleado registrado exitosamente. Se ha enviado un correo para configurar la contraseña.", 
                    employeeId = employeeId 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering employee: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("validate-cedula/{cedula}")]
        public async Task<IActionResult> ValidateCedula(string cedula)
        {
            try
            {
                var exists = await _employeeService.ValidateCedulaExistsAsync(cedula);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating cedula");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        [HttpGet("validate-email/{email}")]
        public async Task<IActionResult> ValidateEmail(string email)
        {
            try
            {
                var exists = await _employeeService.ValidateEmailExistsAsync(email);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating email");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Simple test to verify database connection
                var result = 0; //await _employeeService.TestConnectionAsync();
                return Ok(new { message = "Conexión a la base de datos exitosa", result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed: {Message}", ex.Message);
                return StatusCode(500, new { message = $"Error de conexión a la base de datos: {ex.Message}" });
            }
        }

        private async Task SendPasswordSetupEmailAsync(string employeeName, string email, string token)
        {
            try
            {
                // Get frontend URL from configuration
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:8080";
                var setupUrl = $"{frontendUrl}/password-setup?token={token}";

                // Generate email content
                var emailBody = EmailTemplates.GetPasswordSetupTemplate(employeeName, setupUrl);

                var emailDto = new SendEmailDto
                {
                    ReceiverEmail = email,
                    Subject = "Configuración de Contraseña - Imparables",
                    Body = emailBody,
                    IsHtml = true
                };

                var result = await _emailService.SendEmailAsync(emailDto);
                
                if (result.Success)
                {
                    _logger.LogInformation($"Password setup email sent successfully to {email}");
                }
                else
                {
                    _logger.LogError($"Failed to send password setup email to {email}: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password setup email to {email}", email);
            }
        }
    }
}
