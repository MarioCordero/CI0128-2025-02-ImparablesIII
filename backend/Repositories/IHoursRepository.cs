using backend.Models;

namespace backend.Repositories
{
    public interface IHoursRepository
    {
        Task<Hours> RegisterHoursAsync(Hours entry);
        Task<List<Hours>> GetRecentEntriesAsync(int employeeId, int limit = 6);
        Task<int> GetWeeklyHoursAsync(int employeeId);
        Task<int> GetMonthlyHoursAsync(int employeeId);
        Task<int> GetTotalEntriesAsync(int employeeId);
        Task<Hours?> GetLastEntryAsync(int employeeId);
    }
}