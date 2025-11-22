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
        private readonly IEmployeeDeletionService _employeeDeletionService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordSetupService _passwordSetupService;
        private readonly IConfiguration _configuration;

        public EmployeeController(
            IEmployeeService employeeService,
            IEmployeeDeletionService employeeDeletionService,
            IUsuarioRepository usuarioRepository,
            ILogger<EmployeeController> logger,
            IEmailService emailService,
            IPasswordSetupService passwordSetupService,
            IConfiguration configuration)
        {
            _employeeService = employeeService;
            _employeeDeletionService = employeeDeletionService;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _emailService = emailService;
            _passwordSetupService = passwordSetupService;
            _configuration = configuration;
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
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(request.Contrasena))
                {
                    return BadRequest(new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "La contraseña es requerida para confirmar la eliminación."
                    });
                }

                if (employerId <= 0)
                {
                    return BadRequest(new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "ID de empleador inválido."
                    });
                }

                var employer = await _usuarioRepository.GetUserByIdAsync(employerId);
                if (employer == null)
                {
                    _logger.LogWarning("Empleador {EmployerId} no encontrado", employerId);
                    return NotFound(new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "Empleador no encontrado."
                    });
                }

                if (employer.TipoUsuario != "Empleador" && employer.TipoUsuario != "Administrador")
                {
                    _logger.LogWarning("Usuario {EmployerId} no tiene permisos para eliminar empleados. Tipo: {TipoUsuario}", employerId, employer.TipoUsuario);
                    return StatusCode(403, new DeleteEmployeeResponseDto
                    {
                        Success = false,
                        Message = "No tiene permisos para eliminar empleados. Solo usuarios con rol de Empleador o Administrador pueden realizar esta acción."
                    });
                }

                _logger.LogInformation("Solicitud de eliminación de empleado {EmployeeId} por empleador {EmployerId}", employeeId, employerId);

                var result = await _employeeDeletionService.DeleteEmployeeAsync(employeeId, employerId, request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Empleado {EmployeeId} no encontrado", employeeId);
                return NotFound(new DeleteEmployeeResponseDto
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado {EmployeeId}", employeeId);
                return StatusCode(500, new DeleteEmployeeResponseDto
                {
                    Success = false,
                    Message = ReturnMessagesConstants.General.InternalServerError
                });
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
                _logger.LogError(ex, "Error al obtener información de eliminación para empleado {EmployeeId}", employeeId);
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployer([FromBody] SignUpEmployerDto form)
        {
            try
            {
                var result = await _employerService.RegisterEmployerAsync(form);

                if (result)
                    return Ok(new { message = "Registration successful" });
                else
                    return BadRequest(new { message = "Registration failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterEmployer endpoint");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        // TODO: Refactor email verification to use a single endpoint for both verifying and creating the user
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyAndCreateUserDto dto)
        {
            try
            {
                var (isValid, personaId) = await _verificationService.VerifyTokenAsync(dto.Email, dto.Token);   
                if (!isValid)
                    return BadRequest(new { message = "Token inválido o expirado" });
                var userCreated = await _employerService.VerifyAndCreateUserAsync(personaId, dto.Password);
                if (!userCreated)
                    return StatusCode(500, new { message = "Error al crear usuario" });
                return Ok(new
                {
                    message = "Email verificado. Usuario creado exitosamente. Ya puedes ingresar.",
                    personaId = personaId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyEmail endpoint");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var isAvailable = await _employerService.IsEmailAvailableAsync(email);
            return Ok(new { email, isAvailable });
        }

        [HttpGet("check-cedula/{cedula}")]
        public async Task<IActionResult> CheckCedula(string cedula)
        {
            var isAvailable = await _employerService.IsCedulaAvailableAsync(cedula);
            return Ok(new { cedula, isAvailable });
        }
    }
}

