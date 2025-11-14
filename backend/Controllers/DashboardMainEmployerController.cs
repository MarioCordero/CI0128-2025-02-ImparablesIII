using backend.DTOs;
using backend.Services;
using backend.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardMainEmployerService _dashboardMainEmployerService;

        public DashboardController(IDashboardMainEmployerService dashboardMainEmployerService)
        {
            _dashboardMainEmployerService = dashboardMainEmployerService;
        }

        [HttpGet("employer")]
        public async Task<ActionResult<DashboardMainEmployerDto>> GetDashboardMainEmployer()
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employerIdClaim) || !int.TryParse(employerIdClaim, out int employerId))
            {
                return Unauthorized(ReturnMessagesConstants.Validation.InvalidOrMissingEmployerId);
            }

            try
            {
                var dashboard = await _dashboardMainEmployerService.GetDashboardDataAsync(employerId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Dashboard.ErrorRetrievingDashboardData, error = ex.Message });
            }
        }
    }
}