using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Constants;
using backend.Services.PaymentsCalculate;
using backend.Services.PaymentsCalculate.Employee;
using backend.Services.PaymentsCalculate.Employer;

namespace backend.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _repo;
        private readonly IBenefitDeductionsService _benefitDeductionsService;
        
        public PayrollService(IPayrollRepository repo, IBenefitDeductionsService benefitDeductionsService)
        {
            _repo = repo;
            _benefitDeductionsService = benefitDeductionsService;
        }
        private async Task<List<EmployeeDeductionDto>> GetEmployeeDeductionsAsync()
        {
            return await _repo.GetEmployeeDeductionsAsync();
        }

        private async Task<List<EmployerDeductionDto>> GetEmployerDeductionsAsync()
        {
            return await _repo.GetEmployerDeductionsAsync();
        }
        public async Task<List<EmployeePayrollDto>> GetEmployeePayrollWithDeductionsAsync(int companyId)
        {
            var empleados = await _repo.GetEmployeesForPayrollAsync(companyId);
            var employeeDeductions = await _repo.GetEmployeeDeductionsAsync();

            foreach (var emp in empleados)
            {
                decimal totalDeductions = 0m;
                var deductionLines = new List<EmployeeDeductionLineDto>();

                foreach (var ded in employeeDeductions)
                {
                    EmployeeDeductionLineDto line = null;
                    switch (ded.Code)
                    {
                        case "CCSS_SEM_EE":
                            line = new CcssSemEmployeeCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "CCSS_IVM_EE":
                            line = new CcssIvmEmployeeCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "BP_TRAB":
                            line = new BancoPopularEmployeeCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "RENTA":
                            line = new SalaryTaxEmployeeCalc(ded.Rate, ded.MinAmount, ded.MaxAmount).Calculate(emp.SalarioBruto);
                            break;
                    }
                    if (line != null)
                    {
                        deductionLines.Add(line);
                        totalDeductions += line.Amount;
                    }
                }

                emp.TotalEmployeeDeductions = Math.Round(totalDeductions, 2);
                emp.NetSalary = Math.Round(emp.SalarioBruto - totalDeductions, 2);
                emp.DeductionLines = deductionLines;
            }

            return empleados;
        }
        // ----------------------------------------- EMPLOYER METHODS -----------------------------------------
        public async Task<List<EmployerDeductionResultDto>> GetEmployerPayrollWithDeductionsAsync(int companyId)
        {
            var empleados = await _repo.GetEmployeesForPayrollAsync(companyId);
            var employerDeductions = await _repo.GetEmployerDeductionsAsync();

            var results = new List<EmployerDeductionResultDto>();

            foreach (var emp in empleados)
            {
                decimal totalEmployerDeductions = 0m;
                var deductionLines = new List<EmployerDeductionLineDto>();

                foreach (var ded in employerDeductions)
                {
                    EmployerDeductionLineDto line = null;
                    switch (ded.Code)
                    {
                        case "CCSS_SEM_ER":
                            line = new CcssSemEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "CCSS_IVM_ER":
                            line = new CcssIvmEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "BP_PATRON":
                            line = new BancoPopularEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "INA_PATR":
                            line = new InaEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "FCL_PATR":
                            line = new FclEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "FODESAF_PATR":
                            line = new FodesafEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        case "FPC_PATR":
                            line = new FpcEmployerCalc(ded.Rate).Calculate(emp.SalarioBruto);
                            break;
                        // Agrega otros casos si tienes más deducciones patronales
                    }
                    if (line != null)
                    {
                        deductionLines.Add(line);
                        totalEmployerDeductions += line.Amount;
                    }
                }

                results.Add(new EmployerDeductionResultDto
                {
                    IdEmpleado = emp.IdEmpleado,
                    IdEmpresa = emp.IdEmpresa,
                    Nombre = emp.Nombre,
                    Apellidos = emp.Apellidos,
                    SalarioBruto = emp.SalarioBruto,
                    TotalEmployerDeductions = Math.Round(totalEmployerDeductions, 2),
                    DeductionLines = deductionLines
                });
            }

            return results;
        }

        public async Task<int> GeneratePayrollWithBenefitsAsync(int companyId, int responsibleEmployeeId, int hours, string? periodType = null, int? fortnight = null)
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
                var existsQ = await _repo.ExistsPayrollForFortnightAsync(companyId, now.Year, now.Month, currentFortnight);
                if (existsQ)
                {
                    throw new InvalidOperationException("La planilla de esta quincena ya fue generada para esta empresa.");
                }
            }
            var employees = await GetEmployeePayrollWithDeductionsAsync(companyId);
            var employerDeductions = await GetEmployerPayrollWithDeductionsAsync(companyId);
            
            var payrollDetails = new List<PayrollDetailInsertDto>();
            
            foreach (var emp in employees)
            {
                var employerResult = employerDeductions.FirstOrDefault(e => e.IdEmpleado == emp.IdEmpleado);
                if (employerResult == null)
                {
                    continue;
                }
                
                var benefitCalculation = await _benefitDeductionsService.CalculateBenefitDeductionsAsync(emp.IdEmpleado, companyId);
                
                var totalBenefits = (decimal)benefitCalculation.Deductions.Sum(d => d.Amount);
                var employeeBenefitDeductions = (decimal)benefitCalculation.Deductions
                    .Where(d => d.Role == DeductionRoleNames.EmployeeDeduction)
                    .Sum(d => d.Amount);
                var employerBenefitDeductions = (decimal)benefitCalculation.Deductions
                    .Where(d => d.Role == DeductionRoleNames.EmployerDeduction)
                    .Sum(d => d.Amount);
                
                var totalEmployeeDeductions = emp.TotalEmployeeDeductions + employeeBenefitDeductions;
                var totalEmployerDeductions = employerResult.TotalEmployerDeductions + employerBenefitDeductions;
                
                var netSalary = emp.SalarioBruto - totalEmployeeDeductions;
                
                payrollDetails.Add(new PayrollDetailInsertDto
                {
                    IdEmpleado = emp.IdEmpleado,
                    SalarioBruto = (int)emp.SalarioBruto,
                    DeduccionesEmpleado = (int)Math.Round(totalEmployeeDeductions, 0),
                    DeduccionesEmpresa = (int)Math.Round(totalEmployerDeductions, 0),
                    TotalBeneficios = (int)Math.Round(totalBenefits, 0),
                    SalarioNeto = (int)Math.Round(netSalary, 0)
                });
            }
            
            var payrollId = await _repo.InsertPayrollAsync(new PayrollInsertDto
            {
                FechaGeneracion = now,
                Horas = hours,
                IdResponsable = responsibleEmployeeId,
                IdEmpresa = companyId
            });
            
            await _repo.InsertPayrollDetailsAsync(payrollId, payrollDetails);
            
            return payrollId;
        }

        public async Task<PayrollTotalsDto?> GetLatestPayrollTotalsByCompanyAsync(int companyId)
        {
            if (companyId <= 0) throw new ArgumentOutOfRangeException(nameof(companyId));
            return await _repo.GetLatestPayrollTotalsByCompanyAsync(companyId);
        }

        public async Task<List<PayrollHistoryItemDto>> GetPayrollHistoryByCompanyAsync(int companyId)
        {
            if (companyId <= 0) throw new ArgumentOutOfRangeException(nameof(companyId));
            return await _repo.GetPayrollHistoryByCompanyAsync(companyId);
        }
    }
}
