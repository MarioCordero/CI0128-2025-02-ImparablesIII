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
        private readonly IEmployeeDeletionService _employeeDeletionService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeDeletionService employeeDeletionService,
            IUsuarioRepository usuarioRepository,
            ILogger<EmployeeController> logger)
        {
            _employeeDeletionService = employeeDeletionService;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
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
    }
}

