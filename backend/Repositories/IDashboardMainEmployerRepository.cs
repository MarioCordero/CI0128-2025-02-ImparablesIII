using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<Project>> GetByEmployerIdAsync(int employerId);
        Task<decimal> GetProfitabilityAsync(int companyId, DateTime date);
    }

    public interface INotificationRepository
    {
        Task<List<NotificationDto>> GetByCompanyAsync(int companyId);
    }

}