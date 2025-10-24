using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<PayrollSummaryDto>> GetPayrollSummary(
            [FromQuery] DateTime period,
            [FromQuery] string periodType = "Monthly",
            [FromQuery] int? departmentId = null)
        {
            try
            {
                var filters = new PayrollFiltersDto
                {
                    Period = period,
                    PeriodType = periodType,
                    DepartmentId = departmentId
                };

                var result = await _payrollService.GetPayrollSummaryAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<PayrollDetailDto>> CalculateEmployeePayroll(
            [FromBody] PayrollCalculationRequestDto request)
        {
            try
            {
                var result = await _payrollService.CalculateEmployeePayrollAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("process")]
        public async Task<ActionResult> ProcessPayroll(
            [FromBody] PayrollFiltersDto filters)
        {
            try
            {
                var result = await _payrollService.ProcessPayrollAsync(filters);
                return Ok(new { success = result, message = "Payroll processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("history/{employeeId}")]
        public async Task<ActionResult<IEnumerable<PayrollDetailDto>>> GetPayrollHistory(int employeeId)
        {
            try
            {
                var result = await _payrollService.GetPayrollHistoryAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}