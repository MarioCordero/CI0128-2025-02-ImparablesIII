using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Text.Json;
using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly string _connectionString;
        public PayrollRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<(List<EmployeePayrollDto> Items, PayrollTotalsDto Totals)> ExecutePayrollReportAsync( int companyId, int year, int month, string periodType, int? fortnight, string? department) 
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new
                {
                    CompanyId = companyId,
                    Year = year,
                    Month = month,
                    PeriodType = periodType,
                    Fortnight = fortnight,
                    Department = department
                };

                using var multi = await connection.QueryMultipleAsync(
                    "PlaniFy.sp_PayrollReport",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                var items = (await multi.ReadAsync<EmployeePayrollDto>()).ToList();
                var totals = await multi.ReadFirstOrDefaultAsync<PayrollTotalsDto>() ?? new PayrollTotalsDto();

                return (items, totals);
            }
            catch (Exception)
            {
                return (new List<EmployeePayrollDto>(), new PayrollTotalsDto());
            }
        }

        public async Task<List<EmployeeRow>> GetEmployeesAsync(int companyId, string? department)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT e.idPersona AS IdEmpleado, p.Nombre, p.Apellidos, e.Departamento, e.Salario
                    FROM PlaniFy.Empleado e
                    JOIN PlaniFy.Persona p ON p.Id = e.idPersona
                    WHERE e.idEmpresa = @CompanyId
                    AND (@Department IS NULL OR e.Departamento = @Department);";

                var parameters = new
                {
                    CompanyId = companyId,
                    Department = department
                };

                var result = (await connection.QueryAsync<EmployeeRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<EmployeeRow>();
            }
        }

        public async Task<List<PayrollRow>> GetPayrollsAsync(int companyId, DateTime start, DateTime end)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT Id, idEmpresa AS EmpresaId, FechaGeneracion, Estado, FechaPago
                    FROM PlaniFy.Planilla
                    WHERE idEmpresa = @CompanyId
                    AND CAST(FechaGeneracion AS DATE) BETWEEN @Start AND @End;";

                var parameters = new
                {
                    CompanyId = companyId,
                    Start = start,
                    End = end
                };

                var result = (await connection.QueryAsync<PayrollRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<PayrollRow>();
            }
        }

        public async Task<List<PayrollDetailRow>> GetPayrollDetailsAsync(IEnumerable<int> payrollIds, IEnumerable<int> employeeIds)
        {
            try
            {
                if (!payrollIds.Any() || !employeeIds.Any()) return new List<PayrollDetailRow>();

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT idEmpleado, idPlanilla, salarioBruto
                    FROM PlaniFy.DetallePlanilla
                    WHERE idPlanilla IN @PayrollIds AND idEmpleado IN @EmployeeIds;";

                var parameters = new
                {
                    PayrollIds = payrollIds.ToArray(),
                    EmployeeIds = employeeIds.ToArray()
                };

                var result = (await connection.QueryAsync<PayrollDetailRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<PayrollDetailRow>();
            }
        }

        public async Task<List<DeductionEmployeeRow>> GetDeductionsAsync(IEnumerable<int> payrollIds, IEnumerable<int> employeeIds)
        {
            try
            {
                if (!payrollIds.Any() || !employeeIds.Any()) return new List<DeductionEmployeeRow>();

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT idPlanilla, idEmpleado, Tipo, Nombre, Monto
                    FROM PlaniFy.DeduccionEmpleado
                    WHERE idPlanilla IN @PayrollIds AND idEmpleado IN @EmployeeIds;";

                var parameters = new
                {
                    PayrollIds = payrollIds.ToArray(),
                    EmployeeIds = employeeIds.ToArray()
                };

                var result = (await connection.QueryAsync<DeductionEmployeeRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<DeductionEmployeeRow>();
            }
        }

        public async Task<List<BenefitRow>> GetBenefitsAsync(int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT * FROM PlaniFy.Beneficio WHERE idEmpresa = @CompanyId;";

                var parameters = new { CompanyId = companyId };

                var result = (await connection.QueryAsync<BenefitRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<BenefitRow>();
            }
        }

        public async Task<List<BenefitEmployeeRow>> GetEmployeeBenefitsAsync(int companyId, IEnumerable<int> employeeIds)
        {
            try
            {
                if (!employeeIds.Any()) return new List<BenefitEmployeeRow>();

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT idEmpleado, NombreBeneficio, idEmpresa, TipoBeneficio
                    FROM PlaniFy.BeneficioEmpleado
                    WHERE idEmpresa = @CompanyId AND idEmpleado IN @EmployeeIds;";

                var parameters = new
                {
                    CompanyId = companyId,
                    EmployeeIds = employeeIds.ToArray()
                };

                var result = (await connection.QueryAsync<BenefitEmployeeRow>(query, parameters)).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<BenefitEmployeeRow>();
            }
        }
        public async Task<List<EmployeeDeductionDto>> GetEmployeeDeductionsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT Id, Code, Name, Rate, MinAmount, MaxAmount, IsActive
                        FROM PlaniFy.EmployeeDeductions WHERE IsActive = 1;";
            var result = (await connection.QueryAsync<EmployeeDeductionDto>(query)).ToList();
            return result;
        }

        public async Task<List<EmployerDeductionDto>> GetEmployerDeductionsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT Id, Code, Name, Rate, MinAmount, MaxAmount, IsActive
                        FROM PlaniFy.EmployerDeductions WHERE IsActive = 1;";
            var result = (await connection.QueryAsync<EmployerDeductionDto>(query)).ToList();
            return result;
        }

        public async Task<List<EmployeePayrollDto>> GetEmployeesForPayrollAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PlaniFy.GetEmployeesForPayroll", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@CompanyId", companyId);

            string jsonResult = "";
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    jsonResult += reader.GetString(0);
                }
            }

            var empleadosRoot = JsonSerializer.Deserialize<EmpleadosRoot>(jsonResult);
            return empleadosRoot?.Empleados ?? new List<EmployeePayrollDto>();
        }

        public async Task<int> InsertPayrollAsync(PayrollInsertDto payroll)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO PlaniFy.Planilla (FechaGeneracion, Horas, idResponsable, idEmpresa)
                OUTPUT INSERTED.id
                VALUES (@FechaGeneracion, @Horas, @IdResponsable, @IdEmpresa);";

            var parameters = new
            {
                payroll.FechaGeneracion,
                payroll.Horas,
                payroll.IdResponsable,
                payroll.IdEmpresa
            };

            var payrollId = await connection.QuerySingleAsync<int>(query, parameters);
            return payrollId;
        }

        public async Task InsertPayrollDetailsAsync(int payrollId, List<PayrollDetailInsertDto> details)
        {
            if (details == null || !details.Any())
            {
                return;
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO PlaniFy.DetallePlanilla (idEmpleado, idPlanilla, salarioBruto, DeduccionesEmpleado, DeduccionesEmpresa, totalBeneficios, salarioNeto)
                VALUES (@IdEmpleado, @IdPlanilla, @SalarioBruto, @DeduccionesEmpleado, @DeduccionesEmpresa, @TotalBeneficios, @SalarioNeto);";

            var parameters = details.Select(d => new
            {
                d.IdEmpleado,
                IdPlanilla = payrollId,
                d.SalarioBruto,
                d.DeduccionesEmpleado,
                d.DeduccionesEmpresa,
                d.TotalBeneficios,
                d.SalarioNeto
            });

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<PayrollTotalsDto?> GetLatestPayrollTotalsByCompanyAsync(int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    WITH LastPlanilla AS (
                        SELECT TOP 1 id
                        FROM PlaniFy.Planilla
                        WHERE idEmpresa = @CompanyId
                        ORDER BY FechaGeneracion DESC, id DESC
                    )
                    SELECT 
                        CAST(SUM(d.salarioBruto) AS DECIMAL(18,2)) AS TotalGross,
                        CAST(SUM(d.DeduccionesEmpleado) AS DECIMAL(18,2)) AS TotalEmployeeDeductions,
                        CAST(SUM(d.DeduccionesEmpresa) AS DECIMAL(18,2)) AS TotalEmployerDeductions,
                        CAST(SUM(d.totalBeneficios) AS DECIMAL(18,2)) AS TotalBenefits,
                        CAST(SUM(d.salarioNeto) AS DECIMAL(18,2)) AS TotalNet,
                        COUNT(*) AS EmployeeCount
                    FROM PlaniFy.DetallePlanilla d
                    WHERE d.idPlanilla = (SELECT id FROM LastPlanilla);";

                var totals = await connection.QueryFirstOrDefaultAsync<PayrollTotalsDto>(sql, new { CompanyId = companyId });
                return totals;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> ExistsPayrollForMonthAsync(int companyId, int year, int month)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT CASE WHEN EXISTS (
                            SELECT 1 FROM PlaniFy.Planilla 
                            WHERE idEmpresa = @CompanyId 
                              AND YEAR(FechaGeneracion) = @Year 
                              AND MONTH(FechaGeneracion) = @Month)
                        THEN 1 ELSE 0 END";

            var exists = await connection.ExecuteScalarAsync<int>(sql, new { CompanyId = companyId, Year = year, Month = month });
            return exists == 1;
        }

        public async Task<bool> ExistsPayrollForFortnightAsync(int companyId, int year, int month, int fortnight)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fortnight 1: days 1-15, Fortnight 2: days 16-end of month
            var sql = @"SELECT CASE WHEN EXISTS (
                            SELECT 1 FROM PlaniFy.Planilla 
                            WHERE idEmpresa = @CompanyId 
                              AND YEAR(FechaGeneracion) = @Year 
                              AND MONTH(FechaGeneracion) = @Month
                              AND (
                                  (@Fortnight = 1 AND DAY(FechaGeneracion) BETWEEN 1 AND 15)
                                  OR (@Fortnight = 2 AND DAY(FechaGeneracion) BETWEEN 16 AND DAY(EOMONTH(FechaGeneracion)))
                              ))
                        THEN 1 ELSE 0 END";

            var exists = await connection.ExecuteScalarAsync<int>(sql, new { CompanyId = companyId, Year = year, Month = month, Fortnight = fortnight });
            return exists == 1;
        }

        public async Task<List<PayrollHistoryItemDto>> GetPayrollHistoryByCompanyAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT 
                    p.id AS PayrollId,
                    p.FechaGeneracion,
                    CAST(r.TotalSalarioBruto AS DECIMAL(18,2)) AS TotalGross,
                    CAST(r.TotalDeduccionesEmpleado AS DECIMAL(18,2)) AS TotalEmployeeDeductions,
                    CAST(r.TotalDeduccionesEmpresa AS DECIMAL(18,2)) AS TotalEmployerDeductions,
                    CAST(r.TotalBeneficios AS DECIMAL(18,2)) AS TotalBenefits,
                    CAST(r.TotalSalarioNeto AS DECIMAL(18,2)) AS TotalNet,
                    (SELECT COUNT(*) FROM PlaniFy.DetallePlanilla d WHERE d.idPlanilla = p.id) AS EmployeeCount
                FROM PlaniFy.Planilla p
                JOIN PlaniFy.ResumenPlanilla r ON r.idPlanilla = p.id AND r.idEmpresa = p.idEmpresa
                WHERE p.idEmpresa = @CompanyId
                ORDER BY p.FechaGeneracion DESC, p.id DESC;";

            var rows = (await connection.QueryAsync<PayrollHistoryItemDto>(sql, new { CompanyId = companyId })).ToList();
            return rows;
        }
    }
}