using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using backend.DTOs;
using backend.Models;
using Dapper;

namespace backend.Repositories
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly string _connectionString;
        private readonly IDirectionRepository _direccionRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EmployerRepository(IConfiguration configuration, IDirectionRepository direccionRepository, IPersonaRepository personaRepository, IUsuarioRepository usuarioRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
            _direccionRepository = direccionRepository;
            _personaRepository = personaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular)
        {
            return await _direccionRepository.CreateDireccionAsync(provincia, canton, distrito, direccionParticular);
        }

        public async Task<int> CreatePersonaAsync(Persona persona)
        {
            var personaId = await _personaRepository.CreatePersonaAsync(persona);
            return personaId;
        }

        public async Task<bool> CreateUserAsync(Usuario usuario)
        {
            return await _usuarioRepository.CreateUserAsync(usuario);
        }

        public async Task<EmployerResponseDto?> GetEmployerByIdAsync(int personaId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        p.Id as IdPersona,
                        p.Correo,
                        p.Nombre,
                        p.SegundoNombre,
                        p.Apellidos,
                        p.FechaNacimiento,
                        p.Cedula,
                        p.Rol,
                        p.Telefono,
                        p.idDireccion as IdDireccion,
                        u.TipoUsuario,
                        d.Provincia,
                        d.Canton,
                        d.Distrito,
                        d.DireccionParticular
                    FROM PlaniFy.Persona p
                    INNER JOIN PlaniFy.Usuario u ON p.Id = u.idPersona
                    LEFT JOIN PlaniFy.Direccion d ON p.idDireccion = d.Id
                    WHERE p.Id = @IdPersona AND u.TipoUsuario = 'Empleador'";

                return await connection.QueryFirstOrDefaultAsync<EmployerResponseDto>(query, new { IdPersona = personaId });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Persona?> GetByCedulaAsync(string cedula)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT Id, Correo, Nombre, SegundoNombre, Apellidos, FechaNacimiento,
                        Cedula, Rol, Telefono, idDireccion AS IdDireccion
                    FROM PlaniFy.Persona
                    WHERE Cedula = @Cedula";

                return await connection.QueryFirstOrDefaultAsync<Persona>(sql, new { Cedula = cedula });
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await _personaRepository.IsEmailAvailableAsync(email);
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            return await _personaRepository.IsCedulaAvailableAsync(cedula);
        }

        // GET KPI DATA (Key Performance Indicators)
        public async Task<KPIResponseDTO?> GetKPIDataAsync(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT
                        (SELECT COUNT(*) 
                        FROM PlaniFy.Empresa e 
                        WHERE e.idEmpleador = @UserId AND e.Estado = 'Activo') AS TotalCompanies,

                        (SELECT COUNT(DISTINCT em.idPersona)
                        FROM PlaniFy.Empresa e
                        JOIN PlaniFy.Empleado em ON em.idEmpresa = e.Id
                        WHERE e.idEmpleador = @UserId AND em.Estado = 'Activo'
                        ) AS TotalActiveEmployees,

                        (SELECT COALESCE(SUM(dp.salarioBruto), 0)
                        FROM PlaniFy.Empresa e
                        JOIN PlaniFy.Planilla p ON p.idEmpresa = e.Id
                        JOIN PlaniFy.DetallePlanilla dp ON dp.idPlanilla = p.Id
                        WHERE e.idEmpleador = @UserId
                        AND MONTH(p.FechaGeneracion) = MONTH(GETDATE())
                        AND YEAR(p.FechaGeneracion) = YEAR(GETDATE())
                        ) AS TotalPayroll,

                        (SELECT COUNT(DISTINCT e.Id)
                        FROM PlaniFy.Empresa e
                        JOIN PlaniFy.Planilla p ON p.idEmpresa = e.Id
                        WHERE e.idEmpleador = @UserId
                        AND MONTH(p.FechaGeneracion) = MONTH(GETDATE())
                        AND YEAR(p.FechaGeneracion) = YEAR(GETDATE())
                        ) AS CompaniesWithPayroll
                ";

                return await connection.QueryFirstOrDefaultAsync<KPIResponseDTO>(query, new { UserId = userId });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
