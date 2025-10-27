using backend.DTOs;
using backend.Models;
using backend.Repositories;

// Calculators
using backend.Services.PaymentsCalculate;                // PaymentsTypes.cs (CalcRole, CalcLine)
using backend.Services.PaymentsCalculate.Employee;       // CcssSemEmployeeCalc, etc.
using backend.Services.PaymentsCalculate.Employer;       // CcssSemEmployerCalc, etc.

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
                    ["EE"] = "Employee-side deductions (renta, CCSS, solidarista EE).",
                    ["ER"] = "Employer-side contributions (CCSS, solidarista ER).",
                    ["BEN"] = "Voluntary benefits selected by the employee.",
                    ["NET"] = "Net = Gross - EE + Benefits."
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

        // ----------------------------------------- test methods -----------------------------------------
        public Task<TestEmployeeDeductionsResponse> TestEmployeeDeductionsAsync(TestEmployeeDeductionsRequest request)
        {
            // just for one employee, no DB involved
            if (request.GrossSalary <= 0)
                throw new ArgumentException("GrossSalary must be > 0.");

            // 1) Build calculators on the fly (no DI, no orchestrator)
            var lines = new List<CalcLine>();

            // Employee (EE)
            lines.Add(new CcssSemEmployeeCalc().Calculate(request.GrossSalary));    // 5.5%
            lines.Add(new CcssIvmEmployeeCalc().Calculate(request.GrossSalary));    // 4.17%
            lines.Add(new BancoPopularEmployeeCalc().Calculate(request.GrossSalary)); // 1%
            lines.Add(new SalaryTaxEmployeeCalc().Calculate(request.GrossSalary));  // Salary tax

            // Add deductions?  

            // 2) Totals
            var totalEE = lines.Where(x => x.Role == CalcRole.EmployeeDeduction).Sum(x => x.Amount);

            // 3) Map to DTO response (keep API contract stable)
            var resp = new TestEmployeeDeductionsResponse
            {
                GrossSalary = request.GrossSalary,
                TotalEmployeeDeductions = totalEE,
                NetSalary = request.GrossSalary - totalEE,
                Lines = lines.Select(l => new TestCalcLineDto
                {
                    Code = l.Code,
                    Amount = l.Amount,
                    Role = l.Role.ToString()
                }).ToList()
            };

            return Task.FromResult(resp);
        }
    }
}
