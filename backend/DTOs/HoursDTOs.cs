namespace backend.DTOs
{
	public class RegisterHoursRequestDto
	{
		public int EmployeeId { get; set; }
		public int Quantity { get; set; }
		public string Detail { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public int? ApproverId { get; set; }
		public string Status { get; set; } = "Pendiente";
	}

	public class HoursEntryDto
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public int Quantity { get; set; }
		public string Detail { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public string Status { get; set; } = string.Empty;
		public int ApproverId { get; set; }
	}

	public class RegisterHoursResponseDto
	{
		public string Message { get; set; } = string.Empty;
		public HoursEntryDto Entry { get; set; } = new();
	}

	public class HoursSummaryDto
	{
		public List<HoursEntryDto> RecentEntries { get; set; } = new();
		public int WeeklyHours { get; set; }
		public int MonthlyHours { get; set; }
		public int TotalEntries { get; set; }
		public HoursEntryDto? LastEntry { get; set; }
	}
}
