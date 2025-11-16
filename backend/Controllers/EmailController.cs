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
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send an email to the specified recipient
        /// </summary>
        /// <param name="emailDto">Email details including recipient, subject, and body</param>
        /// <returns>Response indicating success or failure of email sending</returns>
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
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Health check endpoint for the email service
        /// </summary>
        /// <returns>Status of the email service</returns>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = ReturnMessagesConstants.Email.EmailServiceRunning, Timestamp = DateTime.UtcNow });
        }
    }
}
