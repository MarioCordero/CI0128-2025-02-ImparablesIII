using backend.DTOs; 
using backend.Models;
using backend.Constants;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;


namespace backend.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;
        private readonly IDirectionRepository _direccionRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(IConfiguration configuration,
                                IDirectionRepository direccionRepository,
                                IPersonaRepository personaRepository,
                                IUsuarioRepository usuarioRepository,
                                ILogger<EmployeeRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _direccionRepository = direccionRepository;
            _personaRepository = personaRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var direccionId = await CreateAddressAsync(employeeDto);
                var personaId = await CreatePersonaAsync(employeeDto, direccionId);
                await InsertEmpleadoAsync(connection, transaction, employeeDto, personaId);
                transaction.Commit();
                return personaId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        private async Task<int> CreateAddressAsync(RegisterEmployeeDto employeeDto)
        {
            var direccionId = await _direccionRepository.CreateDireccionAsync(
                employeeDto.Provincia, 
                employeeDto.Canton, 
                employeeDto.Distrito, 
                employeeDto.DireccionParticular);
            
            if (direccionId == -1)
            {
                throw new InvalidOperationException("Failed to create address");
            }

            return direccionId;
        }

        private async Task<int> CreatePersonaAsync(RegisterEmployeeDto employeeDto, int direccionId)
        {
            var persona = new Persona
            {
                Correo = employeeDto.Correo,
                Nombre = employeeDto.PrimerNombre,
                SegundoNombre = employeeDto.SegundoNombre,
                Apellidos = employeeDto.PrimerApellido,
                FechaNacimiento = employeeDto.FechaNacimiento,
                Cedula = employeeDto.Cedula,
                Rol = EmployeeConstants.PersonTypeEmployee,
                Telefono = employeeDto.Telefono,
                IdDireccion = direccionId
            };

            var createdPersona = await _personaRepository.CreatePersonaAsync(persona);
            
            if (createdPersona == null)
            {
                throw new InvalidOperationException("Failed to create person");
            }

            return createdPersona;
        }

        private async Task InsertEmpleadoAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            RegisterEmployeeDto employeeDto, 
            int personaId)
        {
            var empresaId = employeeDto.ProjectId ?? employeeDto.IdEmpresa;
            
            if (!empresaId.HasValue)
            {
                throw new ArgumentException("Either ProjectId or IdEmpresa must be provided");
            }

            var query = @"
                INSERT INTO PlaniFy.Empleado (idPersona, Departamento, TipoContrato, TipoSalario, Puesto, FechaContratacion, Salario, iban, Contrasena, idEmpresa)
                VALUES (@IdPersona, @Departamento, @TipoContrato, @TipoSalario, @Puesto, @FechaContratacion, @Salario, @Iban, @Contrasena, @IdEmpresa)";

            var parameters = new
            {
                IdPersona = personaId,
                Departamento = employeeDto.Departamento,
                TipoContrato = employeeDto.TipoContrato,
                TipoSalario = EmployeeConstants.SalaryTypeMonthly,
                Puesto = employeeDto.Puesto,
                FechaContratacion = DateTime.Now,
                Salario = employeeDto.Salario,
                Iban = employeeDto.NumeroCuentaIban,
                Contrasena = employeeDto.Contrasena,
                IdEmpresa = empresaId.Value
            };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        public async Task<bool> ValidateCedulaExistsAsync(string cedula)
        {
            return !await _personaRepository.IsCedulaAvailableAsync(cedula);
        }

        public async Task<bool> ValidateEmailExistsAsync(string email)
        {
            return !await _personaRepository.IsEmailAvailableAsync(email);
        }

        public async Task<Empleado?> GetEmployeeByIdAsync(int employeeId)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT e.*, p.*
                FROM PlaniFy.Empleado e
                INNER JOIN PlaniFy.Persona p ON e.idPersona = p.Id
                WHERE e.idPersona = @EmployeeId";

            var employee = await connection.QueryAsync<Empleado, Persona, Empleado>(
                query,
                (e, p) => { e.Persona = p; return e; },
                new { EmployeeId = employeeId }
            );

            return employee.FirstOrDefault();
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int?> GetEmployeeCompanyIdAsync(int employeeId)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT idEmpresa
                FROM PlaniFy.Empleado
                WHERE idPersona = @EmployeeId";

            return await connection.QueryFirstOrDefaultAsync<int?>(query, new { EmployeeId = employeeId });
        }

        public async Task<int> GetEmployeeAgeAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var query = @"
                    SELECT p.FechaNacimiento
                    FROM PlaniFy.Persona p
                    INNER JOIN PlaniFy.Empleado e ON p.Id = e.idPersona
                    WHERE e.idPersona = @EmployeeId";

                var fechaNacimiento = await connection.QueryFirstOrDefaultAsync<DateTime?>(query, new { EmployeeId = employeeId });

                if (!fechaNacimiento.HasValue)
                {
                    throw new InvalidOperationException($"Employee with ID {employeeId} not found or has no birth date");
                }

                var today = DateTime.Today;
                var age = today.Year - fechaNacimiento.Value.Year;

                return age;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to calculate age for employee {employeeId}", ex);
            }
        }

        public async Task<List<EmployeeListDto>> GetEmployeesByCompanyAsync(int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = GetEmployeesByCompanyQuery();
                var employees = await connection.QueryAsync<EmployeeListDto>(
                    query, 
                    new { CompanyId = companyId, StatusActive = EmployeeConstants.StatusActive });

                return employees.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees for company {CompanyId}", companyId);
                throw new InvalidOperationException($"Failed to retrieve employees for company {companyId}", ex);
            }
        }

        private string GetEmployeesByCompanyQuery()
        {
            return @"
                SELECT 
                    e.idPersona AS Id,
                    CONCAT(p.Nombre, ' ', COALESCE(p.SegundoNombre + ' ', ''), p.Apellidos) AS NombreCompleto,
                    p.Correo,
                    p.Telefono,
                    e.Puesto,
                    e.Departamento,
                    e.Salario,
                    e.TipoContrato
                FROM PlaniFy.Empleado e
                INNER JOIN PlaniFy.Persona p ON p.Id = e.idPersona
                WHERE e.idEmpresa = @CompanyId
                AND (e.Estado = @StatusActive OR e.Estado IS NULL)";
        }

        public async Task<bool> HasPayrollRecordsAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(*)
                    FROM PlaniFy.DetallePlanilla
                    WHERE idEmpleado = @EmployeeId";

                var count = await connection.QueryFirstOrDefaultAsync<int>(query, new { EmployeeId = employeeId });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payroll records for employee {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to check payroll records for employee {employeeId}", ex);
            }
        }

        public async Task<int> GetPayrollRecordsCountAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(*)
                    FROM PlaniFy.DetallePlanilla
                    WHERE idEmpleado = @EmployeeId";

                return await connection.QueryFirstOrDefaultAsync<int>(query, new { EmployeeId = employeeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll records count for employee {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to get payroll records count for employee {employeeId}", ex);
            }
        }

        public async Task<bool> DeleteEmployeeLogicallyAsync(int employeeId, int deletedByUserId, string motivoBaja)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var query = GetLogicalDeletionQuery();
                var motivoBajaValue = motivoBaja ?? EmployeeConstants.DefaultDeletionReason;
                
                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    EmployeeId = employeeId,
                    MotivoBaja = motivoBajaValue,
                    UsuarioBajaId = deletedByUserId,
                    StatusInactive = EmployeeConstants.StatusInactive
                }, transaction);

                if (rowsAffected > 0)
                {
                    await DeleteUserRecordAsync(connection, transaction, employeeId);
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error performing logical deletion for employee {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to delete employee {employeeId} logically", ex);
            }
        }

        private string GetLogicalDeletionQuery()
        {
            return @"
                UPDATE PlaniFy.Empleado
                SET Estado = @StatusInactive,
                    FechaBaja = GETDATE(),
                    MotivoBaja = @MotivoBaja,
                    UsuarioBajaId = @UsuarioBajaId
                WHERE idPersona = @EmployeeId";
        }

        public async Task<bool> DeleteEmployeePhysicallyAsync(int employeeId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                await DeleteEmployeeBenefitsAsync(connection, transaction, employeeId);
                await DeleteEmployeeHoursAsync(connection, transaction, employeeId);
                var employeeRowsAffected = await DeleteEmployeeRecordAsync(connection, transaction, employeeId);
                await DeleteUserRecordAsync(connection, transaction, employeeId);
                await DeletePersonRecordAsync(connection, transaction, employeeId);

                transaction.Commit();
                return employeeRowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error performing physical deletion for employee {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to delete employee {employeeId} physically", ex);
            }
        }

        private async Task DeleteEmployeeBenefitsAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            int employeeId)
        {
            var query = @"
                DELETE FROM PlaniFy.BeneficioEmpleado
                WHERE idEmpleado = @EmployeeId";

            await connection.ExecuteAsync(query, new { EmployeeId = employeeId }, transaction);
        }

        private async Task DeleteEmployeeHoursAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            int employeeId)
        {
            var query = @"
                DELETE FROM PlaniFy.HorasTrabajadas
                WHERE idEmpleado = @EmployeeId";

            await connection.ExecuteAsync(query, new { EmployeeId = employeeId }, transaction);
        }

        private async Task<int> DeleteEmployeeRecordAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            int employeeId)
        {
            var query = @"
                DELETE FROM PlaniFy.Empleado
                WHERE idPersona = @EmployeeId";

            return await connection.ExecuteAsync(query, new { EmployeeId = employeeId }, transaction);
        }

        private async Task DeleteUserRecordAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            int employeeId)
        {
            var query = @"
                DELETE FROM PlaniFy.Usuario
                WHERE idPersona = @EmployeeId";

            await connection.ExecuteAsync(query, new { EmployeeId = employeeId }, transaction);
        }

        private async Task DeletePersonRecordAsync(
            SqlConnection connection, 
            SqlTransaction transaction, 
            int employeeId)
        {
            var query = @"
                DELETE FROM PlaniFy.Persona
                WHERE Id = @EmployeeId";

            await connection.ExecuteAsync(query, new { EmployeeId = employeeId }, transaction);
        }

        public async Task<bool> IsEmployeeDeletedAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(*)
                    FROM PlaniFy.Empleado e
                    WHERE e.idPersona = @EmployeeId
                    AND e.Estado = @StatusInactive";

                var count = await connection.QueryFirstOrDefaultAsync<int>(
                    query, 
                    new { EmployeeId = employeeId, StatusInactive = EmployeeConstants.StatusInactive });

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if employee {EmployeeId} is deleted", employeeId);
                throw new InvalidOperationException($"Failed to check deletion status for employee {employeeId}", ex);
            }
        }

        public async Task<EmployeeDeletionInfoDto> GetEmployeeDeletionInfoAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = GetDeletionInfoQuery();
                var result = await connection.QueryFirstOrDefaultAsync<(string EmployeeName, int PayrollRecordsCount)>(
                    query, new { EmployeeId = employeeId });

                if (result.EmployeeName == null)
                {
                    throw new KeyNotFoundException($"Employee with ID {employeeId} not found");
                }

                return CreateDeletionInfoDto(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deletion info for employee {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to get deletion info for employee {employeeId}", ex);
            }
        }

        private string GetDeletionInfoQuery()
        {
            return @"
                SELECT 
                    CONCAT(p.Nombre, ' ', COALESCE(p.SegundoNombre + ' ', ''), p.Apellidos) AS EmployeeName,
                    (SELECT COUNT(*) FROM PlaniFy.DetallePlanilla WHERE idEmpleado = @EmployeeId) AS PayrollRecordsCount
                FROM PlaniFy.Empleado e
                INNER JOIN PlaniFy.Persona p ON p.Id = e.idPersona
                WHERE e.idPersona = @EmployeeId";
        }

        private EmployeeDeletionInfoDto CreateDeletionInfoDto((string EmployeeName, int PayrollRecordsCount) result)
        {
            return new EmployeeDeletionInfoDto
            {
                EmployeeName = result.EmployeeName,
                PayrollRecordsCount = result.PayrollRecordsCount,
                HasPayrollRecords = result.PayrollRecordsCount > 0
            };
        }
    }
}
