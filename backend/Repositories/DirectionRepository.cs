using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public class DirectionRepository : IDirectionRepository
    {
        private readonly string _connectionString;

        public DirectionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        // TODO: Hacer SOLID  
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

        public async Task<DirectionDTO?> GetDirectionByIdAsync(int id)
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

        public async Task<bool> UpdateDireccionAsync(int id, DirectionDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE PlaniFy.Direccion
                SET Provincia = @Provincia,
                    Canton = @Canton,
                    Distrito = @Distrito,
                    DireccionParticular = @DireccionParticular
                WHERE Id = @Id";

            var affected = await connection.ExecuteAsync(query, new {
                Id = id,
                dto.Provincia,
                dto.Canton,
                dto.Distrito,
                dto.DireccionParticular
            });

            return affected > 0;
        }
    }
}