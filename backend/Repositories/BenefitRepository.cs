using backend.DTOs;
using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace backend.Repositories
{
    public class BenefitRepository : IBenefitRepository
    {
        private readonly string _connectionString;

        public BenefitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        #region CRUD Operations
        
        public async Task<Benefit> CreateAsync(Benefit benefit)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO PlaniFy.Beneficio (idEmpresa, Nombre, TipoCalculo, Tipo)
                VALUES (@CompanyId, @Name, @CalculationType, @Type)";

            var parameters = new
            {
                CompanyId = benefit.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type
            };

            await connection.ExecuteAsync(query, parameters);
            return benefit;
        }

        public async Task<Benefit?> GetByIdAsync(int companyId, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type
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
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type
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
                SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type
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

        public async Task<bool> UpdateAsync(int companyId, string name, UpdateBenefitDto updateDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE PlaniFy.Beneficio
                SET Nombre = @NewName, TipoCalculo = @CalculationType, Tipo = @Type
                WHERE idEmpresa = @CompanyId AND Nombre = @OldName";

            var parameters = new
            {
                CompanyId = companyId,
                OldName = name,
                NewName = updateDto.Name,
                CalculationType = updateDto.CalculationType,
                Type = updateDto.Type
            };

            var rowsAffected = await connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int companyId, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                DELETE FROM PlaniFy.Beneficio
                WHERE idEmpresa = @CompanyId AND Nombre = @Name";

            var parameters = new
            {
                CompanyId = companyId,
                Name = name
            };

            var rowsAffected = await connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        #endregion

        #region Validation Methods

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

        #endregion

        #region Specific Queries

        public async Task<List<BenefitResponseDto>> GetBenefitsWithCompanyNameAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT b.idEmpresa as CompanyId, b.Nombre as Name, b.TipoCalculo as CalculationType, b.Tipo as Type, e.Nombre as CompanyName
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

        public async Task<int> CountByCompanyIdAsync(int companyId)
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
            return count;
        }

        #endregion
    }
}
