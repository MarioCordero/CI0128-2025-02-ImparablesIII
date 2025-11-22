using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;
using backend.Repositories;
using backend.Constants;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPasswordSetupService _passwordSetupService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplates _emailTemplates; // Asegúrese de tener esta interfaz registrada
        private readonly IEmployeeDeletionService _employeeDeletionService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeDeletionService employeeDeletionService,
            IUsuarioRepository usuarioRepository,
            IEmployeeService employeeService,
            ILogger<EmployeeController> logger,
            IEmailService emailService,
            IPasswordSetupService passwordSetupService,
            IConfiguration configuration,
            IEmailTemplates emailTemplates)
        {
            _employeeDeletionService = employeeDeletionService;
            _usuarioRepository = usuarioRepository;
            _employeeService = employeeService;
            _logger = logger;
            _emailService = emailService;
            _passwordSetupService = passwordSetupService;
            _configuration = configuration;
            _emailTemplates = emailTemplates;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto employeeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cedulaExists = await _employeeService.ValidateCedulaExistsAsync(employeeDto.Cedula);
                if (cedulaExists)
                    return BadRequest(new { message = ReturnMessagesConstants.Employee.CedulaAlreadyRegistered });

                var emailExists = await _employeeService.ValidateEmailExistsAsync(employeeDto.Correo);
                if (emailExists)
                    return BadRequest(new { message = ReturnMessagesConstants.General.EmailAlreadyExists });

                var employeeId = await _employeeService.RegisterEmployeeAsync(employeeDto);
                _logger.LogInformation("Employee registered successfully with ID: {EmployeeId}", employeeId);

                var token = await _passwordSetupService.GeneratePasswordSetupTokenAsync(employeeId, employeeDto.Correo);
                await SendPasswordSetupEmailAsync(employeeDto.PrimerNombre, employeeDto.Correo, token);

                return Ok(new
                {
                    message = ReturnMessagesConstants.Employee.EmployeeRegisteredSuccessfully,
                    employeeId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering employee: {Message}", ex.Message);
                return StatusCode(500, new { message = string.Format(ReturnMessagesConstants.General.InternalServerErrorWithDetail, ex.Message) });
            }
        }

        [HttpGet("validate-cedula/{cedula}")]
        public async Task<IActionResult> ValidateCedula(string cedula)
        {
            try
            {
                var exists = await _employeeService.ValidateCedulaExistsAsync(cedula);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating cedula");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpGet("validate-email/{email}")]
        public async Task<IActionResult> ValidateEmail(string email)
        {
            try
            {
                var exists = await _employeeService.ValidateEmailExistsAsync(email);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating email");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult<DeleteEmployeeResponseDto>> DeleteEmployee(
            int employeeId,
            [FromBody] DeleteEmployeeRequestDto request,
            [FromQuery] int employerId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrEmpty(request.Contrasena))
                    return BadRequest(new DeleteEmployeeResponseDto { Success = false, Message = "La contraseña es requerida para confirmar la eliminación." });

                if (employerId <= 0)
                    return BadRequest(new DeleteEmployeeResponseDto { Success = false, Message = "ID de empleador inválido." });

                var employer = await _usuarioRepository.GetUserByIdAsync(employerId);
                if (employer == null)
                {
                    _logger.LogWarning("Empleador {EmployerId} no encontrado", employerId);
                    return NotFound(new DeleteEmployeeResponseDto { Success = false, Message = "Empleador no encontrado." });
                }

                if (employer.TipoUsuario != "Empleador" && employer.TipoUsuario != "Administrador")
                {
                    _logger.LogWarning("Usuario {EmployerId} sin permisos. Tipo: {Tipo}", employerId, employer.TipoUsuario);
                    return StatusCode(403, new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "No tiene permisos para eliminar empleados. Solo Empleador o Administrador."
                    });
                }

                _logger.LogInformation("Solicitud eliminación empleado {EmployeeId} por {EmployerId}", employeeId, employerId);
                var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

                if (result.Success)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Empleado {EmployeeId} no encontrado", employeeId);
                return NotFound(new DeleteEmployeeResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado {EmployeeId}", employeeId);
                return StatusCode(500, new DeleteEmployeeResponseDto { Success = false, Message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpGet("{employeeId}/deletion-info")]
        public async Task<ActionResult<EmployeeDeletionInfoDto>> GetEmployeeDeletionInfo(int employeeId)
        {
            try
            {
                var info = await _employeeDeletionService.GetEmployeeDeletionInfoAsync(employeeId);
                return Ok(info);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Empleado {EmployeeId} no encontrado", employeeId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información eliminación empleado {EmployeeId}", employeeId);
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        private async Task SendPasswordSetupEmailAsync(string employeeName, string email, string token)
        {
            try
            {
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:8080";
                var setupUrl = $"{frontendUrl}/password-setup?token={token}";

                var emailBody = _emailTemplates.GetPasswordSetupTemplate(employeeName, setupUrl);

                var emailDto = new SendEmailDto
                {
                    ReceiverEmail = email,
                    Subject = "Configuración de Contraseña - Imparables",
                    Body = emailBody,
                    IsHtml = true
                };

                var result = await _emailService.SendEmailAsync(emailDto);

                if (result.Success)
                    _logger.LogInformation("Password setup email enviado a {Email}", email);
                else
                    _logger.LogError("Fallo enviando email a {Email}: {Msg}", email, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de configuración a {Email}", email);
            }
        }
    }
}