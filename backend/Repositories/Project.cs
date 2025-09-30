using Dapper;
using Microsoft.Data.SqlClient;
using backend.Models;
// TODO REFRACT
namespace backend.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProjectRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<Project> CreateAsync(Project project)
        {
            const string sql = @"
                INSERT INTO Companies (CompanyName, LegalId, Email, Address, Phone, MaxBenefits, PaymentPeriod, EmployerId, CreatedAt, UpdatedAt)
                VALUES (@CompanyName, @LegalId, @Email, @Address, @Phone, @MaxBenefits, @PaymentPeriod, @EmployerId, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(sql, company);
            company.Id = id;
            return company;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Companies WHERE Id = @Id";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { Id = id });
        }

        public async Task<Company?> GetByLegalIdAsync(string legalId)
        {
            const string sql = "SELECT * FROM Companies WHERE LegalId = @LegalId";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { LegalId = legalId });
        }

        public async Task<Company?> GetByNameAsync(string companyName)
        {
            const string sql = "SELECT * FROM Companies WHERE CompanyName = @CompanyName";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { CompanyName = companyName });
        }

        public async Task<Company?> GetByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM Companies WHERE Email = @Email";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { Email = email });
        }

        public async Task<List<Company>> GetByEmployerIdAsync(int employerId)
        {
            const string sql = "SELECT * FROM Companies WHERE EmployerId = @EmployerId ORDER BY CreatedAt DESC";
            
            using var connection = new SqlConnection(_connectionString);
            var companies = await connection.QueryAsync<Company>(sql, new { EmployerId = employerId });
            return companies.ToList();
        }

        public async Task<bool> ExistsByLegalIdAsync(string legalId)
        {
            const string sql = "SELECT COUNT(1) FROM Companies WHERE LegalId = @LegalId";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { LegalId = legalId });
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string companyName)
        {
            const string sql = "SELECT COUNT(1) FROM Companies WHERE CompanyName = @CompanyName";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { CompanyName = companyName });
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            const string sql = "SELECT COUNT(1) FROM Companies WHERE Email = @Email";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });
            return count > 0;
        }
    }
}