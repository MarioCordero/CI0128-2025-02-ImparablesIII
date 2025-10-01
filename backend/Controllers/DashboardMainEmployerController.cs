using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

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
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employerId))
                return Unauthorized();

            var dashboard = await _dashboardMainEmployerService.GetDashboardMainEmployer(employerId);
            return Ok(dashboard);
        }
    }
}