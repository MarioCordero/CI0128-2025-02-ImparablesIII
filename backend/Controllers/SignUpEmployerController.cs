using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend_lab_c28730.Controllers
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
                    // Return success without sensitive data
                    return Ok(new 
                    { 
                        message = result.Message,
                        employer = new
                        {
                            id = result.Data!.Id,
                            username = result.Data.Username,
                            email = result.Data.Email,
                            nombre = result.Data.Nombre,
                            createdAt = result.Data.CreatedAt
                        }
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
                return StatusCode(500, new { message = "An internal error occurred" });
            }
        }

        // Additional endpoints for validation
        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var isAvailable = await _employerService.IsUsernameAvailableAsync(username);
            return Ok(new { username, isAvailable });
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var isAvailable = await _employerService.IsEmailAvailableAsync(email);
            return Ok(new { email, isAvailable });
        }
    }

    // DTO class for receiving form data
    public class SignUpEmployerDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
