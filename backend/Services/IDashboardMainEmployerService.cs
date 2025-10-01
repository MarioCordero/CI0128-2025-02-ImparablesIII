using backend.DTOs;

namespace backend.Services
{
    public interface IDashboardMainEmployerService
    {
        Task<DashboardMainEmployerDto> GetDashboardDataAsync(int employerId);
    }
}