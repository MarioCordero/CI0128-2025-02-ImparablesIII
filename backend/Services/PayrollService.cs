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

        // public async Task<PayrollSummaryDto> GetReportAsync(PayrollFiltersDto f)
        // {
        //     // Valores hardcodeados
        //     var start = new DateTime(2025, 10, 1);
        //     var end = new DateTime(2025, 10, 31);
        //     var label = "Monthly 2025-10";
        //     int year = 2025, month = 10;
        //     int? fortnight = null;

        //     // Ejemplo de items y totales hardcodeados
        //     var items = new List<PayrollItemDto>
        //     {
        //         new PayrollItemDto
        //         {
        //             EmployeeId = 1,
        //             EmployeeName = "Mario Jiménez",
        //             GrossSalary = 1000000m,
        //             TotalDeductions = 106700m,
        //             NetSalary = 893300m
        //         }
        //     };

        //     var totals = new PayrollTotalsDto
        //     {
        //         TotalGross = 1000000m,
        //         TotalDeductions = 106700m,
        //         TotalNet = 893300m
        //     };

        //     return new PayrollSummaryDto
        //     {
        //         PeriodLabel = label,
        //         Items = items,
        //         Totals = totals
        //     };
        // }


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
        // ----------------------------------------- MONSTER METHODS -----------------------------------------
        private async Task<List<EmployeeDeductionDto>> GetEmployeeDeductionsAsync()
        {
            // Consulta los montos y reglas de deducción de empleado desde la base de datos
            return await _repo.GetEmployeeDeductionsAsync();
        }

        private async Task<List<EmployerDeductionDto>> GetEmployerDeductionsAsync()
        {
            // Consulta los montos y reglas de deducción de empleador desde la base de datos
            return await _repo.GetEmployerDeductionsAsync();
        }
        // ----------------------------------------- EMPLOYEE METHODS -----------------------------------------
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

        // ----------------------------------------- ORCHESTRATOR METHOD -----------------------------------------
        // public async Task<PayrollCalculationResultDto> CalculatePayrollAsync(PayrollCalculationRequestDto request)
        // {
        //     if (request.CompanyId <= 0)
        //         throw new ArgumentException("CompanyId inválido.");

        //     var empleados = await _repo.GetEmployeesForPayrollAsync(request.CompanyId);


        //     // 2. Consultar deducciones desde la base de datos

        //     // 3. Instanciar calculadoras dinámicamente según deducciones
        //     // TODO: Crear instancias de calculadoras usando los datos de la BD

        //     // 4. Calcular deducciones de empleado
        //     var employeeResults = new List<CalcLine>();
        //     foreach (var ded in employeeDeductions)
        //     {
        //         // TODO: Ejecutar cálculo usando ded.Rate, ded.MinAmount, ded.MaxAmount, etc.
        //         // employeeResults.Add(calculatedLine);
        //     }

        //     // 5. Calcular deducciones de empleador
        //     var employerResults = new List<CalcLine>();
        //     foreach (var ded in employerDeductions)
        //     {
        //         // TODO: Ejecutar cálculo usando ded.Rate, ded.MinAmount, ded.MaxAmount, etc.
        //         // employerResults.Add(calculatedLine);
        //     }

        //     // 6. Calcular totales y neto
        //     // TODO: Sumar deducciones, calcular salario neto, etc.

        //     // 7. Mapear a DTO de respuesta
        //     var result = new PayrollCalculationResultDto
        //     {
        //         // TODO: Asignar resultados, totales, detalles, etc.
        //     };

        //     return result;
        // }
    }
}
