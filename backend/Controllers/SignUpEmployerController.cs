using Microsoft.AspNetCore.Mvc;

namespace backend_lab_c28730.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpEmployerController : ControllerBase
    {
        // GET endpoint (keep it for testing)
        [HttpGet]
        public string Get()
        {
            return "SignUpEmployerController is working!";
        }

        // POST endpoint to receive form data
        [HttpPost]
        public IActionResult RegisterEmployer([FromBody] SignUpEmployerDto form)
        {
            // TODO: Add validation, save to database, send verification email, etc.

            // For now, just return OK with the received data
            return Ok(new { message = "Data received successfully", data = form });
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
