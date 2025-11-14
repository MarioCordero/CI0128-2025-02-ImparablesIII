using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public class DireccionRepository : IDireccionRepository
    {
        private readonly string _connectionString;

        public DireccionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
                    OUTPUT INSERTED.id
                    VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular)";

                var parameters = new
                {
                    Provincia = provincia,
                    Canton = canton,
                    Distrito = distrito,
                    DireccionParticular = direccionParticular
                };

                return await connection.QuerySingleAsync<int>(query, parameters);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<DirectionDTO?> GetDireccionByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT id, Provincia, Canton, Distrito, DireccionParticular FROM PlaniFy.Direccion WHERE id = @Id";

                var result = await connection.QueryFirstOrDefaultAsync<DirectionDTO>(query, new { Id = id });
                return result;
            }
            catch (Exception)
            {
                return null;
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
        public async Task<bool> UpdateDireccionAsync(Direccion direccion)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    UPDATE PlaniFy.Direccion
                    SET Provincia = @Provincia,
                        Canton = @Canton,
                        Distrito = @Distrito,
                        DireccionParticular = @DireccionParticular
                    WHERE id = @Id";

                var parameters = new
                {
                    Provincia = direccion.Provincia,
                    Canton = direccion.Canton,
                    Distrito = direccion.Distrito,
                    DireccionParticular = direccion.DireccionParticular,
                    Id = direccion.Id
                };

                var affected = await connection.ExecuteAsync(query, parameters);
                return affected > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}