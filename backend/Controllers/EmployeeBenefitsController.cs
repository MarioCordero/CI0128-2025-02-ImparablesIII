using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;
using backend.Repositories;
using backend.Constants;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeBenefitsController : ControllerBase
    {
        private readonly IEmployeeBenefitService _employeeBenefitService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeBenefitsController> _logger;

        public EmployeeBenefitsController(
            IEmployeeBenefitService employeeBenefitService,
            IEmployeeService employeeService,
            ILogger<EmployeeBenefitsController> logger)
        {
            _employeeBenefitService = employeeBenefitService;
            _employeeService = employeeService;
            _logger = logger;
        }
        
        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<EmployeeBenefitsSummaryDto>> GetEmployeeBenefits(int employeeId, [FromQuery] string? searchTerm = null, [FromQuery] string? calculationType = null, [FromQuery] string? status = null)
        {
            try
            {
                _logger.LogInformation("Getting benefits for employee {EmployeeId}", employeeId);

                var companyId = await _employeeService.GetEmployeeCompanyIdAsync(employeeId);
                if (!companyId.HasValue)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Employee.EmployeeNotFoundOrNoCompany });
                }

                var filter = new BenefitFilterDto
                {
                    SearchTerm = searchTerm,
                    CalculationType = calculationType,
                    Status = status
                };

                var result = await _employeeBenefitService.GetEmployeeBenefitsAsync(employeeId, companyId.Value, filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting benefits for employee {EmployeeId}", employeeId);
                return StatusCode(500, new { message = ReturnMessagesConstants.Benefit.ErrorRetrievingBenefits, error = ex.Message });
            }
        }

        [HttpPost("employee/{employeeId:int}/select")]
        public async Task<ActionResult<EmployeeBenefitSelectionResponseDto>> SelectBenefit(int employeeId, [FromBody] SelectBenefitRequestDto request)
        {
            try
            {
                _logger.LogInformation("Selecting benefit {BenefitName} for employee {EmployeeId}", request.BenefitName, employeeId);

                var companyId = await _employeeService.GetEmployeeCompanyIdAsync(employeeId);
                if (!companyId.HasValue)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Employee.EmployeeNotFoundOrNoCompany });
                }

                var result = await _employeeBenefitService.SelectBenefitAsync(employeeId, companyId.Value, request);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting benefit for employee {EmployeeId}", employeeId);
                return StatusCode(500, new { message = ReturnMessagesConstants.Benefit.ErrorSelectingBenefit, error = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId:int}/validation")]
        public async Task<ActionResult> ValidateBenefitSelection(int employeeId)
        {
            try
            {
                var companyId = await _employeeService.GetEmployeeCompanyIdAsync(employeeId);
                if (!companyId.HasValue)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Employee.EmployeeNotFoundOrNoCompany });
                }

                var canSelectMore = await _employeeBenefitService.ValidateBenefitSelectionAsync(employeeId, companyId.Value);
                return Ok(new { canSelectMore });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating benefit selection for employee {EmployeeId}", employeeId);
                return StatusCode(500, new { message = ReturnMessagesConstants.Benefit.ErrorValidatingSelection, error = ex.Message });
            }
        }
    }
}

