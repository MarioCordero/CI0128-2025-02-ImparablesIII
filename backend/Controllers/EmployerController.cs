using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;
using backend.Constants;
using backend.Repositories;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerController : ControllerBase
    {
        private readonly IEmployerService _employerService;
        private readonly IEmailVerificationService _verificationService;
        private readonly ILogger<EmployerController> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public EmployerController(
            IEmployerService employerService,
            IEmailVerificationService verificationService,
            ILogger<EmployerController> logger,
            IUsuarioRepository usuarioRepository)
        {
            _employerService = employerService;
            _verificationService = verificationService;
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        // REGISTER AN EMPLOYER
        [HttpPost("register")]
        public async Task<IActionResult> RegisterEmployer([FromBody] SignUpEmployerDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? EmployerConstants.Validation.InvalidData : e.ErrorMessage)
                    .ToList();
                return BadRequest(new { success = false, message = EmployerConstants.Validation.ValidationErrors, errors });
            }

            try
            {
                var ok = await _employerService.RegisterEmployerAsync(dto);
                if (!ok)
                    return BadRequest(new { success = false, message = EmployerConstants.Employer.EmployerRegistrationFailed });
                return Ok(new { success = true, message = EmployerConstants.Employer.EmployerRegisteredSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando empleador");
                return StatusCode(500, new { success = false, message = EmployerConstants.General.InternalServerError });
            }
        }

        // CHECK IF EMAIL IS AVAILABLE
        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var available = await _employerService.IsEmailAvailableAsync(email);
            return Ok(new { available });
        }

        // CHECK IF CEDULA IS AVAILABLE
        [HttpGet("check-cedula/{cedula}")]
        public async Task<IActionResult> CheckCedula(string cedula)
        {
            var available = await _employerService.IsCedulaAvailableAsync(cedula);
            return Ok(new { available });
        }

        // VERIFY EMPLOYER
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyEmployerRequestDto req)
        {
            if (req.PersonaId <= 0 || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { success = false, message = EmployerConstants.Validation.PersonaIdPasswordRequired });

            var ok = await _employerService.VerifyAndCreateUserAsync(req.PersonaId, req.Password);
            if (!ok)
                return BadRequest(new { success = false, message = EmployerConstants.Employer.VerificationFailed });

            return Ok(new { success = true, message = EmployerConstants.Employer.VerificationSuccess });
        }

        // VERIFY LINK TOKEN
        [HttpPost("verify-link-token")]
        public async Task<IActionResult> VerifyLinkToken([FromBody] VerifyLinkTokenRequestDto req)
        {
            var (success, personaId, rol) = await _verificationService.VerifyLinkTokenAsync(req.Token);
            
            if (!success)
                return BadRequest(new { success = false, message = "Token inválido o expirado" });

            return Ok(new { 
                success = true, 
                personaId = personaId, 
                rol = rol,
                message = "Token válido" 
            });
        }

        // GET KPI DATA (Key Performance Indicators)
        [HttpGet("kpi")]
        public async Task<IActionResult> GetKPI([FromQuery] int userId)
        {
            try
            {
                var kpi = await _employerService.GetKPIAsync(userId);
                if (kpi == null)
                    return NotFound(new { success = false, message = "No KPI data found." });
                return Ok(new
                {
                    totalCompanies = kpi.TotalCompanies,
                    totalActiveEmployees = kpi.TotalActiveEmployees,
                    totalPayroll = kpi.TotalPayroll,
                    companiesWithPayroll = kpi.CompaniesWithPayroll
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching KPI data");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}