using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _repo;
        public PayrollService(IPayrollRepository repo)
        {
            _repo = repo;
        }

        public async Task<PayrollSummaryDto> GetReportAsync(PayrollFiltersDto f)
        {
            var (start, end, label, year, month, fortnight) = ResolvePeriod(f.Period, f.PeriodType, f.Fortnight);

            var (items, totals) = await _repo.ExecutePayrollReportAsync(
                f.CompanyId, year, month, f.PeriodType.ToString(), fortnight, f.Department);

            return new PayrollSummaryDto
            {
                Employees = items.OrderBy(i => i.FullName).ToList(),
                Totals = totals,
                PeriodInfo = new PeriodInfoDto
                {
                    PeriodType = f.PeriodType,
                    Label = label,
                    StartDate = start,
                    EndDate = end,
                    Year = year,
                    Month = month,
                    Fortnight = fortnight
                },
                Tooltips = new Dictionary<string, string>
                {
                    ["GROSS"] = "Gross salary before deductions and benefits.",
                    ["EE"]    = "Employee-side deductions (renta, CCSS, solidarista EE).",
                    ["ER"]    = "Employer-side contributions (CCSS, solidarista ER).",
                    ["BEN"]   = "Voluntary benefits selected by the employee.",
                    ["NET"]   = "Net = Gross - EE + Benefits."
                }
            };
        }


        private static (DateTime, DateTime, string, int, int, int?) ResolvePeriod(DateTime period, PeriodType type, int? fortnight)
        {
            int year = period.Year, month = period.Month;
            if (type == PeriodType.Monthly)
            {
                var from = new DateTime(year, month, 1);
                var to = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                return (from, to, $"Monthly {year}-{month:00}", year, month, null);
            }
            int q = (fortnight is 1 or 2) ? fortnight.Value : (period.Day <= 15 ? 1 : 2);
            var fromQ = q == 1 ? new DateTime(year, month, 1) : new DateTime(year, month, 16);
            var toQ = q == 1 ? new DateTime(year, month, 15) : new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return (fromQ, toQ, $"Biweekly {q} · {year}-{month:00}", year, month, q);
        }
    }
}
