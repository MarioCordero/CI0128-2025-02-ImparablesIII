using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public PayrollService(IPayrollRepository payrollRepository, IEmployeeRepository employeeRepository)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<PayrollSummaryDto> GetPayrollSummaryAsync(PayrollFiltersDto filters)
        {
            var payrollDetails = await _payrollRepository.GetPayrollDetailsAsync(filters);
            
            var employees = payrollDetails.Select(p => new EmployeePayrollDto
            {
                EmployeeId = p.EmployeeId,
                Name = p.EmployeeName,
                Department = p.Department ?? "N/A",
                GrossSalary = FormatColones(p.GrossSalary),
                EmployeeDeductions = FormatColones(p.CcssEmployee + p.IncomeTax + p.OtherDeductions),
                EmployerDeductions = FormatColones(p.CcssEmployer),
                Benefits = FormatColones(p.Benefits),
                NetSalary = FormatColones(p.NetSalary),
                PaymentStatus = p.Status,
                StatusColor = GetStatusColor(p.Status),
                DeductionTooltips = GetDeductionTooltips(p)
            }).ToList();

            var totals = new PayrollTotalsDto
            {
                TotalCompanyGrossSalary = FormatColones(payrollDetails.Sum(p => p.GrossSalary + p.CcssEmployer)),
                TotalEmployerDeductions = FormatColones(payrollDetails.Sum(p => p.CcssEmployer)),
                TotalEmployeeDeductions = FormatColones(payrollDetails.Sum(p => p.CcssEmployee + p.IncomeTax + p.OtherDeductions)),
                TotalBenefits = FormatColones(payrollDetails.Sum(p => p.Benefits)),
                TotalNetSalary = FormatColones(payrollDetails.Sum(p => p.NetSalary))
            };

            var periodInfo = new PeriodInfoDto
            {
                Period = filters.Period.ToString("yyyy-MM"),
                PeriodType = filters.PeriodType,
                StartDate = GetPeriodStartDate(filters.Period, filters.PeriodType),
                EndDate = GetPeriodEndDate(filters.Period, filters.PeriodType)
            };

            return new PayrollSummaryDto
            {
                Employees = employees,
                Totals = totals,
                PeriodInfo = periodInfo
            };
        }

        public async Task<PayrollDetailDto> CalculateEmployeePayrollAsync(PayrollCalculationRequestDto request)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeId);
            if (employee == null)
                throw new ArgumentException("Employee not found");

            var grossSalary = employee.Salario;
            var ccssEmployee = CalculateCcssEmployee(grossSalary);
            var ccssEmployer = CalculateCcssEmployer(grossSalary);
            var incomeTax = CalculateIncomeTax(grossSalary);
            var benefits = await CalculateBenefits(request.EmployeeId, request.Period);
            var netSalary = grossSalary - ccssEmployee - incomeTax + benefits;

            return new PayrollDetailDto
            {
                EmployeeId = request.EmployeeId,
                EmployeeName = $"{employee.Persona?.Nombre} {employee.Persona?.Apellidos}",
                GrossSalary = grossSalary,
                CcssEmployee = ccssEmployee,
                CcssEmployer = ccssEmployer,
                IncomeTax = incomeTax,
                OtherDeductions = 0,
                Benefits = benefits,
                NetSalary = netSalary,
                Period = request.Period,
                Status = "Calculated"
            };
        }

        // public async Task<bool> ProcessPayrollAsync(PayrollFiltersDto filters)
        // {
        //     var employees = await _employeeRepository.GetAllEmployeesAsync();
            
        //     foreach (var employee in employees)
        //     {
        //         if (filters.DepartmentId.HasValue && employee.DepartamentoId != filters.DepartmentId.Value)
        //             continue;

        //         // Check if payroll already processed
        //         if (await _payrollRepository.IsPayrollProcessedAsync(employee.EmpleadoId, filters.Period, filters.PeriodType))
        //             continue;

        //         var calculationRequest = new PayrollCalculationRequestDto
        //         {
        //             EmployeeId = employee.EmpleadoId,
        //             Period = filters.Period,
        //             PeriodType = filters.PeriodType
        //         };

        //         var payrollDetail = await CalculateEmployeePayrollAsync(calculationRequest);

        //         var payroll = new Payroll
        //         {
        //             EmployeeId = employee.EmpleadoId,
        //             Period = filters.Period,
        //             PeriodType = filters.PeriodType,
        //             GrossSalary = payrollDetail.GrossSalary,
        //             CcssEmployee = payrollDetail.CcssEmployee,
        //             CcssEmployer = payrollDetail.CcssEmployer,
        //             IncomeTax = payrollDetail.IncomeTax,
        //             OtherDeductions = payrollDetail.OtherDeductions,
        //             Benefits = payrollDetail.Benefits,
        //             NetSalary = payrollDetail.NetSalary,
        //             Status = "Paid"
        //         };

        //         await _payrollRepository.CreatePayrollAsync(payroll);
        //     }

        //     return true;
        // }

        public async Task<IEnumerable<PayrollDetailDto>> GetPayrollHistoryAsync(int employeeId)
        {
            var filters = new PayrollFiltersDto
            {
                Period = DateTime.Now,
                PeriodType = "Monthly"
            };

            var allPayrolls = await _payrollRepository.GetPayrollDetailsAsync(filters);
            return allPayrolls.Where(p => p.EmployeeId == employeeId);
        }

        // Helper methods
        private decimal CalculateCcssEmployee(decimal grossSalary)
        {
            return grossSalary * 0.0917m; // 9.17%
        }

        private decimal CalculateCcssEmployer(decimal grossSalary)
        {
            return grossSalary * 0.0934m; // 9.34%
        }

        private decimal CalculateIncomeTax(decimal grossSalary)
        {
            // Simplified tax calculation - should implement proper tax brackets
            if (grossSalary <= 941000) return 0;
            return grossSalary * 0.10m; // 10% for demonstration
        }

        private async Task<decimal> CalculateBenefits(int employeeId, DateTime period)
        {
            // Implement benefit calculation logic
            // This could include aguinaldo, vacations, bonuses, etc.
            return 0; // Placeholder
        }

        private string FormatColones(decimal amount)
        {
            return $"₡{amount:N0}";
        }

        private string GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "paid" => "success",
                "pending" => "warning",
                "cancelled" => "danger",
                _ => "secondary"
            };
        }

        private List<DeductionTooltipDto> GetDeductionTooltips(PayrollDetailDto payroll)
        {
            return new List<DeductionTooltipDto>
            {
                new() { Concept = "CCSS", Description = "Aporte seguro social (9.17%)", Amount = FormatColones(payroll.CcssEmployee) },
                new() { Concept = "Renta", Description = "Impuesto sobre la renta", Amount = FormatColones(payroll.IncomeTax) },
                new() { Concept = "Otros", Description = "Otras deducciones", Amount = FormatColones(payroll.OtherDeductions) }
            };
        }

        private DateTime GetPeriodStartDate(DateTime period, string periodType)
        {
            return periodType.ToLower() switch
            {
                "biweekly" => period.AddDays(-14),
                _ => new DateTime(period.Year, period.Month, 1) // Monthly
            };
        }

        private DateTime GetPeriodEndDate(DateTime period, string periodType)
        {
            return periodType.ToLower() switch
            {
                "biweekly" => period,
                _ => new DateTime(period.Year, period.Month, DateTime.DaysInMonth(period.Year, period.Month)) // Monthly
            };
        }
    }
}