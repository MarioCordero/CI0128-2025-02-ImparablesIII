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

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var available = await _employerService.IsEmailAvailableAsync(email);
            return Ok(new { available });
        }

        [HttpGet("check-cedula/{cedula}")]
        public async Task<IActionResult> CheckCedula(string cedula)
        {
            var available = await _employerService.IsCedulaAvailableAsync(cedula);
            return Ok(new { available });
        }

        // TODO
        // [HttpPost("resend-verification")]
        // public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequestDto req)
        // {
        //     if (string.IsNullOrWhiteSpace(req.Email))
        //         return BadRequest(new { success = false, message = EmployerConstants.Validation.EmailRequired });

        //     var ok = await _employerService.ResendVerificationAsync(req.Email);
        //     if (!ok)
        //         return BadRequest(new { success = false, message = EmployerConstants.Employer.VerificationResentFailed });

        //     return Ok(new { success = true, message = EmployerConstants.Employer.VerificationResentSuccess });
        // }

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

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Code))
                return BadRequest(new { success = false, message = EmployerConstants.Validation.EmailAndCodeRequired });

            var ok = await _verificationService.VerifyCodeAsync(req.Email, req.Code);
            if (!ok)
                return BadRequest(new { success = false, message = EmployerConstants.Employer.CodeInvalidOrExpired });

            return Ok(new { success = true, message = EmployerConstants.Employer.CodeVerified });
        }
    }
}