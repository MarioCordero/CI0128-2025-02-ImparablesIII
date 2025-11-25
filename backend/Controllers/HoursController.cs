using System;
using System.Collections.Generic;
using backend.Constants;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class HoursController : ControllerBase
	{
		private readonly IHoursService _hoursService;
		private readonly ILogger<HoursController> _logger;

		public HoursController(IHoursService hoursService, ILogger<HoursController> logger)
		{
			_hoursService = hoursService;
			_logger = logger;
		}

		[HttpPost]
		public async Task<ActionResult<RegisterHoursResponseDto>> RegisterHours([FromBody] RegisterHoursRequestDto request)
		{
			if (request == null)
			{
				return BadRequest(ReturnMessagesConstants.General.InvalidRequest);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var result = await _hoursService.RegisterHoursAsync(request);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Invalid request when registering hours for employee {EmployeeId}", request.EmployeeId);
				return BadRequest(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				if (string.Equals(ex.Message, ReturnMessagesConstants.Hours.DailyLimitExceeded, StringComparison.OrdinalIgnoreCase))
				{
					_logger.LogWarning(ex, "Daily hours limit exceeded for employee {EmployeeId}", request.EmployeeId);
					return BadRequest(new { message = ex.Message });
				}

				_logger.LogWarning(ex, "Cannot resolve approver for employee {EmployeeId}", request.EmployeeId);
				return StatusCode(409, new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error registering hours for employee {EmployeeId}", request.EmployeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/summary")]
		public async Task<ActionResult<HoursSummaryDto>> GetSummary(int employeeId)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var summary = await _hoursService.GetSummaryAsync(employeeId);
				return Ok(summary);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Invalid employeeId {EmployeeId} for summary", employeeId);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching summary for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/recent")]
		public async Task<ActionResult<List<HoursEntryDto>>> GetRecentEntries(int employeeId, [FromQuery] int limit = 6)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var entries = await _hoursService.GetRecentEntriesAsync(employeeId, limit);
				return Ok(entries);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Invalid request for recent entries of employee {EmployeeId}", employeeId);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching recent entries for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/weekly")]
		public async Task<ActionResult<object>> GetWeeklyHours(int employeeId)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var hours = await _hoursService.GetWeeklyHoursAsync(employeeId);
				return Ok(new { weeklyHours = hours });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching weekly hours for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/monthly")]
		public async Task<ActionResult<object>> GetMonthlyHours(int employeeId)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var hours = await _hoursService.GetMonthlyHoursAsync(employeeId);
				return Ok(new { monthlyHours = hours });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching monthly hours for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/total-entries")]
		public async Task<ActionResult<object>> GetTotalEntries(int employeeId)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var total = await _hoursService.GetTotalEntriesAsync(employeeId);
				return Ok(new { totalEntries = total });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching total entries for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}

		[HttpGet("{employeeId}/last-entry")]
		public async Task<ActionResult<HoursEntryDto>> GetLastEntry(int employeeId)
		{
			if (employeeId <= 0)
			{
				return BadRequest(ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero);
			}

			try
			{
				var last = await _hoursService.GetLastEntryAsync(employeeId);
				return last is null ? NotFound() : Ok(last);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching last entry for employee {EmployeeId}", employeeId);
				return StatusCode(500, new { message = ReturnMessagesConstants.General.InternalServerError });
			}
		}
	}
}
