using backend.DTOs;
using backend.Repositories;
using backend.Constants;
using backend.Services.PaymentsCalculate;

namespace backend.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _repo;
        private readonly IBenefitDeductionsService _benefitDeductionsService;
        private readonly IEmployeeBenefitRepository _employeeBenefitRepository;
        private readonly EmployeeDeductionCalculatorFactory _employeeCalculatorFactory;
        private readonly EmployerDeductionCalculatorFactory _employerCalculatorFactory;
        private readonly IBenefitCodeParser _benefitCodeParser;
        
        public PayrollService(
            IPayrollRepository repo, 
            IBenefitDeductionsService benefitDeductionsService, 
            IEmployeeBenefitRepository employeeBenefitRepository,
            EmployeeDeductionCalculatorFactory employeeCalculatorFactory,
            EmployerDeductionCalculatorFactory employerCalculatorFactory,
            IBenefitCodeParser benefitCodeParser)
        {
            _repo = repo;
            _benefitDeductionsService = benefitDeductionsService;
            _employeeBenefitRepository = employeeBenefitRepository;
            _employeeCalculatorFactory = employeeCalculatorFactory;
            _employerCalculatorFactory = employerCalculatorFactory;
            _benefitCodeParser = benefitCodeParser;
        }

        public async Task<List<EmployeePayrollDto>> GetEmployeePayrollWithDeductionsAsync(int companyId)
        {
            var employees = await _repo.GetEmployeesForPayrollAsync(companyId);
            var employeeDeductions = await _repo.GetEmployeeDeductionsAsync();

            foreach (var employee in employees)
            {
                var deductionResult = CalculateEmployeeDeductions(employee, employeeDeductions);
                employee.TotalEmployeeDeductions = Math.Round(deductionResult.TotalDeductions, 2);
                employee.NetSalary = Math.Round(employee.SalarioBruto - deductionResult.TotalDeductions, 2);
                employee.DeductionLines = deductionResult.DeductionLines;
            }

            return employees;
        }

        public async Task<List<EmployerDeductionResultDto>> GetEmployerPayrollWithDeductionsAsync(int companyId)
        {
            var employees = await _repo.GetEmployeesForPayrollAsync(companyId);
            var employerDeductions = await _repo.GetEmployerDeductionsAsync();

            var results = new List<EmployerDeductionResultDto>();

            foreach (var employee in employees)
            {
                var deductionResult = CalculateEmployerDeductions(employee, employerDeductions);
                results.Add(CreateEmployerDeductionResult(employee, deductionResult));
            }

            return results;
        }

        public async Task<int> GeneratePayrollWithBenefitsAsync(int companyId, int responsibleEmployeeId, int hours, string? periodType = null, int? fortnight = null)
        {
            await ValidatePayrollPeriod(companyId, periodType, fortnight);
            
            var employees = await GetEmployeePayrollWithDeductionsAsync(companyId);
            var employerDeductions = await GetEmployerPayrollWithDeductionsAsync(companyId);
            var employeePositions = await _repo.GetEmployeePositionsByCompanyAsync(companyId);
            
            var payrollBuildResult = await BuildPayrollDetailsAsync(employees, employerDeductions, employeePositions, companyId);
            
            var payrollId = await CreatePayrollRecordAsync(companyId, responsibleEmployeeId, hours);
            await _repo.InsertPayrollDetailsAsync(payrollId, payrollBuildResult.PayrollDetails);
            if (payrollBuildResult.BenefitNames.Any())
            {
                await _repo.InsertPayrollBenefitsAsync(payrollId, companyId, payrollBuildResult.BenefitNames);
            }
            
            return payrollId;
        }

        public async Task<PayrollTotalsDto?> GetLatestPayrollTotalsByCompanyAsync(int companyId)
        {
            if (companyId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(companyId));
            }
            
            return await _repo.GetLatestPayrollTotalsByCompanyAsync(companyId);
        }

        public async Task<List<PayrollHistoryItemDto>> GetPayrollHistoryByCompanyAsync(int companyId)
        {
            if (companyId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(companyId));
            }
            
            return await _repo.GetPayrollHistoryByCompanyAsync(companyId);
        }

        public async Task<DetailedPayrollReportDto?> GetDetailedPayrollReportAsync(int employeeId, int payrollId, int authenticatedEmployeeId)
        {
            ValidateEmployeeAccess(employeeId, authenticatedEmployeeId);

            var report = await _repo.GetDetailedPayrollReportAsync(employeeId, payrollId);
            if (report == null)
            {
                return null;
            }

            await EnrichReportWithEmployeeInfoAsync(report, employeeId);
            await EnrichReportWithMandatoryDeductionsAsync(report);
            await EnrichReportWithVoluntaryDeductionsAsync(report, employeeId, payrollId);

            return report;
        }

        public async Task<List<EmployeePayrollReportDto>> GetEmployeePayrollReportsAsync(int employeeId, int authenticatedEmployeeId, int? year = null, int? month = null, string? puesto = null)
        {
            ValidateEmployeeAccess(employeeId, authenticatedEmployeeId);
            return await _repo.GetEmployeePayrollReportsAsync(employeeId, year, month, puesto);
        }

        public async Task<HistoricalPayrollReportDto> GetHistoricalPayrollReportAsync(int employeeId, int authenticatedEmployeeId, DateTime? startDate, DateTime? endDate)
        {
            ValidateEmployeeAccess(employeeId, authenticatedEmployeeId);
            var report = await _repo.GetHistoricalPayrollReportAsync(employeeId, startDate, endDate);
            
            if (report == null)
            {
                return null;
            }

            return report;
        }

        private EmployeeDeductionResult CalculateEmployeeDeductions(EmployeePayrollDto employee, List<EmployeeDeductionDto> deductions)
        {
            var deductionLines = new List<EmployeeDeductionLineDto>();
            decimal totalDeductions = 0m;

            foreach (var deduction in deductions)
            {
                var calculator = _employeeCalculatorFactory.CreateCalculator(deduction);
                if (calculator == null)
                {
                    continue;
                }

                var line = calculator.Calculate(employee.SalarioBruto);
                deductionLines.Add(line);
                totalDeductions += line.Amount;
            }

            return new EmployeeDeductionResult
            {
                DeductionLines = deductionLines,
                TotalDeductions = totalDeductions
            };
        }

        private EmployerDeductionResult CalculateEmployerDeductions(EmployeePayrollDto employee, List<EmployerDeductionDto> deductions)
        {
            var deductionLines = new List<EmployerDeductionLineDto>();
            decimal totalDeductions = 0m;

            foreach (var deduction in deductions)
            {
                var calculator = _employerCalculatorFactory.CreateCalculator(deduction);
                if (calculator == null)
                {
                    continue;
                }

                var line = calculator.Calculate(employee.SalarioBruto);
                deductionLines.Add(line);
                totalDeductions += line.Amount;
            }

            return new EmployerDeductionResult
            {
                DeductionLines = deductionLines,
                TotalDeductions = totalDeductions
            };
        }

        private EmployerDeductionResultDto CreateEmployerDeductionResult(EmployeePayrollDto employee, EmployerDeductionResult deductionResult)
        {
            return new EmployerDeductionResultDto
            {
                IdEmpleado = employee.IdEmpleado,
                IdEmpresa = employee.IdEmpresa,
                Nombre = employee.Nombre,
                Apellidos = employee.Apellidos,
                SalarioBruto = employee.SalarioBruto,
                TotalEmployerDeductions = Math.Round(deductionResult.TotalDeductions, 2),
                DeductionLines = deductionResult.DeductionLines
            };
        }

        private async Task ValidatePayrollPeriod(int companyId, string? periodType, int? fortnight)
        {
            var now = DateTime.Now;
            var isMonthly = string.Equals(periodType, "Mensual", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(periodType);
            
            if (isMonthly)
            {
                var exists = await _repo.ExistsPayrollForMonthAsync(companyId, now.Year, now.Month);
                if (exists)
                {
                    throw new InvalidOperationException("La planilla de este mes ya fue generada para esta empresa.");
                }
            }
            else
            {
                var currentFortnight = fortnight ?? (now.Day <= 15 ? 1 : 2);
                var exists = await _repo.ExistsPayrollForFortnightAsync(companyId, now.Year, now.Month, currentFortnight);
                if (exists)
                {
                    throw new InvalidOperationException("La planilla de esta quincena ya fue generada para esta empresa.");
                }
            }
        }

        private async Task<PayrollBuildResult> BuildPayrollDetailsAsync(
            List<EmployeePayrollDto> employees,
            List<EmployerDeductionResultDto> employerDeductions,
            Dictionary<int, string> employeePositions,
            int companyId)
        {
            var payrollDetails = new List<PayrollDetailInsertDto>();
            var benefitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var employee in employees)
            {
                var employerResult = employerDeductions.FirstOrDefault(e => e.IdEmpleado == employee.IdEmpleado);
                if (employerResult == null)
                {
                    continue;
                }

                var benefitCalculation = await _benefitDeductionsService.CalculateBenefitDeductionsAsync(employee.IdEmpleado, companyId);
                var selectedBenefits = await _employeeBenefitRepository.GetSelectedBenefitsForEmployeeAsync(employee.IdEmpleado, companyId);
                var benefitNameMap = BuildBenefitNameMap(selectedBenefits);
                foreach (var benefitName in ExtractBenefitNames(benefitCalculation, benefitNameMap))
                {
                    benefitNames.Add(benefitName);
                }
                var benefitTotals = CalculateBenefitTotals(benefitCalculation);
                
                var totalEmployeeDeductions = employee.TotalEmployeeDeductions + benefitTotals.EmployeeDeductions;
                var totalEmployerDeductions = employerResult.TotalEmployerDeductions + benefitTotals.EmployerDeductions;
                var netSalary = employee.SalarioBruto - totalEmployeeDeductions;
                var position = employeePositions.GetValueOrDefault(employee.IdEmpleado, "Sin Puesto");

                payrollDetails.Add(CreatePayrollDetail(employee, totalEmployeeDeductions, totalEmployerDeductions, benefitTotals.TotalBenefits, netSalary, position));
            }

            return new PayrollBuildResult
            {
                PayrollDetails = payrollDetails,
                BenefitNames = benefitNames.ToList()
            };
        }

        private BenefitTotals CalculateBenefitTotals(BenefitDeductionCalculationDto benefitCalculation)
        {
            var totalBenefits = (decimal)benefitCalculation.Deductions.Sum(d => d.Amount);
            var employeeBenefitDeductions = (decimal)benefitCalculation.Deductions
                .Where(d => d.Role == DeductionRoleNames.EmployeeDeduction)
                .Sum(d => d.Amount);
            var employerBenefitDeductions = (decimal)benefitCalculation.Deductions
                .Where(d => d.Role == DeductionRoleNames.EmployerDeduction)
                .Sum(d => d.Amount);

            return new BenefitTotals
            {
                TotalBenefits = totalBenefits,
                EmployeeDeductions = employeeBenefitDeductions,
                EmployerDeductions = employerBenefitDeductions
            };
        }

        private PayrollDetailInsertDto CreatePayrollDetail(
            EmployeePayrollDto employee,
            decimal totalEmployeeDeductions,
            decimal totalEmployerDeductions,
            decimal totalBenefits,
            decimal netSalary,
            string position)
        {
            return new PayrollDetailInsertDto
            {
                IdEmpleado = employee.IdEmpleado,
                SalarioBruto = (int)employee.SalarioBruto,
                DeduccionesEmpleado = (int)Math.Round(totalEmployeeDeductions, 0),
                DeduccionesEmpresa = (int)Math.Round(totalEmployerDeductions, 0),
                TotalBeneficios = (int)Math.Round(totalBenefits, 0),
                SalarioNeto = (int)Math.Round(netSalary, 0),
                Puesto = position
            };
        }

        private async Task<int> CreatePayrollRecordAsync(int companyId, int responsibleEmployeeId, int hours)
        {
            var payrollId = await _repo.InsertPayrollAsync(new PayrollInsertDto
            {
                FechaGeneracion = DateTime.Now,
                Horas = hours,
                IdResponsable = responsibleEmployeeId,
                IdEmpresa = companyId
            });

            return payrollId;
        }

        private void ValidateEmployeeAccess(int employeeId, int authenticatedEmployeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(employeeId), "El ID del empleado debe ser mayor a 0");
            }

            if (employeeId != authenticatedEmployeeId)
            {
                throw new UnauthorizedAccessException("No tiene permiso para acceder a los reportes de otro empleado");
            }
        }

        private async Task EnrichReportWithEmployeeInfoAsync(DetailedPayrollReportDto report, int employeeId)
        {
            var employeeInfo = await _repo.GetEmployeeBasicInfoAsync(employeeId);
            report.NombreCompletoEmpleado = employeeInfo.NombreCompleto;
            report.TipoContrato = employeeInfo.TipoContrato;
        }

        private async Task EnrichReportWithMandatoryDeductionsAsync(DetailedPayrollReportDto report)
        {
            var employeeDeductions = await _repo.GetEmployeeDeductionsAsync();
            var mandatoryDeductions = new List<MandatoryDeductionDto>();
            decimal totalMandatory = 0m;

            foreach (var deduction in employeeDeductions)
            {
                var calculator = _employeeCalculatorFactory.CreateCalculator(deduction);
                if (calculator == null)
                {
                    continue;
                }

                var line = calculator.Calculate(report.SalarioBruto);
                if (line.Amount > 0)
                {
                    var displayName = _employeeCalculatorFactory.GetDisplayName(deduction.Code);
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = deduction.Name;
                    }

                    mandatoryDeductions.Add(new MandatoryDeductionDto
                    {
                        Nombre = displayName,
                        Monto = line.Amount
                    });
                    totalMandatory += line.Amount;
                }
            }

            report.DeduccionesObligatorias = mandatoryDeductions;
            report.TotalDeduccionesObligatorias = Math.Round(totalMandatory, 2);
        }

        private async Task EnrichReportWithVoluntaryDeductionsAsync(DetailedPayrollReportDto report, int employeeId, int payrollId)
        {
            var companyId = await _repo.GetCompanyIdFromPayrollAsync(payrollId);
            if (!companyId.HasValue)
            {
                report.DeduccionesVoluntarias = new List<VoluntaryDeductionDto>();
                report.TotalDeduccionesVoluntarias = 0m;
                return;
            }

            var benefitCalculation = await _benefitDeductionsService.CalculateBenefitDeductionsAsync(employeeId, companyId.Value);
            var selectedBenefits = await _employeeBenefitRepository.GetSelectedBenefitsForEmployeeAsync(employeeId, companyId.Value);
            var benefitNameMap = BuildBenefitNameMap(selectedBenefits);

            var voluntaryDeductions = benefitCalculation.Deductions
                .Where(d => d.Role == DeductionRoleNames.EmployeeDeduction)
                .Select(d => new VoluntaryDeductionDto
                {
                    Nombre = GetBenefitDisplayName(d.Code, benefitNameMap),
                    Monto = d.Amount
                })
                .ToList();

            report.DeduccionesVoluntarias = voluntaryDeductions;
            report.TotalDeduccionesVoluntarias = Math.Round(voluntaryDeductions.Sum(d => d.Monto), 2);
        }

        private Dictionary<string, string> BuildBenefitNameMap(List<EmployeeBenefitDto> selectedBenefits)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var benefit in selectedBenefits)
            {
                var employeeCode = _benefitCodeParser.GenerateBenefitCode(benefit.BenefitName, BenefitConstants.DeductionTypeEmployee);
                var employerCode = _benefitCodeParser.GenerateBenefitCode(benefit.BenefitName, BenefitConstants.DeductionTypeEmployer);

                map[employeeCode] = benefit.BenefitName;
                map[employerCode] = benefit.BenefitName;
            }

            return map;
        }

        private string GetBenefitDisplayName(string code, Dictionary<string, string> benefitNameMap)
        {
            if (benefitNameMap.TryGetValue(code, out var benefitName))
            {
                return benefitName;
            }

            return _benefitCodeParser.ParseBenefitNameFromCode(code);
        }

        private IEnumerable<string> ExtractBenefitNames(BenefitDeductionCalculationDto benefitCalculation, Dictionary<string, string> benefitNameMap)
        {
            if (benefitCalculation?.Deductions == null)
            {
                yield break;
            }

            foreach (var deduction in benefitCalculation.Deductions)
            {
                var displayName = GetBenefitDisplayName(deduction.Code, benefitNameMap);
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    yield return displayName.Trim();
                }
            }
        }

        private class EmployeeDeductionResult
        {
            public List<EmployeeDeductionLineDto> DeductionLines { get; set; } = new();
            public decimal TotalDeductions { get; set; }
        }

        private class EmployerDeductionResult
        {
            public List<EmployerDeductionLineDto> DeductionLines { get; set; } = new();
            public decimal TotalDeductions { get; set; }
        }

        private class BenefitTotals
        {
            public decimal TotalBenefits { get; set; }
            public decimal EmployeeDeductions { get; set; }
            public decimal EmployerDeductions { get; set; }
        }

        private class PayrollBuildResult
        {
            public List<PayrollDetailInsertDto> PayrollDetails { get; set; } = new();
            public List<string> BenefitNames { get; set; } = new();
        }
    }
}
