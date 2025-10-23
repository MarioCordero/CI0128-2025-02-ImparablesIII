using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkHoursController : ControllerBase
    {
        private readonly IWorkHoursService _workHoursService;
        private readonly ILogger<WorkHoursController> _logger;

        public WorkHoursController(IWorkHoursService workHoursService, ILogger<WorkHoursController> logger)
        {
            _workHoursService = workHoursService;
            _logger = logger;
        }

        /// <summary>
        /// Register work hours for an employee
        /// </summary>
        /// <param name="request">Work hours registration data</param>
        /// <returns>Success response</returns>
        [HttpPost]
        public async Task<ActionResult<WorkHoursResponseDto>> RegisterWorkHours([FromBody] RegisterWorkHoursDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _workHoursService.RegisterWorkHoursAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid work hours registration attempt");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering work hours");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get work hours summary for an employee
        /// </summary>
        /// <param name="userId">Employee ID</param>
        /// <param name="scope">Summary scope: week, month, or total</param>
        /// <returns>Work hours summary</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<WorkHoursSummaryDto>> GetWorkHoursSummary(
            [FromQuery, Required] string userId, 
            [FromQuery, Required] string scope)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(scope))
                {
                    return BadRequest("UserId and scope are required");
                }

                if (!IsValidScope(scope))
                {
                    return BadRequest("Invalid scope. Must be 'week', 'month', or 'total'");
                }

                var summary = await _workHoursService.GetWorkHoursSummaryAsync(userId, scope);
                return Ok(summary);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid summary request for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting work hours summary for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get recent work hours records for an employee
        /// </summary>
        /// <param name="userId">Employee ID</param>
        /// <param name="limit">Number of records to return (default: 5)</param>
        /// <returns>Recent work hours records</returns>
        [HttpGet("recent")]
        public async Task<ActionResult<RecentWorkHoursDto>> GetRecentWorkHours(
            [FromQuery, Required] string userId, 
            [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                if (limit <= 0 || limit > 50)
                {
                    return BadRequest("Limit must be between 1 and 50");
                }

                var recentRecords = await _workHoursService.GetRecentWorkHoursAsync(userId, limit);
                return Ok(recentRecords);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid recent hours request for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent work hours for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete a work hours record
        /// </summary>
        /// <param name="recordId">Record ID to delete</param>
        /// <param name="userId">Employee ID (for authorization)</param>
        /// <returns>Success response</returns>
        [HttpDelete("{recordId}")]
        public async Task<ActionResult> DeleteWorkHoursRecord(int recordId, [FromQuery, Required] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                var result = await _workHoursService.DeleteWorkHoursRecordAsync(recordId, userId);
                
                if (!result)
                {
                    return NotFound("Work hours record not found or unauthorized");
                }

                return Ok(new { message = "Work hours record deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid delete request for record {RecordId}", recordId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting work hours record {RecordId}", recordId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get work hours for a specific date range
        /// </summary>
        /// <param name="userId">Employee ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Work hours in date range</returns>
        [HttpGet("range")]
        public async Task<ActionResult<List<WorkHoursRecordDto>>> GetWorkHoursByDateRange(
            [FromQuery, Required] string userId,
            [FromQuery, Required] DateTime startDate,
            [FromQuery, Required] DateTime endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("UserId is required");
                }

                if (startDate > endDate)
                {
                    return BadRequest("Start date must be before end date");
                }

                var records = await _workHoursService.GetWorkHoursByDateRangeAsync(userId, startDate, endDate);
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid date range request for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting work hours by date range for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private static bool IsValidScope(string scope)
        {
            return scope.ToLower() is "week" or "month" or "total";
        }
    }
}