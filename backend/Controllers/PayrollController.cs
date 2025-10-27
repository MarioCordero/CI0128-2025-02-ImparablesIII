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

        // NEW: quick ad-hoc testing endpoint
        // POST /api/payroll/test-employee-deductions
        [HttpPost("test-employee-deductions")] // TEST
        public async Task<ActionResult<TestEmployeeDeductionsResponse>> TestEmployeeDeductions(
            [FromBody] TestEmployeeDeductionsRequest request)
        {
            if (request is null) return BadRequest("Body is required.");
            if (request.GrossSalary <= 0) return BadRequest("GrossSalary must be > 0.");
            var res = await _service.TestEmployeeDeductionsAsync(request);
            return Ok(res);
        }
    }
}
