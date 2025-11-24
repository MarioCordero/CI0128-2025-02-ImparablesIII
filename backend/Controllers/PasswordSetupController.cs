using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;
using backend.Constants;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordSetupController : ControllerBase
    {
        private readonly IPasswordSetupService _passwordSetupService;
        private readonly ILogger<PasswordSetupController> _logger;

        public PasswordSetupController(
            IPasswordSetupService passwordSetupService, 
            ILogger<PasswordSetupController> logger)
        {
            _passwordSetupService = passwordSetupService;
            _logger = logger;
        }

        [HttpPost("setup")]
        public async Task<IActionResult> SetupPassword([FromBody] PasswordSetupRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _passwordSetupService.SetupPasswordAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting up password");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        [HttpGet("validate-token/{token}")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            try
            {
                var isValid = await _passwordSetupService.ValidateTokenAsync(token);
                return Ok(new { valid = isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating token");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }
    }
}