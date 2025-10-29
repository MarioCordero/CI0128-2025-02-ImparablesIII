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

        [HttpGet("employee-deductions")]
        public async Task<ActionResult<List<EmployeePayrollDto>>> GetEmployeePayrollWithDeductions([FromQuery] int companyId)
        {
            if (companyId <= 0)
                return BadRequest("CompanyId inválido.");

            var result = await _service.GetEmployeePayrollWithDeductionsAsync(companyId);
            return Ok(result);
        }

        [HttpGet("employer-deductions")]
        public async Task<ActionResult<List<EmployerDeductionResultDto>>> GetEmployerPayrollWithDeductions([FromQuery] int companyId)
        {
            if (companyId <= 0)
                return BadRequest("CompanyId inválido.");

            var result = await _service.GetEmployerPayrollWithDeductionsAsync(companyId);
            return Ok(result);
        }

    }
}
