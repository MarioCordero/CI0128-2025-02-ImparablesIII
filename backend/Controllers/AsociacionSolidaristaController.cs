using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/asociacionsolidarista")]
    public class AsociacionSolidaristaController : ControllerBase
    {
        // This controller only handles the HTTP request and response. (SRP)
        private readonly IAuthenticationService _authService;
        private readonly IAporteSolidaristaCalculatorService _calculatorService;

        public AsociacionSolidaristaController(
            IAuthenticationService authService, 
            IAporteSolidaristaCalculatorService calculatorService)
        {
            _authService = authService;
            _calculatorService = calculatorService;
        }

        [HttpGet("aporte-empleado")]
        public ActionResult<AporteSolidaristaResponse> GetAporte(
            [FromQuery] string cedulaEmpresa, 
            [FromQuery] decimal salarioBruto)
        {
            // Validate token format and content
            string? authHeader = Request.Headers["Authorization"];
            if (!_authService.IsValidToken(authHeader))
            {
                return Unauthorized("Invalid or missing token");
            }

            // Validate parameters
            string? validationError = _calculatorService.GetValidationError(cedulaEmpresa, salarioBruto);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            // Calculate and return response
            var response = _calculatorService.CalculateAporte(cedulaEmpresa, salarioBruto);
            return Ok(response);
        }
    }
}