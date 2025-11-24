using backend.DTOs;
using backend.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace backend.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;
        private readonly IDirectionRepository _direccionRepository;

        public ProjectRepository(IConfiguration configuration, IDirectionRepository direccionRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            _direccionRepository = direccionRepository;
        }

        public async Task<ProjectResponseDTO> CreateAsync(Project project)
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

            return new ProjectResponseDTO
            {
                Id = project.Id,
                Nombre = project.Nombre,
                CedulaJuridica = project.CedulaJuridica,
                Email = project.Email,
                PeriodoPago = project.PeriodoPago,
                Telefono = project.Telefono,
                MaximoBeneficios = project.MaximoBeneficios,
                IdDireccion = project.IdDireccion
            };
        }

        public async Task<ProjectResponseDTO?> GetProjectWithDireccionAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    p.Id,
                    p.Nombre,
                    p.CedulaJuridica,
                    p.Email,
                    p.PeriodoPago,
                    p.Telefono,
                    p.IdDireccion,
                    p.MaximoBeneficios,
                    d.Id AS DireccionId,
                    d.Provincia,
                    d.Canton,
                    d.Distrito,
                    d.DireccionParticular
                FROM PlaniFy.Empresa p
                LEFT JOIN PlaniFy.Direccion d ON p.IdDireccion = d.Id
                WHERE p.Id = @CompanyId";

            var result = await connection.QueryAsync(query, new { CompanyId = companyId });
            var row = result.FirstOrDefault();

            if (row != null)
            {
                var project = new ProjectResponseDTO
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    CedulaJuridica = row.CedulaJuridica,
                    Email = row.Email,
                    PeriodoPago = row.PeriodoPago,
                    Telefono = row.Telefono,
                    IdDireccion = row.IdDireccion,
                    MaximoBeneficios = row.MaximoBeneficios,
                    Direccion = row.DireccionId != null ? new DirectionDTO
                    {
                        Id = row.DireccionId,
                        Provincia = row.Provincia,
                        Canton = row.Canton,
                        Distrito = row.Distrito,
                        DireccionParticular = row.DireccionParticular
                    } : null
                };
                return project;
            }
            return null;
        }

        public async Task<ProjectResponseDTO?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion, MaximoBeneficios
                FROM PlaniFy.Empresa
                WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync(query, new { Id = id });
            
            if (result != null)
            {
                return new ProjectResponseDTO
                {
                    Id = result.Id,
                    Nombre = result.Nombre,
                    CedulaJuridica = result.CedulaJuridica,
                    Email = result.Email,
                    PeriodoPago = result.PeriodoPago,
                    Telefono = result.Telefono,
                    IdDireccion = result.idDireccion,
                    MaximoBeneficios = result.MaximoBeneficios,
                    CreatedAt = DateTime.Now
                };
            }
            return null;
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

        public async Task<List<ProjectResponseDTO>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT Id, Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion, MaximoBeneficios
                FROM PlaniFy.Empresa";

            var results = await connection.QueryAsync(query);
            return results.Select(r => new ProjectResponseDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CedulaJuridica = r.CedulaJuridica,
                Email = r.Email,
                PeriodoPago = r.PeriodoPago,
                Telefono = r.Telefono,
                IdDireccion = r.idDireccion,
                MaximoBeneficios = r.MaximoBeneficios,
                CreatedAt = DateTime.Now
            }).ToList();
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjectDTO dto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Update company information
                var updateCompanyQuery = @"
                    UPDATE PlaniFy.Empresa 
                    SET 
                        Nombre = @Nombre,
                        Email = @Email,
                        PeriodoPago = @PeriodoPago,
                        Telefono = @Telefono,
                        MaximoBeneficios = @MaximoBeneficios
                    WHERE Id = @Id";

                var companyParameters = new
                {
                    Id = id,
                    Nombre = dto.Nombre,
                    Email = dto.Email,
                    PeriodoPago = dto.PeriodoPago,
                    Telefono = dto.Telefono,
                    MaximoBeneficios = dto.MaximoBeneficios
                };

                var rowsAffected = await connection.ExecuteAsync(updateCompanyQuery, companyParameters, transaction);

                // If address information is provided, update it as well
                if (dto.Direccion != null)
                {
                    // Get the current address ID
                    var getAddressIdQuery = "SELECT idDireccion FROM PlaniFy.Empresa WHERE Id = @Id";
                    var addressId = await connection.QuerySingleOrDefaultAsync<int?>(getAddressIdQuery, new { Id = id }, transaction);

                    if (addressId.HasValue)
                    {
                        var updateAddressQuery = @"
                            UPDATE PlaniFy.Direccion 
                            SET 
                                Provincia = @Provincia,
                                Canton = @Canton,
                                Distrito = @Distrito,
                                DireccionParticular = @DireccionParticular
                            WHERE Id = @Id";

                        var addressParameters = new
                        {
                            Id = addressId.Value,
                            Provincia = dto.Direccion.Provincia,
                            Canton = dto.Direccion.Canton,
                            Distrito = dto.Direccion.Distrito,
                            DireccionParticular = dto.Direccion.DireccionParticular
                        };

                        await connection.ExecuteAsync(updateAddressQuery, addressParameters, transaction);
                    }
                }

                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(query, new { Id = id });
            return count > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM PlaniFy.Empresa WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
        
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

        public async Task<bool> ExistsByLegalIdAsync(string legalId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM PlaniFy.Empresa WHERE CedulaJuridica = @CedulaJuridica";
            var count = await connection.QuerySingleAsync<int>(query, new { CedulaJuridica = legalId });
            return count > 0;
        }

        public async Task<List<ProjectResponseDTO>> GetByEmployerIdAsync(int employerId)
        {
            // TODO: Implementar relación employer-project según esquema BD
            return await GetAllAsync();
        }

        public async Task<List<ProjectResponseDTO>> GetByCompanyIdAsync(int companyId)
        {
            var project = await GetByIdAsync(companyId);
            return project != null ? new List<ProjectResponseDTO> { project } : new List<ProjectResponseDTO>();
        }

        public async Task<List<ProjectResponseDTO>> GetProjectsForDashboardAsync(int employerId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    e.Id,
                    e.Nombre,
                    e.CedulaJuridica,
                    e.Email,
                    e.PeriodoPago,
                    e.Telefono,
                    e.MaximoBeneficios,
                    e.idEmpleador
                FROM PlaniFy.Empresa AS e
                WHERE e.idEmpleador = @EmployerId";

            var results = await connection.QueryAsync(query, new { EmployerId = employerId });
            
            return results.Select(r => new ProjectResponseDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                CedulaJuridica = r.CedulaJuridica,
                Email = r.Email,
                PeriodoPago = r.PeriodoPago,
                Telefono = r.Telefono,
                MaximoBeneficios = r.MaximoBeneficios,
                ActiveEmployees = 0, // TODO: Implement
                MonthlyPayroll = 0, // TODO: Implement
            }).ToList();
        }

        public async Task<bool> UpdateDireccionAsync(int id, DirectionDTO direccion)
        {
            return await _direccionRepository.UpdateDireccionAsync(id, direccion);
        }

        public async Task<int> CountActiveEmployeesAsync(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = "SELECT COUNT(*) FROM PlaniFy.Empleado WHERE idEmpresa = @ProjectId";
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

        public async Task<int> CreateDireccionAsync(string provincia, string? canton, string? distrito, string? direccionParticular)
        {
            return await _direccionRepository.CreateDireccionAsync(provincia, canton ?? string.Empty, distrito ?? string.Empty, direccionParticular);
        }

        public async Task<DirectionDTO?> GetDireccionByIdAsync(int id)
        {
            return await _direccionRepository.GetDireccionByIdAsync(id);
        }
    }
}