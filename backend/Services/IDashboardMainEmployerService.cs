using backend.DTOs;
public interface IDashboardMainEmployerService
{
    Task<DashboardMainEmployerDto> GetDashboardAsync(int employerId);
}

public class DashboardMainEmployerService : IDashboardMainEmployerService
{
    private readonly ICompanyRepository _companyRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IPayrollRepository _payrollRepo;

    public DashboardMainEmployerService(
        ICompanyRepository companyRepo,
        IEmployeeRepository employeeRepo,
        INotificationRepository notificationRepo,
        IPayrollRepository payrollRepo)
    {
        _companyRepo = companyRepo;
        _employeeRepo = employeeRepo;
        _notificationRepo = notificationRepo;
        _payrollRepo = payrollRepo;
    }

    public async Task<DashboardMainEmployerDto> GetDashboardAsync(int employerId)
    {
        var companies = await _companyRepo.GetByEmployerIdAsync(employerId);

        var dto = new DashboardMainEmployerDto
        {
            EmployerName = "John Doe", // Load from Employer table
            Companies = new List<CompanyDashboardMainEmployerDto>()
        };

        foreach (var company in companies)
        {
            dto.Companies.Add(new CompanyDashboardMainEmployerDto
            {
                Id = company.Id,
                Name = company.Name,
                LegalId = company.LegalId,
                ActiveEmployees = await _employeeRepo.CountActiveByCompanyAsync(company.Id),
                PayPeriod = company.PayPeriod,
                MonthlyPayroll = await _payrollRepo.GetMonthlyTotalAsync(company.Id),
                CurrentProfitability = await _companyRepo.GetProfitabilityAsync(company.Id, DateTime.Now),
                LastMonthProfitability = await _companyRepo.GetProfitabilityAsync(company.Id, DateTime.Now.AddMonths(-1)),
                Notifications = await _notificationRepo.GetByCompanyAsync(company.Id)
            });
        }

        return dto;
    }
}
