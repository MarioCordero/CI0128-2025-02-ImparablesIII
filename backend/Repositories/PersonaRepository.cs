using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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

        public async Task<int> CreatePersonaAsync(string email, string nombre, string? segundoNombre, string apellidos, DateTime fechaNacimiento, string cedula, string rol, int? telefono, int direccionId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.Persona (Correo, Nombre, SegundoNombre, Apellidos, FechaNacimiento, Cedula, Rol, Telefono, idDireccion)
                    OUTPUT INSERTED.Id
                    VALUES (@Correo, @Nombre, @SegundoNombre, @Apellidos, @FechaNacimiento, @Cedula, @Rol, @Telefono, @IdDireccion)";

                var parameters = new
                {
                    Correo = email,
                    Nombre = nombre,
                    SegundoNombre = segundoNombre,
                    Apellidos = apellidos,
                    FechaNacimiento = fechaNacimiento,
                    Cedula = cedula,
                    Rol = rol,
                    Telefono = telefono,
                    IdDireccion = direccionId
                };

                return await connection.QuerySingleAsync<int>(query, parameters);
            }
            catch (Exception)
            {
                return -1;
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
