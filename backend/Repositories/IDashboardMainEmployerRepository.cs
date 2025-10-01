using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<Project>> GetByEmployerIdAsync(int employerId);
        Task<decimal> GetProfitabilityAsync(int companyId, DateTime date);
    }

    public interface IEmployeeRepository
    {
        Task<int> CountActiveByCompanyAsync(int companyId);
    }

    public interface INotificationRepository
    {
        Task<List<NotificationDto>> GetByCompanyAsync(int companyId);
    }

    public interface IPayrollRepository
    {
        Task<decimal> GetMonthlyTotalAsync(int companyId);
    }
}