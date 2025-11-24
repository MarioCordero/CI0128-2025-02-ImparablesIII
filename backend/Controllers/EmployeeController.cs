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
        private readonly IEmailTemplates _emailTemplates;
        private readonly IEmployeeDeletionService _employeeDeletionService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUsuarioRepository _usuarioRepository; // QUITAR DE AQUIII

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
        public async Task<ActionResult<RegisterEmployeeResponseDto>> RegisterEmployee([FromBody] RegisterEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Dato inválido." : e.ErrorMessage)
                    .ToList();

                return BadRequest(new RegisterEmployeeResponseDto
                {
                    Success = false,
                    Message = "Errores de validación.",
                    ValidationErrors = errors
                });
            }

            try
            {
                var employeeId = await _employeeService.RegisterEmployeeAsync(employeeDto);
                if (employeeId <= 0)
                {
                    return BadRequest(new RegisterEmployeeResponseDto
                    {
                        Success = false,
                        Message = EmployeeConstants.Employee.EmployeeRegistrationFailed
                    });
                }
                return Ok(new RegisterEmployeeResponseDto
                {
                    Success = true,
                    Message = EmployeeConstants.Employee.EmployeeRegisteredSuccessfully,
                    EmployeeId = employeeId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering employee");
                return StatusCode(500, new RegisterEmployeeResponseDto
                {
                    Success = false,
                    Message = ReturnMessagesConstants.General.InternalServerError
                });
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
            [FromBody] DeleteEmployeeRequestDto request)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de eliminación para empleado {EmployeeId} solicitado por {EmployerId}", employeeId, request.EmployerId);
                if (!ModelState.IsValid)
                    return BadRequest(new DeleteEmployeeResponseDto { Success = false, Message = "Datos inválidos." });

                if (request.EmployerId <= 0)
                    return BadRequest(new DeleteEmployeeResponseDto { Success = false, Message = "EmployerId inválido." });

                if (string.IsNullOrEmpty(request.Contrasena))
                    return BadRequest(new DeleteEmployeeResponseDto { Success = false, Message = "La contraseña es requerida." });

                var employer = await _usuarioRepository.GetUserByIdAsync(request.EmployerId);
                if (employer == null)
                {
                    _logger.LogWarning("Empleador {EmployerId} no encontrado", request.EmployerId);
                    return NotFound(new DeleteEmployeeResponseDto { Success = false, Message = "Empleador no encontrado." });
                }

                if (employer.TipoUsuario != "Empleador" && employer.TipoUsuario != "Administrador")
                {
                    _logger.LogWarning("Usuario {EmployerId} sin permisos. Tipo: {Tipo}", request.EmployerId, employer.TipoUsuario);
                    return StatusCode(403, new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "Permisos insuficientes."
                    });
                }

                _logger.LogInformation("Solicitud eliminación empleado {EmployeeId} por {EmployerId}", employeeId, request.EmployerId);
                var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, request.EmployerId, request);

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