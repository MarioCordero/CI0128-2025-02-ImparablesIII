using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;
using backend.Constants;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpEmployerController : ControllerBase
    {
        private readonly IEmployerService _employerService;
        private readonly IEmailVerificationService _verificationService;
        private readonly ILogger<SignUpEmployerController> _logger;

        public SignUpEmployerController(
            IEmployerService employerService,
            IEmailVerificationService verificationService,
            ILogger<SignUpEmployerController> logger)
        {
            _employerService = employerService;
            _verificationService = verificationService;
            _logger = logger;
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