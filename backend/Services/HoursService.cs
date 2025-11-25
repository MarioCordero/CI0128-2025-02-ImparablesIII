using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
	public class HoursService : IHoursService
	{
		private const string DefaultStatus = "Pendiente";
		private readonly IHoursRepository _hoursRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly ILogger<HoursService> _logger;

		public HoursService(
			IHoursRepository hoursRepository,
			IEmployeeRepository employeeRepository,
			IProjectRepository projectRepository,
			ILogger<HoursService> logger)
		{
			_hoursRepository = hoursRepository;
			_employeeRepository = employeeRepository;
			_projectRepository = projectRepository;
			_logger = logger;
		}

		public async Task<RegisterHoursResponseDto> RegisterHoursAsync(RegisterHoursRequestDto request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			ValidateEmployeeId(request.EmployeeId);

			if (request.Quantity <= 0)
			{
				throw new ArgumentException("Quantity must be greater than zero", nameof(request.Quantity));
			}

			if (string.IsNullOrWhiteSpace(request.Detail))
			{
				throw new ArgumentException("Detail is required", nameof(request.Detail));
			}

			var approverId = await ResolveApproverIdAsync(request.EmployeeId);

			var hoursEntry = new Hours
			{
				EmployeeId = request.EmployeeId,
				Quantity = request.Quantity,
				Detail = request.Detail.Trim(),
				Date = request.Date == default ? DateTime.UtcNow : request.Date,
				Status = string.IsNullOrWhiteSpace(request.Status) ? DefaultStatus : request.Status,
				ApproverId = approverId
			};

			_logger.LogInformation("Registering hours for employee {EmployeeId}", request.EmployeeId);
			var savedEntry = await _hoursRepository.RegisterHoursAsync(hoursEntry);

			return new RegisterHoursResponseDto
			{
				Message = "Horas registradas correctamente.",
				Entry = MapToDto(savedEntry)
			};
		}

		public async Task<List<HoursEntryDto>> GetRecentEntriesAsync(int employeeId, int limit = 6)
		{
			ValidateEmployeeId(employeeId);
			if (limit <= 0)
			{
				limit = 6;
			}

			_logger.LogInformation("Fetching recent {Limit} hours entries for employee {EmployeeId}", limit, employeeId);
			var entries = await _hoursRepository.GetRecentEntriesAsync(employeeId, limit);
			return entries.Select(MapToDto).ToList();
		}

		public async Task<int> GetWeeklyHoursAsync(int employeeId)
		{
			ValidateEmployeeId(employeeId);
			_logger.LogInformation("Fetching weekly hours for employee {EmployeeId}", employeeId);
			return await _hoursRepository.GetWeeklyHoursAsync(employeeId);
		}

		public async Task<int> GetMonthlyHoursAsync(int employeeId)
		{
			ValidateEmployeeId(employeeId);
			_logger.LogInformation("Fetching monthly hours for employee {EmployeeId}", employeeId);
			return await _hoursRepository.GetMonthlyHoursAsync(employeeId);
		}

		public async Task<int> GetTotalEntriesAsync(int employeeId)
		{
			ValidateEmployeeId(employeeId);
			_logger.LogInformation("Fetching total hour entries for employee {EmployeeId}", employeeId);
			return await _hoursRepository.GetTotalEntriesAsync(employeeId);
		}

		public async Task<HoursEntryDto?> GetLastEntryAsync(int employeeId)
		{
			ValidateEmployeeId(employeeId);
			_logger.LogInformation("Fetching last entry for employee {EmployeeId}", employeeId);
			var entry = await _hoursRepository.GetLastEntryAsync(employeeId);
			return entry is null ? null : MapToDto(entry);
		}

		public async Task<HoursSummaryDto> GetSummaryAsync(int employeeId)
		{
			ValidateEmployeeId(employeeId);
			_logger.LogInformation("Fetching hours summary for employee {EmployeeId}", employeeId);

			var recentEntriesTask = _hoursRepository.GetRecentEntriesAsync(employeeId);
			var weeklyHoursTask = _hoursRepository.GetWeeklyHoursAsync(employeeId);
			var monthlyHoursTask = _hoursRepository.GetMonthlyHoursAsync(employeeId);
			var totalEntriesTask = _hoursRepository.GetTotalEntriesAsync(employeeId);
			var lastEntryTask = _hoursRepository.GetLastEntryAsync(employeeId);

			await Task.WhenAll(recentEntriesTask, weeklyHoursTask, monthlyHoursTask, totalEntriesTask, lastEntryTask);

			var lastEntry = await lastEntryTask;
			var recentEntries = await recentEntriesTask;

			return new HoursSummaryDto
			{
				RecentEntries = recentEntries.Select(MapToDto).ToList(),
				WeeklyHours = await weeklyHoursTask,
				MonthlyHours = await monthlyHoursTask,
				TotalEntries = await totalEntriesTask,
				LastEntry = lastEntry != null ? MapToDto(lastEntry) : null
			};
		}

		private async Task<int> ResolveApproverIdAsync(int employeeId)
		{
			var companyId = await _employeeRepository.GetEmployeeCompanyIdAsync(employeeId);
			if (!companyId.HasValue)
			{
				throw new InvalidOperationException($"Employee {employeeId} does not belong to a company");
			}

			var employerId = await _projectRepository.GetEmployerIdByCompanyIdAsync(companyId.Value);
			if (!employerId.HasValue)
			{
				throw new InvalidOperationException($"Company {companyId.Value} does not have an associated employer");
			}

			return employerId.Value;
		}

		private static void ValidateEmployeeId(int employeeId, string? fieldName = null)
		{
			if (employeeId <= 0)
			{
				throw new ArgumentException("Value must be greater than zero", fieldName ?? nameof(employeeId));
			}
		}

		private static HoursEntryDto MapToDto(Hours entry)
		{
			return new HoursEntryDto
			{
				Id = entry.Id,
				EmployeeId = entry.EmployeeId,
				Quantity = entry.Quantity,
				Detail = entry.Detail,
				Date = entry.Date,
				Status = entry.Status,
				ApproverId = entry.ApproverId
			};
		}
	}
}
