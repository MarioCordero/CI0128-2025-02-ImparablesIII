using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/payroll")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _service;
        public PayrollController(IPayrollService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<GeneratePayrollResponseDto>> GeneratePayrollWithBenefits([FromBody] GeneratePayrollRequestDto request)
        {
            if (request.CompanyId <= 0)
                return BadRequest("CompanyId inválido.");

            if (request.ResponsibleEmployeeId <= 0)
                return BadRequest("ResponsibleEmployeeId inválido.");

            if (request.Hours <= 0)
                return BadRequest("Hours debe ser mayor que cero.");

            try
            {
                var payrollId = await _service.GeneratePayrollWithBenefitsAsync(
                    request.CompanyId,
                    request.ResponsibleEmployeeId,
                    request.Hours,
                    request.PeriodType,
                    request.Fortnight);

                var response = new GeneratePayrollResponseDto
                {
                    PayrollId = payrollId,
                    Message = "Planilla generada exitosamente con beneficios y deducciones.",
                    GeneratedAt = DateTime.Now
                };

                return Ok(response);
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(new { message = ioe.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar la planilla.", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<PayrollTotalsDto?>> GetLatestPayrollSummary([FromQuery] int companyId)
        {
            if (companyId <= 0)
                return BadRequest("CompanyId inválido.");

            var totals = await _service.GetLatestPayrollTotalsByCompanyAsync(companyId);
            if (totals == null)
            {
                return NotFound();
            }
            return Ok(totals);
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<PayrollHistoryItemDto>>> GetPayrollHistory([FromQuery] int companyId)
        {
            if (companyId <= 0)
                return BadRequest("CompanyId inválido.");

            var history = await _service.GetPayrollHistoryByCompanyAsync(companyId);
            return Ok(history);
        }

    }
}
