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

        [HttpGet("report")]
        public async Task<ActionResult<PayrollSummaryDto>> GetPayrollReport([FromQuery] PayrollFiltersDto filters)
        {
            if (filters.CompanyId <= 0)
                return BadRequest("CompanyId is required.");

            var result = await _service.GetReportAsync(filters);
            return Ok(result);
        }
    }
}
