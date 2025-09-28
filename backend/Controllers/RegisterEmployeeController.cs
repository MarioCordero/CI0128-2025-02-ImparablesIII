using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Repositories;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterEmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly ILogger<RegisterEmployeeController> _logger;

        public RegisterEmployeeController(EmployeeRepository employeeRepository, ILogger<RegisterEmployeeController> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
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
                var cedulaExists = await _employeeRepository.ValidateCedulaExistsAsync(employeeDto.Cedula);
                if (cedulaExists)
                {
                    return BadRequest(new { message = "La cédula ya está registrada en el sistema." });
                }

                // Validate that email doesn't already exist
                var emailExists = await _employeeRepository.ValidateEmailExistsAsync(employeeDto.Correo);
                if (emailExists)
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado en el sistema." });
                }

                // Register the employee
                var employeeId = await _employeeRepository.RegisterEmployeeAsync(employeeDto);

                _logger.LogInformation($"Employee registered successfully with ID: {employeeId}");

                return Ok(new 
                { 
                    message = "Empleado registrado exitosamente.", 
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
                var exists = await _employeeRepository.ValidateCedulaExistsAsync(cedula);
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
                var exists = await _employeeRepository.ValidateEmailExistsAsync(email);
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
                var result = await _employeeRepository.TestConnectionAsync();
                return Ok(new { message = "Conexión a la base de datos exitosa", result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed: {Message}", ex.Message);
                return StatusCode(500, new { message = $"Error de conexión a la base de datos: {ex.Message}" });
            }
        }
    }
}
