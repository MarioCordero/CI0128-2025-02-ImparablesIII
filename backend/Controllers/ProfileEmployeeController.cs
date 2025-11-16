using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using backend.Services;
using backend.DTOs;
using backend.Constants;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class ProfileEmployeeController : ControllerBase
    {
        private readonly IProfileEmployeeService _profileService;
        private readonly ILogger<ProfileEmployeeController> _logger;

        public ProfileEmployeeController(
            IProfileEmployeeService profileService,
            ILogger<ProfileEmployeeController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProfileEmployeeResponseDto>> GetEmployeeProfile(int id)
        {
            try
            {
                _logger.LogInformation("Solicitud de perfil para empleado {EmployeeId}", id);

                var profile = await _profileService.GetEmployeeProfileAsync(id);
                
                _logger.LogInformation("Perfil enviado exitosamente para empleado {EmployeeId}", id);
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Argumento inválido para empleado {EmployeeId}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Empleado no encontrado: {EmployeeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno obteniendo perfil del empleado {EmployeeId}", id);
                return StatusCode(500, new { 
                    message = ReturnMessagesConstants.General.InternalServerError,
                    detail = ex.Message 
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UpdateEmployeeProfileResponseDto>> UpdateEmployeeProfile(
            int id,
            [FromBody] UpdateEmployeeProfileRequestDto updateRequest)
        {
            try
            {
                _logger.LogInformation("Solicitud de actualización de perfil para empleado {EmployeeId}", id);

                var result = await _profileService.UpdateEmployeeProfileAsync(id, updateRequest);

                if (result.Success)
                {
                    _logger.LogInformation("Perfil actualizado exitosamente para empleado {EmployeeId}", id);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Error actualizando perfil para empleado {EmployeeId}: {Message}", id, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno actualizando perfil del empleado {EmployeeId}", id);
                return StatusCode(500, new UpdateEmployeeProfileResponseDto
                {
                    Success = false,
                    Message = string.Format(ReturnMessagesConstants.General.InternalServerErrorWithDetail, ex.Message)
                });
            }
        }

        /*
        [HttpGet("me")]
        public async Task<ActionResult<ProfileEmployeeResponseDto>> GetMyProfile()
        {
            try
            {
                // Si quitaste [Authorize], este endpoint no funcionará sin autenticación
                // Puedes comentarlo temporalmente también o quitar la lógica del token
                
                _logger.LogWarning("Endpoint /me requiere autenticación, pero [Authorize] está deshabilitado");
                return BadRequest(new { message = "Este endpoint requiere autenticación" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo perfil propio");
                return StatusCode(500, new { 
                    message = "Error interno del servidor",
                    detail = ex.Message 
                });
            }
        }
    */
    }
}