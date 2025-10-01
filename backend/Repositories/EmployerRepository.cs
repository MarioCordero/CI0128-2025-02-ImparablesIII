using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using backend.DTOs;
using Dapper;

namespace backend.Repositories
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly string _connectionString;
        private readonly IDireccionRepository _direccionRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EmployerRepository(IConfiguration configuration, IDireccionRepository direccionRepository, IPersonaRepository personaRepository, IUsuarioRepository usuarioRepository)
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

        public async Task<int> CreatePersonaAsync(string email, string nombre, string apellidos, DateTime fechaNacimiento, string cedula, string rol, int? telefono, int direccionId)
        {
            return await _personaRepository.CreatePersonaAsync(email, nombre, null, apellidos, fechaNacimiento, cedula, rol, telefono, direccionId);
        }

        public async Task<bool> CreateUsuarioAsync(int personaId, string password, string tipoUsuario)
        {
            return await _usuarioRepository.CreateUserAsync(personaId, password, tipoUsuario);
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

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await _personaRepository.IsEmailAvailableAsync(email);
        }

        public async Task<bool> IsCedulaAvailableAsync(string cedula)
        {
            return await _personaRepository.IsCedulaAvailableAsync(cedula);
        }
    }
}
