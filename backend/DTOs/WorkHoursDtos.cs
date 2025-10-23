using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    /// <summary>
    /// DTO for registering work hours
    /// </summary>
    public class RegisterWorkHoursDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one session is required")]
        public List<WorkSessionDto> Sessions { get; set; } = new();
    }

    /// <summary>
    /// DTO for work session (start and end time)
    /// </summary>
    public class WorkSessionDto
    {
        [Required]
        public TimeSpan Start { get; set; }

        [Required]
        public TimeSpan End { get; set; }
    }

    /// <summary>
    /// DTO for work hours registration response
    /// </summary>
    public class WorkHoursResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public decimal TotalHours { get; set; }
    }

    /// <summary>
    /// DTO for work hours summary
    /// </summary>
    public class WorkHoursSummaryDto
    {
        public decimal WeekHours { get; set; }
        public decimal MonthHours { get; set; }
        public int TotalRecords { get; set; }
        public decimal MaxWeeklyHours { get; set; } = 40;
    }

    /// <summary>
    /// DTO for recent work hours response
    /// </summary>
    public class RecentWorkHoursDto
    {
        public List<WorkHoursRecordDto> RecentRecords { get; set; } = new();
    }

    /// <summary>
    /// DTO for individual work hours record
    /// </summary>
    public class WorkHoursRecordDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Weekday { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public List<WorkSessionDto> Sessions { get; set; } = new();
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for work hours statistics
    /// </summary>
    public class WorkHoursStatsDto
    {
        public decimal TotalHoursThisWeek { get; set; }
        public decimal TotalHoursThisMonth { get; set; }
        public decimal TotalHoursThisYear { get; set; }
        public decimal AverageHoursPerDay { get; set; }
        public int WorkingDaysThisMonth { get; set; }
        public decimal OvertimeHours { get; set; }
    }
}