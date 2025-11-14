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
        private readonly ILogger<SignUpEmployerController> _logger;

        public SignUpEmployerController(IEmployerService employerService, ILogger<SignUpEmployerController> logger)
        {
            _employerService = employerService;
            _logger = logger;
        }

        // GET endpoint (keep it for testing)
        [HttpGet]
        public string Get()
        {
            return "SignUpEmployerController is working!";
        }

        // POST endpoint to receive form data
        [HttpPost]
        public async Task<IActionResult> RegisterEmployer([FromBody] SignUpEmployerDto form)
        {
            try
            {
                var result = await _employerService.RegisterEmployerAsync(form);

                if (result.IsSuccess)
                {
                    // Return success with the response DTO
                    return Ok(new 
                    { 
                        message = result.Message,
                        employer = result.Data
                    });
                }
                else
                {
                    return BadRequest(new { message = result.Message, errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterEmployer endpoint");
                return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
            }
        }

        // Additional endpoints for validation
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
