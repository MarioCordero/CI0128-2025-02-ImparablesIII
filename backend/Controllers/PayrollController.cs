using backend.DTOs;
using backend.Services;
using backend.Constants;
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
                return BadRequest(ReturnMessagesConstants.Validation.CompanyIdInvalid);

            if (request.ResponsibleEmployeeId <= 0)
                return BadRequest(ReturnMessagesConstants.Validation.ResponsibleEmployeeIdInvalid);

            if (request.Hours <= 0)
                return BadRequest(ReturnMessagesConstants.Validation.HoursMustBeGreaterThanZero);

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
                    Message = ReturnMessagesConstants.Payroll.PayrollGeneratedSuccessfully,
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Payroll.ErrorGeneratingPayroll, error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<PayrollTotalsDto?>> GetLatestPayrollSummary([FromQuery] int companyId)
        {
            if (companyId <= 0)
                return BadRequest(ReturnMessagesConstants.Validation.CompanyIdInvalid);

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
                return BadRequest(ReturnMessagesConstants.Validation.CompanyIdInvalid);

            var history = await _service.GetPayrollHistoryByCompanyAsync(companyId);
            return Ok(history);
        }

        // GET EMPLOYEES FOR PAYROLL
        [HttpGet("{payrollId}/employees")]
        public async Task<ActionResult<List<EmployeePayrollDto>>> GetEmployeesForPayroll(int payrollId)
        {
            if (payrollId <= 0)
                return BadRequest("ID de planilla inválido");

            var employees = await _service.GetEmployeesForPayrollAsync(payrollId);
            return Ok(employees);
        }
    }
}
