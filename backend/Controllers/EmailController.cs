using backend.DTOs;
using backend.Services;
using backend.Constants;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IEmailVerificationService _verificationService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailService emailService, 
            IEmailVerificationService verificationService,
            ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _verificationService = verificationService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<EmailResponseDto>> SendEmail([FromBody] SendEmailDto emailDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid email request: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Attempting to send email to {emailDto.ReceiverEmail} with subject: {emailDto.Subject}");
            var result = await _emailService.SendEmailAsync(emailDto);
            
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("send-verification")]
        public async Task<ActionResult<EmailResponseDto>> SendVerificationEmail([FromBody] SendVerificationEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation($"Sending verification email to {dto.Email}");
            var success = await _verificationService.SendVerificationEmailAsync(dto);
            
            return success ? Ok(new { Message = "Correo de verificación enviado" }) : StatusCode(500, new { Message = "Error al enviar correo" });
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (isValid, personaId) = await _verificationService.VerifyTokenAsync(dto.Email, dto.Token);
            
            if (!isValid)
                return BadRequest(new { Message = "Token inválido o expirado" });

            return Ok(new { Message = "Email verificado correctamente", PersonaId = personaId });
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = ReturnMessagesConstants.Email.EmailServiceRunning, Timestamp = DateTime.UtcNow });
        }
    }
}