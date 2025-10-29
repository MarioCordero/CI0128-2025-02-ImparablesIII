using backend.DTOs;
using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace backend.Repositories
{
    public class BenefitRepository : IBenefitRepository
    {
        private readonly string _connectionString;

        public BenefitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        
        public async Task<Benefit> CreateAsync(Benefit benefit)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO PlaniFy.Beneficio (idEmpresa, Nombre, TipoCalculo, Tipo, Valor, Porcentaje, Descripcion)
                VALUES (@CompanyId, @Name, @CalculationType, @Type, @Value, @Percentage, @Descripcion)";

            var parameters = new
            {
                CompanyId = benefit.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type,
                Value = benefit.Value,
                Percentage = benefit.Percentage,
                Descripcion = benefit.Descripcion
            };

            await connection.ExecuteAsync(query, parameters);
            return benefit;
        }

        public async Task<Benefit?> GetByIdAsync(int companyId, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor as Value, Porcentaje as Percentage, Descripcion
                FROM PlaniFy.Beneficio
                WHERE idEmpresa = @CompanyId AND Nombre = @Name";

            var parameters = new
            {
                CompanyId = companyId,
                Name = name
            };

            var result = await connection.QueryFirstOrDefaultAsync<Benefit>(query, parameters);
            return result;
        }

        public async Task<List<Benefit>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor, Porcentaje, Descripcion
                FROM PlaniFy.Beneficio
                ORDER BY idEmpresa, Nombre";

            var result = await connection.QueryAsync<Benefit>(query);
            return result.ToList();
        }

        public async Task<List<Benefit>> GetByCompanyIdAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor as Value, Porcentaje as Percentage, Descripcion
                FROM PlaniFy.Beneficio
                WHERE idEmpresa = @CompanyId
                ORDER BY Nombre";

            var parameters = new
            {
                CompanyId = companyId
            };

            var result = await connection.QueryAsync<Benefit>(query, parameters);
            return result.ToList();
        }

        public async Task<bool> ExistsAsync(int companyId, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(1)
                FROM PlaniFy.Beneficio
                WHERE idEmpresa = @CompanyId AND Nombre = @Name";

            var parameters = new
            {
                CompanyId = companyId,
                Name = name
            };

            var count = await connection.QuerySingleAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<bool> ExistsByCompanyIdAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(1)
                FROM PlaniFy.Beneficio
                WHERE idEmpresa = @CompanyId";

            var parameters = new
            {
                CompanyId = companyId
            };

            var count = await connection.QuerySingleAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<List<BenefitResponseDto>> GetBenefitsWithCompanyNameAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT b.idEmpresa as CompanyId, b.Nombre as Name, b.TipoCalculo as CalculationType, b.Tipo as Type, b.Valor as Value, b.Porcentaje as Percentage, b.Descripcion as Descripcion, e.Nombre as CompanyName
                FROM PlaniFy.Beneficio b
                INNER JOIN PlaniFy.Empresa e ON b.idEmpresa = e.Id
                WHERE b.idEmpresa = @CompanyId
                ORDER BY b.Nombre";

            var parameters = new
            {
                CompanyId = companyId
            };

            var result = await connection.QueryAsync<BenefitResponseDto>(query, parameters);
            return result.ToList();
        }

        public async Task<bool> UpdateAsync(int companyId, string name, UpdateBenefitRequestDto updateDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                Console.WriteLine($"=== EXECUTING STORED PROCEDURE ===");
                Console.WriteLine($"CompanyId: {companyId}, Original: '{name}', New: '{updateDto.Name.Trim()}'");

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyId", companyId);
                parameters.Add("@OriginalName", name);
                parameters.Add("@NewName", updateDto.Name.Trim());
                parameters.Add("@Descripcion", updateDto.Descripcion);

                // Ejecutar el stored procedure
                await connection.ExecuteAsync(
                    "PlaniFy.SP_ActualizarBeneficio",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                Console.WriteLine($"Stored procedure executed successfully");
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error in stored procedure: {ex.Message}");
                
                // Manejar errores específicos del SP
                if (ex.Message.Contains("Beneficio no encontrado"))
                    throw new ArgumentException("Beneficio no encontrado");
                else if (ex.Message.Contains("Ya existe un beneficio"))
                    throw new ArgumentException("Ya existe un beneficio con este nombre para esta empresa");
                else
                    throw new Exception($"Error al actualizar beneficio: {ex.Message}");
            }
        }
    }
}
