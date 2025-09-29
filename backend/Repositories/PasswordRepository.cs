using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace backend.Repositories
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly string _connectionString;

        public PasswordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CountryContext") ?? throw new ArgumentNullException("Connection string not found");
        }


        public async Task<bool> UpdateEmployeePasswordAsync(int personaId, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    UPDATE PlaniFy.Empleado SET Contrasena = @contrasena WHERE idPersona = @idPersona";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", personaId);
                command.Parameters.AddWithValue("@contrasena", password);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
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
