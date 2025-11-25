using backend.DTOs;

namespace backend.Services
{
	public interface IHoursService
	{
		Task<RegisterHoursResponseDto> RegisterHoursAsync(RegisterHoursRequestDto request);
		Task<List<HoursEntryDto>> GetRecentEntriesAsync(int employeeId, int limit = 6);
		Task<int> GetWeeklyHoursAsync(int employeeId);
		Task<int> GetMonthlyHoursAsync(int employeeId);
		Task<int> GetTotalEntriesAsync(int employeeId);
		Task<HoursEntryDto?> GetLastEntryAsync(int employeeId);
		Task<HoursSummaryDto> GetSummaryAsync(int employeeId);
	}
}
