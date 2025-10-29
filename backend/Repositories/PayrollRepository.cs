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
    }
}