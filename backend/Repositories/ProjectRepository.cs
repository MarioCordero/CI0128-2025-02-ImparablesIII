using backend.DTOs;
using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace backend.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;
        private readonly IDireccionRepository _direccionRepository;

        public ProjectRepository(IConfiguration configuration, IDireccionRepository direccionRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            _direccionRepository = direccionRepository;
        }

        #region CRUD Operations
        
        public async Task<Project> CreateAsync(Project project)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO PlaniFy.Empresa (Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion, MaximoBeneficios)
                OUTPUT INSERTED.Id
                VALUES (@Nombre, @CedulaJuridica, @Email, @PeriodoPago, @Telefono, @IdDireccion, @MaximoBeneficios)";

            var parameters = new
            {
                Nombre = project.Nombre,
                CedulaJuridica = project.CedulaJuridica,
                Email = project.Email,
                PeriodoPago = project.PeriodoPago,
                Telefono = project.Telefono,
                IdDireccion = project.IdDireccion,
                MaximoBeneficios = project.MaximoBeneficios
            };

            var id = await connection.QuerySingleAsync<int>(query, parameters);
            project.Id = id;
            return project;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion
                FROM PlaniFy.Empresa
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Project>(query, new { Id = id });
        }

        public async Task<Project?> GetByNameAsync(string nombre)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion
                FROM PlaniFy.Empresa
                WHERE Nombre = @Nombre";

            return await connection.QueryFirstOrDefaultAsync<Project>(query, new { Nombre = nombre });
        }

        public async Task<Project?> GetByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion
                FROM PlaniFy.Empresa
                WHERE Email = @Email";

            return await connection.QueryFirstOrDefaultAsync<Project>(query, new { Email = email });
        }

        public async Task<List<Project>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion
                FROM PlaniFy.Empresa";

            var projects = await connection.QueryAsync<Project>(query);
            return projects.ToList();
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectDto updateDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE PlaniFy.Empresa 
                SET Nombre = @Nombre, Email = @Email, PeriodoPago = @PeriodoPago, Telefono = @Telefono
                WHERE Id = @Id";

            var parameters = new
            {
                Id = id,
                Nombre = updateDto.Nombre,
                Email = updateDto.Email,
                PeriodoPago = updateDto.PeriodoPago,
                Telefono = updateDto.Telefono
            };

            var rowsAffected = await connection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM PlaniFy.Empresa WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        #endregion

        #region Existence Checks

        public async Task<bool> ExistsByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(query, new { Id = id });
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string nombre)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Nombre = @Nombre";
            var count = await connection.QuerySingleAsync<int>(query, new { Nombre = nombre });
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Email = @Email";
            var count = await connection.QuerySingleAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<bool> ExistsByCedulaJuridicaAsync(int cedulaJuridica)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE CedulaJuridica = @CedulaJuridica";
            var count = await connection.QuerySingleAsync<int>(query, new { CedulaJuridica = cedulaJuridica });
            return count > 0;
        }

        public async Task<bool> ExistsByLegalIdAsync(string legalId)
        {
            if (int.TryParse(legalId, out int cedulaJuridica))
            {
                return await ExistsByCedulaJuridicaAsync(cedulaJuridica);
            }
            return false;
        }

        #endregion

        #region Specific Queries

        public async Task<List<Project>> GetByEmployerIdAsync(int employerId)
        {
            // TODO: Implementar relación employer-project según esquema BD
            return await GetAllAsync();
        }

        public async Task<List<Project>> GetByCompanyIdAsync(int companyId)
        {
            var project = await GetByIdAsync(companyId);
            return project != null ? new List<Project> { project } : new List<Project>();
        }

        public async Task<ProjectResponseDto?> GetProjectWithDireccionAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion
                FROM PlaniFy.Empresa
                WHERE Id = @Id";

            var project = await connection.QueryFirstOrDefaultAsync<ProjectResponseDto>(query, new { Id = id });

            if (project != null)
            {
                project.Direccion = await _direccionRepository.GetDireccionByIdAsync(project.IdDireccion);
                project.CreatedAt = DateTime.UtcNow; // Valor por defecto
            }

            return project;
        }

        public async Task<List<CompanyDashboardMainEmployerDto>> GetProjectsForDashboardAsync(int employerId)
        {
            var projects = await GetAllAsync();

            return projects.Select(p => new CompanyDashboardMainEmployerDto
            {
                Id = p.Id,
                Name = p.Nombre,
                LegalId = p.CedulaJuridica.ToString(),
                ActiveEmployees = 0, // TODO: Implementar
                PayPeriod = p.PeriodoPago,
                MonthlyPayroll = 0, // TODO: Implementar
                CurrentProfitability = 0, // TODO: Implementar
                LastMonthProfitability = 0, // TODO: Implementar
                Notifications = new List<NotificationDto>()
            }).ToList();
        }

        #endregion

        #region Business Operations

        public async Task<int> CountActiveEmployeesAsync(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // TODO: Ajustar según esquema real de empleados
            var query = "SELECT COUNT(*) FROM PlaniFy.Empleado WHERE EmpresaId = @ProjectId AND Activo = 1";
            var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { ProjectId = projectId });
            return count;
        }

        public async Task<decimal> GetMonthlyPayrollAsync(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // TODO: Ajustar según esquema real de empleados
            var query = "SELECT ISNULL(SUM(Salario), 0) FROM PlaniFy.Empleado WHERE EmpresaId = @ProjectId AND Activo = 1";
            var payroll = await connection.QuerySingleOrDefaultAsync<decimal>(query, new { ProjectId = projectId });
            return payroll;
        }

        public async Task<bool> ActivateAsync(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // TODO: Verificar si existe columna Activo en tabla
            var query = "UPDATE PlaniFy.Empresa SET Activo = 1 WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = projectId });
            return rowsAffected > 0;
        }

        public async Task<bool> DeactivateAsync(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // TODO: Verificar si existe columna Activo en tabla
            var query = "UPDATE PlaniFy.Empresa SET Activo = 0 WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = projectId });
            return rowsAffected > 0;
        }

        #endregion

        #region Address Operations (Delegated)

        public async Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular)
        {
            return await _direccionRepository.CreateDireccionAsync(provincia, canton ?? string.Empty, distrito ?? string.Empty, direccionParticular);
        }

        public async Task<DireccionDto?> GetDireccionByIdAsync(int id)
        {
            return await _direccionRepository.GetDireccionByIdAsync(id);
        }

        #endregion
    }
}