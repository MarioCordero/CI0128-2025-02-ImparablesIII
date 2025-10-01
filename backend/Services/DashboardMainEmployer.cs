using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    public class DashboardMainEmployerService : IDashboardMainEmployerService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployerRepository _employerRepository;

        public DashboardMainEmployerService(
            IProjectRepository projectRepository,
            IEmployerRepository employerRepository)
        {
            _projectRepository = projectRepository;
            _employerRepository = employerRepository;
        }

        public async Task<DashboardMainEmployerDto> GetDashboardDataAsync(int employerId)
        {
            var projects = await _projectRepository.GetProjectsForDashboardAsync(employerId);

            var dto = new DashboardMainEmployerDto
            {
                Companies = projects?.Select(p => new CompanyDashboardMainEmployerDto
                {
                    Id = p.Id,
                    Name = p.Name ?? string.Empty,
                    LegalId = p.LegalId ?? string.Empty,
                    ActiveEmployees = p.ActiveEmployees,
                    PayPeriod = p.PayPeriod ?? string.Empty,
                    MonthlyPayroll = p.MonthlyPayroll,
                    CurrentProfitability = p.CurrentProfitability,
                    LastMonthProfitability = p.LastMonthProfitability,
                    Notifications = new List<NotificationDto>() // Por ahora vacío
                }).ToList() ?? new List<CompanyDashboardMainEmployerDto>()
            };

            return dto;
        }
    }
}