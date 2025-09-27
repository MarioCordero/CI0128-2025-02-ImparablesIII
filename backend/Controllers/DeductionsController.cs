using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/deductions")]
    public class DeductionsController : ControllerBase
    {
        private readonly ExternalApiService _externalApi;

        public DeductionsController(ExternalApiService externalApi)
        {
            _externalApi = externalApi;
        }

        // GET /api/deductions/hacienda/{salarioBruto}?token=XXX
        [HttpGet("hacienda/{salarioBruto}")]
        public async Task<IActionResult> GetHacienda(decimal salarioBruto, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token)) return Forbid("Token is required");
            try
            {
                var result = await _externalApi.GetHaciendaDeduccionAsync(salarioBruto, token);
                return Ok(result.RootElement);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/deductions/ccss/{salarioBruto}?token=XXX
        [HttpGet("ccss/{salarioBruto}")]
        public async Task<IActionResult> GetCCSS(decimal salarioBruto, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token)) return Forbid("Token is required");
            try
            {
                var result = await _externalApi.GetCCSSDeduccionAsync(salarioBruto, token);
                return Ok(result.RootElement);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET /api/deductions/asociacion/{salarioBruto}/{cedula}?token=XXX
        [HttpGet("asociacion/{salarioBruto}/{cedula}")]
        public async Task<IActionResult> GetAsociacion(decimal salarioBruto, string cedula, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token)) return Forbid("Token is required");
            try
            {
                var result = await _externalApi.GetAsociacionAporteAsync(salarioBruto, cedula, token);
                return Ok(result.RootElement);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
