using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace backend.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("CountryContext") ?? throw new InvalidOperationException("Connection string 'CountryContext' not found.");
        }

        public async Task<int> RegisterEmployeeAsync(RegisterEmployeeDto employeeDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // First, insert the address
                var direccionId = await InsertDireccionAsync(connection, transaction, employeeDto);
                
                // Then, insert the person
                var personaId = await InsertPersonaAsync(connection, transaction, employeeDto, direccionId);
                
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

        private async Task<int> InsertDireccionAsync(SqlConnection connection, SqlTransaction transaction, RegisterEmployeeDto employeeDto)
        {
            var query = @"
                INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
                OUTPUT INSERTED.id
                VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular)";

            var parameters = new
            {
                Provincia = employeeDto.Provincia,
                Canton = employeeDto.Canton,
                Distrito = employeeDto.Distrito,
                DireccionParticular = employeeDto.DireccionParticular
            };

            return await connection.QuerySingleAsync<int>(query, parameters, transaction);
        }

        private async Task<int> InsertPersonaAsync(SqlConnection connection, SqlTransaction transaction, RegisterEmployeeDto employeeDto, int direccionId)
        {
            var query = @"
                INSERT INTO PlaniFy.Persona (Correo, Nombre, SegundoNombre, Apellidos, FechaNacimiento, Cedula, Rol, Telefono, idDireccion)
                OUTPUT INSERTED.Id
                VALUES (@Correo, @Nombre, @SegundoNombre, @Apellidos, @FechaNacimiento, @Cedula, @Rol, @Telefono, @IdDireccion)";

            var parameters = new
            {
                Correo = employeeDto.Correo,
                Nombre = employeeDto.PrimerNombre,
                SegundoNombre = employeeDto.SegundoNombre,
                Apellidos = employeeDto.PrimerApellido,
                FechaNacimiento = employeeDto.FechaNacimiento,
                Cedula = employeeDto.Cedula,
                Rol = "Empleado", // Default role for employees
                Telefono = employeeDto.Telefono,
                IdDireccion = direccionId
            };

            return await connection.QuerySingleAsync<int>(query, parameters, transaction);
        }

        private async Task InsertEmpleadoAsync(SqlConnection connection, SqlTransaction transaction, RegisterEmployeeDto employeeDto, int personaId)
        {
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
                IdEmpresa = employeeDto.IdEmpresa
            };

            await connection.ExecuteAsync(query, parameters, transaction);
        }

        public async Task<bool> ValidateCedulaExistsAsync(string cedula)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = "SELECT COUNT(1) FROM PlaniFy.Persona WHERE Cedula = @Cedula";
            var count = await connection.QuerySingleAsync<int>(query, new { Cedula = cedula });
            
            return count > 0;
        }

        public async Task<bool> ValidateEmailExistsAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = "SELECT COUNT(1) FROM PlaniFy.Persona WHERE Correo = @Correo";
            var count = await connection.QuerySingleAsync<int>(query, new { Correo = email });
            
            return count > 0;
        }
    }
}
