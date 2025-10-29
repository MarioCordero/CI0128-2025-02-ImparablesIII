using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using backend.DTOs;
using Dapper;

namespace backend.Repositories
{
    public class ProfileEmployeeRepository : IProfileEmployeeRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProfileEmployeeRepository> _logger;

        public ProfileEmployeeRepository(
            IConfiguration configuration,
            ILogger<ProfileEmployeeRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
            _logger = logger;
        }

        public async Task<ProfileEmployeeResponseDto> GetProfileByIdAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT
                        p.Nombre,
                        p.SegundoNombre,
                        p.Apellidos,
                        e.Puesto,
                        e.Departamento,
                        p.Correo,
                        p.Telefono,
                        d.Provincia,
                        d.Canton,
                        d.Distrito,
                        d.DireccionParticular,
                        e.Iban,
                        e.TipoContrato,
                        e.FechaContratacion,
                        e.Salario,
                        emp.Nombre AS NombreEmpresa,
                        e.TipoSalario AS PeriodoPago
                    FROM Planify.Persona p
                    INNER JOIN PlaniFy.Empleado e ON p.Id = e.IdPersona
                    LEFT JOIN PlaniFy.Direccion d ON p.IdDireccion = d.Id
                    LEFT JOIN PlaniFy.Empresa emp ON e.IdEmpresa = emp.Id
                    WHERE p.Id = @idPersona;";

                var parameters = new { idPersona = employeeId };

                var result = await connection.QueryFirstOrDefaultAsync<(string Nombre, string? SegundoNombre, string Apellidos, string Puesto, string Departamento, string Correo, int? Telefono, string? Provincia, string? Canton, string? Distrito, string? DireccionParticular, string Iban, string TipoContrato, DateTime FechaContratacion, int Salario, string? NombreEmpresa, string? PeriodoPago)>(sql, parameters);

                if (result == default)
                {
                    _logger.LogWarning("No se encontraron datos para el empleado {EmployeeId}", employeeId);
                    return new ProfileEmployeeResponseDto();
                }

                // Construir nombre completo
                string nombreCompleto = $"{result.Nombre} {result.SegundoNombre ?? ""} {result.Apellidos}".Trim();
                
                // Construir dirección completa
                string direccionCompleta = "Dirección no disponible";
                if (!string.IsNullOrWhiteSpace(result.Provincia) && !string.IsNullOrWhiteSpace(result.Canton))
                {
                    direccionCompleta = $"{result.Provincia}, {result.Canton}, {result.Distrito}, {result.DireccionParticular}";
                }

                return new ProfileEmployeeResponseDto
                {
                    User = new UserProfileDto
                    {
                        Nombre = nombreCompleto,
                        Puesto = result.Puesto,
                        Departamento = result.Departamento,
                        Correo = result.Correo,
                        Telefono = result.Telefono ?? 0,
                        Direccion = direccionCompleta,
                        IBAN = result.Iban,
                        TipoContrato = result.TipoContrato,
                        FechaContratacion = result.FechaContratacion,
                        Salario = result.Salario
                    },
                    Project = new ProjectInfoDto
                    {
                        NombreEmpresa = result.NombreEmpresa ?? "Empresa no asignada",
                        PeriodoPago = result.PeriodoPago ?? "No especificado"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo perfil del empleado {EmployeeId}", employeeId);
                return new ProfileEmployeeResponseDto();
            }
        }

        public async Task<bool> ExistsAsync(int employeeId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT COUNT(1) FROM Planify.Persona WHERE Id = @idPersona";
                var count = await connection.QuerySingleAsync<int>(sql, new { idPersona = employeeId });
                
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando existencia del empleado {EmployeeId}", employeeId);
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeProfileAsync(int employeeId, UpdateEmployeeProfileRequestDto updateRequest)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var transaction = await connection.BeginTransactionAsync();
            
            try
            {
                _logger.LogInformation("Iniciando actualización de perfil para empleado {EmployeeId}", employeeId);

                // 1. Primero obtener el ID de dirección si existe
                var getDireccionIdSql = "SELECT IdDireccion FROM Planify.Persona WHERE Id = @idPersona";
                var direccionId = await connection.QueryFirstOrDefaultAsync<int?>(
                    getDireccionIdSql, 
                    new { idPersona = employeeId }, 
                    transaction);

                int? nuevaDireccionId = null;

                // 2. Manejar la dirección (actualizar o insertar)
                if (direccionId.HasValue && direccionId.Value > 0)
                {
                    // Actualizar dirección existente
                    var updateDireccionSql = @"
                        UPDATE PlaniFy.Direccion 
                        SET Provincia = @Provincia, 
                            Canton = @Canton, 
                            Distrito = @Distrito, 
                            DireccionParticular = @DireccionParticular
                        WHERE Id = @DireccionId";

                    var direccionParams = new 
                    {
                        Provincia = updateRequest.Provincia,
                        Canton = updateRequest.Canton,
                        Distrito = updateRequest.Distrito,
                        DireccionParticular = updateRequest.DireccionParticular,
                        DireccionId = direccionId.Value
                    };

                    var direccionUpdated = await connection.ExecuteAsync(
                        updateDireccionSql, 
                        direccionParams, 
                        transaction);

                    if (direccionUpdated == 0)
                    {
                        _logger.LogWarning("No se pudo actualizar la dirección para empleado {EmployeeId}", employeeId);
                        await transaction.RollbackAsync();
                        return false;
                    }
                    nuevaDireccionId = direccionId.Value;
                }
                else
                {
                    // Insertar nueva dirección
                    var insertDireccionSql = @"
                        INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
                        OUTPUT INSERTED.Id
                        VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular)";

                    var direccionParams = new 
                    {
                        Provincia = updateRequest.Provincia,
                        Canton = updateRequest.Canton,
                        Distrito = updateRequest.Distrito,
                        DireccionParticular = updateRequest.DireccionParticular
                    };

                    nuevaDireccionId = await connection.QuerySingleAsync<int>(
                        insertDireccionSql, 
                        direccionParams, 
                        transaction);
                }

                // 3. Actualizar la persona
                var updatePersonaSql = @"
                    UPDATE Planify.Persona 
                    SET Nombre = @Nombre, 
                        SegundoNombre = @SegundoNombre, 
                        Apellidos = @Apellidos,
                        Telefono = @Telefono,
                        IdDireccion = @IdDireccion
                    WHERE Id = @idPersona";

                var personaParams = new 
                {
                    Nombre = updateRequest.Nombre,
                    SegundoNombre = string.IsNullOrWhiteSpace(updateRequest.SegundoNombre) ? null : updateRequest.SegundoNombre,
                    Apellidos = updateRequest.Apellidos,
                    Telefono = updateRequest.Telefono,
                    IdDireccion = nuevaDireccionId,
                    idPersona = employeeId
                };

                var personaUpdated = await connection.ExecuteAsync(
                    updatePersonaSql, 
                    personaParams, 
                    transaction);

                if (personaUpdated == 0)
                {
                    _logger.LogWarning("No se pudo actualizar la persona para empleado {EmployeeId}", employeeId);
                    await transaction.RollbackAsync();
                    return false;
                }

                // 4. Actualizar el empleado (IBAN)
                var updateEmpleadoSql = @"
                    UPDATE PlaniFy.Empleado 
                    SET Iban = @IBAN,
                    Departamento = @Departamento,
                    Puesto = @Puesto, 
                    Salario = @Salario
                    WHERE IdPersona = @idPersona";

                var empleadoParams = new
                {
                    IBAN = updateRequest.IBAN,
                    Departamento = updateRequest.Departamento,
                    Puesto = updateRequest.Puesto,
                    Salario = updateRequest.Salario, 
                    idPersona = employeeId
                };

                var empleadoUpdated = await connection.ExecuteAsync(
                    updateEmpleadoSql, 
                    empleadoParams, 
                    transaction);

                if (empleadoUpdated == 0)
                {
                    _logger.LogWarning("No se pudo actualizar el empleado para ID {EmployeeId}", employeeId);
                    await transaction.RollbackAsync();
                    return false;
                }

                // Si todo salió bien, confirmar la transacción
                await transaction.CommitAsync();
                _logger.LogInformation("Perfil actualizado exitosamente para empleado {EmployeeId}", employeeId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error actualizando perfil del empleado {EmployeeId}", employeeId);
                return false;
            }
        }
    }
}