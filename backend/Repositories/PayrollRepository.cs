using Dapper;
using backend.DTOs;
using backend.Models;
using System.Data;

namespace backend.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly IDbConnection _dbConnection;

        public PayrollRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Payroll>> GetPayrollByPeriodAsync(DateTime period, string periodType)
        {
            var sql = @"
                SELECT p.*, e.*, per.* 
                FROM Payroll p
                INNER JOIN Empleado e ON p.EmployeeId = e.EmpleadoId
                INNER JOIN Persona per ON e.PersonaId = per.PersonaId
                WHERE p.Period = @Period AND p.PeriodType = @PeriodType";

            return await _dbConnection.QueryAsync<Payroll, Empleado, Persona, Payroll>(
                sql,
                (payroll, employee, person) =>
                {
                    employee.Persona = person;
                    payroll.Employee = employee;
                    return payroll;
                },
                new { Period = period, PeriodType = periodType },
                splitOn: "EmpleadoId,PersonaId"
            );
        }

        public async Task<IEnumerable<Payroll>> GetPayrollByFiltersAsync(PayrollFiltersDto filters)
        {
            var sql = @"
                SELECT p.*, e.*, per.*, d.Nombre as DepartamentoNombre
                FROM Payroll p
                INNER JOIN Empleado e ON p.EmployeeId = e.EmpleadoId
                INNER JOIN Persona per ON e.PersonaId = per.PersonaId
                LEFT JOIN Departamento d ON e.DepartamentoId = d.DepartamentoId
                WHERE p.Period = @Period AND p.PeriodType = @PeriodType";

            if (filters.DepartmentId.HasValue)
            {
                sql += " AND e.DepartamentoId = @DepartmentId";
            }

            return await _dbConnection.QueryAsync<Payroll, Empleado, Persona, Payroll>(
                sql,
                (payroll, employee, person) =>
                {
                    employee.Persona = person;
                    payroll.Employee = employee;
                    return payroll;
                },
                new { 
                    Period = filters.Period, 
                    PeriodType = filters.PeriodType,
                    DepartmentId = filters.DepartmentId
                },
                splitOn: "EmpleadoId,PersonaId"
            );
        }

        public async Task<IEnumerable<PayrollDetailDto>> GetPayrollDetailsAsync(PayrollFiltersDto filters)
        {
            var sql = @"
                SELECT 
                    p.PayrollId,
                    p.EmployeeId,
                    CONCAT(per.Nombre, ' ', per.Apellido1) as EmployeeName,
                    p.GrossSalary,
                    p.CcssEmployee,
                    p.CcssEmployer,
                    p.IncomeTax,
                    p.OtherDeductions,
                    p.Benefits,
                    p.NetSalary,
                    p.Period,
                    p.Status,
                    d.Nombre as Department
                FROM Payroll p
                INNER JOIN Empleado e ON p.EmployeeId = e.EmpleadoId
                INNER JOIN Persona per ON e.PersonaId = per.PersonaId
                LEFT JOIN Departamento d ON e.DepartamentoId = d.DepartamentoId
                WHERE p.Period = @Period AND p.PeriodType = @PeriodType";

            if (filters.DepartmentId.HasValue)
            {
                sql += " AND e.DepartamentoId = @DepartmentId";
            }

            return await _dbConnection.QueryAsync<PayrollDetailDto>(sql, new
            {
                Period = filters.Period,
                PeriodType = filters.PeriodType,
                DepartmentId = filters.DepartmentId
            });
        }

        public async Task<Payroll> GetPayrollByIdAsync(int payrollId)
        {
            var sql = "SELECT * FROM Payroll WHERE PayrollId = @PayrollId";
            return await _dbConnection.QueryFirstOrDefaultAsync<Payroll>(sql, new { PayrollId = payrollId });
        }

        public async Task<Payroll> CreatePayrollAsync(Payroll payroll)
        {
            var sql = @"
                INSERT INTO Payroll (EmployeeId, Period, PeriodType, GrossSalary, CcssEmployee, 
                                   CcssEmployer, IncomeTax, OtherDeductions, Benefits, NetSalary, Status)
                OUTPUT INSERTED.*
                VALUES (@EmployeeId, @Period, @PeriodType, @GrossSalary, @CcssEmployee, 
                       @CcssEmployer, @IncomeTax, @OtherDeductions, @Benefits, @NetSalary, @Status)";

            return await _dbConnection.QueryFirstOrDefaultAsync<Payroll>(sql, payroll);
        }

        public async Task<Payroll> UpdatePayrollAsync(Payroll payroll)
        {
            var sql = @"
                UPDATE Payroll 
                SET GrossSalary = @GrossSalary, CcssEmployee = @CcssEmployee, CcssEmployer = @CcssEmployer,
                    IncomeTax = @IncomeTax, OtherDeductions = @OtherDeductions, Benefits = @Benefits,
                    NetSalary = @NetSalary, Status = @Status, UpdatedAt = GETUTCDATE()
                OUTPUT INSERTED.*
                WHERE PayrollId = @PayrollId";

            return await _dbConnection.QueryFirstOrDefaultAsync<Payroll>(sql, payroll);
        }

        public async Task<bool> DeletePayrollAsync(int payrollId)
        {
            var sql = "DELETE FROM Payroll WHERE PayrollId = @PayrollId";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, new { PayrollId = payrollId });
            return affectedRows > 0;
        }

        public async Task<Payroll> GetEmployeePayrollAsync(int employeeId, DateTime period, string periodType)
        {
            var sql = @"
                SELECT * FROM Payroll 
                WHERE EmployeeId = @EmployeeId AND Period = @Period AND PeriodType = @PeriodType";

            return await _dbConnection.QueryFirstOrDefaultAsync<Payroll>(sql, new
            {
                EmployeeId = employeeId,
                Period = period,
                PeriodType = periodType
            });
        }

        public async Task<bool> IsPayrollProcessedAsync(int employeeId, DateTime period, string periodType)
        {
            var sql = @"
                SELECT COUNT(1) FROM Payroll 
                WHERE EmployeeId = @EmployeeId AND Period = @Period AND PeriodType = @PeriodType";

            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                EmployeeId = employeeId,
                Period = period,
                PeriodType = periodType
            });

            return count > 0;
        }
    }
}