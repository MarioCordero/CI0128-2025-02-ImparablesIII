using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using backend.Models;
using Dapper;

namespace backend.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly string _connectionString;

        public PersonaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<int> CreatePersonaAsync(Persona persona)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.Persona (Correo, Nombre, SegundoNombre, Apellidos, FechaNacimiento, Cedula, Rol, Telefono, idDireccion)
                    OUTPUT INSERTED.Id
                    VALUES (@Correo, @Nombre, @SegundoNombre, @Apellidos, @FechaNacimiento, @Cedula, @Rol, @Telefono, @IdDireccion)";

                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    Correo = persona.Correo,
                    Nombre = persona.Nombre,
                    SegundoNombre = persona.SegundoNombre,
                    Apellidos = persona.Apellidos,
                    FechaNacimiento = persona.FechaNacimiento,
                    Cedula = persona.Cedula,
                    Rol = persona.Rol,
                    Telefono = persona.Telefono,
                    IdDireccion = persona.IdDireccion
                });

                if (id == -1)
                {
                    throw new InvalidOperationException("Failed to create person");
                }
                return id;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<Persona?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        Id,
                        Correo,
                        Nombre,
                        SegundoNombre,
                        Apellidos,
                        FechaNacimiento,
                        Cedula,
                        Rol,
                        Telefono,
                        idDireccion AS IdDireccion
                    FROM PlaniFy.Persona
                    WHERE Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<Persona>(query, new { Id = id });
            }
            catch
            {
                return null;
            }
        }

        public async Task<Persona?> GetByEmailAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT Id, Correo, Nombre, SegundoNombre, Apellidos, FechaNacimiento, Cedula, Rol, Telefono, idDireccion AS IdDireccion
                    FROM PlaniFy.Persona
                    WHERE Correo = @Correo";

                return await connection.QueryFirstOrDefaultAsync<Persona>(sql, new { Correo = email });
            }
            catch
            {
                return null;
            }
        }


        public async Task<Persona?> GetByCedulaAsync(string cedula)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT p.Id, p.Correo, p.Nombre, p.SegundoNombre, p.Apellidos, p.FechaNacimiento,
                        p.Cedula, p.Rol, p.Telefono, p.idDireccion AS IdDireccion
                    FROM PlaniFy.Persona p
                    WHERE p.Cedula = @Cedula";

                return await connection.QueryFirstOrDefaultAsync<Persona>(sql, new { Cedula = cedula });
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(1) FROM PlaniFy.Persona WHERE Correo = @Email";
                var count = await connection.QuerySingleAsync<int>(query, new { Email = email });
                
                return count == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(1) FROM PlaniFy.Persona WHERE Cedula = @Cedula";
                var count = await connection.QuerySingleAsync<int>(query, new { Cedula = cedula });
                
                return count == 0;
            }
            catch (Exception)
            {
                return false;
            }
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
    }
}
