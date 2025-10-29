using backend.DTOs; 
using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;


namespace backend.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;
        private readonly IDireccionRepository _direccionRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeRepository> _logger; // ← Agregar esta línea

        public EmployeeRepository(IConfiguration configuration,
                                IDireccionRepository direccionRepository,
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
                // First, insert the address using common repository
                var direccionId = await _direccionRepository.CreateDireccionAsync(
                    employeeDto.Provincia, 
                    employeeDto.Canton, 
                    employeeDto.Distrito, 
                    employeeDto.DireccionParticular);
                
                if (direccionId == -1)
                    throw new Exception("Failed to create address");
                
                // Then, insert the person using common repository
                var personaId = await _personaRepository.CreatePersonaAsync(
                    employeeDto.Correo,
                    employeeDto.PrimerNombre,
                    employeeDto.SegundoNombre,
                    employeeDto.PrimerApellido,
                    employeeDto.FechaNacimiento,
                    employeeDto.Cedula,
                    "Empleado", // Default role for employees
                    employeeDto.Telefono,
                    direccionId);
                
                if (personaId == -1)
                    throw new Exception("Failed to create person");
                
                // Finally, insert the employee
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


        private async Task InsertEmpleadoAsync(SqlConnection connection, SqlTransaction transaction, RegisterEmployeeDto employeeDto, int personaId)
        {
            // Use ProjectId if available, otherwise fall back to IdEmpresa
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
                TipoSalario = "Mensual", // Default to monthly salary
                Puesto = employeeDto.Puesto,
                FechaContratacion = DateTime.Now, // Current date as hire date
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
                INNER JOIN PlaniFy.Persona p ON e.idPersona = p.idPersona
                WHERE e.EmpleadoId = @EmployeeId";

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

         public async Task<List<EmployeeListDto>> GetEmployeesByCompanyAsync(int companyId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        p.Id,
                        CONCAT_WS(' ', p.Nombre, p.SegundoNombre, p.Apellidos) AS NombreCompleto,
                        p.Correo,
                        p.Telefono,
                        e.Puesto,
                        e.Departamento,
                        e.Salario,
                        e.TipoContrato
                    FROM Planify.Persona p
                    INNER JOIN PlaniFy.Empleado e ON p.Id = e.IdPersona
                    WHERE e.IdEmpresa = @CompanyId";

                var parameters = new { CompanyId = companyId };

                var employees = await connection.QueryAsync<EmployeeListDto>(sql, parameters);
                
                return employees.AsList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo empleados para la empresa {CompanyId}", companyId);
                return new List<EmployeeListDto>();
            }
        }
    }
}
