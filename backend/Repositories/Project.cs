using Dapper;
using Microsoft.Data.SqlClient;
using backend.Models;
using backend.DTOs;

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
                INSERT INTO PlaniFy.Empresa (Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion)
                VALUES (@Nombre, @CedulaJuridica, @Email, @PeriodoPago, @Telefono, @IdDireccion);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(sql, project);
            project.Id = id;
            return project;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM PlaniFy.Empresa WHERE Id = @Id";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Project>(sql, new { Id = id });
        }

        public async Task<Project?> GetByNameAsync(string nombre)
        {
            const string sql = "SELECT * FROM PlaniFy.Empresa WHERE Nombre = @Nombre";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Project>(sql, new { Nombre = nombre });
        }

        public async Task<Project?> GetByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM PlaniFy.Empresa WHERE Email = @Email";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Project>(sql, new { Email = email });
        }

        public async Task<List<Project>> GetAllAsync()
        {
            const string sql = "SELECT * FROM PlaniFy.Empresa ORDER BY Nombre";
            
            using var connection = new SqlConnection(_connectionString);
            var projects = await connection.QueryAsync<Project>(sql);
            return projects.ToList();
        }

        public async Task<bool> ExistsByNameAsync(string nombre)
        {
            const string sql = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Nombre = @Nombre";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { Nombre = nombre });
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            const string sql = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Email = @Email";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<bool> ExistsByCedulaJuridicaAsync(int cedulaJuridica)
        {
            const string sql = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE CedulaJuridica = @CedulaJuridica";
            
            using var connection = new SqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(sql, new { CedulaJuridica = cedulaJuridica });
            return count > 0;
        }

        public async Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular)
        {
            const string sql = @"
                INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
                VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sql, new 
            { 
                Provincia = provincia, 
                Canton = canton, 
                Distrito = distrito, 
                DireccionParticular = direccionParticular 
            });
        }

        public async Task<DireccionDto?> GetDireccionByIdAsync(int id)
        {
            const string sql = "SELECT id as Id, Provincia, Canton, Distrito, DireccionParticular FROM PlaniFy.Direccion WHERE id = @Id";
            
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<DireccionDto>(sql, new { Id = id });
        }

        public async Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id)
        {
            const string sql = @"
                SELECT 
                    e.Id, e.Nombre, e.CedulaJuridica, e.Email, e.PeriodoPago, 
                    e.Telefono, e.idDireccion as IdDireccion,
                    d.id as Id, d.Provincia, d.Canton, d.Distrito, d.DireccionParticular
                FROM PlaniFy.Empresa e
                LEFT JOIN PlaniFy.Direccion d ON e.idDireccion = d.id
                WHERE e.Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<ProjectResponseDto, DireccionDto, ProjectResponseDto>(
                sql,
                (project, direccion) =>
                {
                    project.Direccion = direccion;
                    return project;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return result.FirstOrDefault();
        }

        public async Task<List<Project>> GetByEmployerIdAsync(int employerId)
        {
            // Como no hay relación con empleador en tu esquema, devuelve todas las empresas
            return await GetAllAsync();
        }

        public async Task<bool> ExistsByLegalIdAsync(string legalId)
        {
            // Convierte string a int para coincidir con tu esquema
            if (int.TryParse(legalId, out int cedulaJuridica))
            {
                return await ExistsByCedulaJuridicaAsync(cedulaJuridica);
            }
            return false;
        }

        public async Task<List<Project>> GetByCompanyIdAsync(int companyId)
        {
            // No aplica en tu esquema actual
            var project = await GetByIdAsync(companyId);
            return project != null ? new List<Project> { project } : new List<Project>();
        }
    }
}