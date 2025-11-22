using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using backend.Models;
using backend.DTOs;
using backend.Constants;
using Dapper;

namespace backend.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<UserDataDto?> GetUserDataAsync(int idPersona)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var query = @"
                SELECT 
                    p.Id as IdPersona,
                    p.Correo,
                    p.Nombre,
                    p.SegundoNombre,
                    p.Apellidos,
                    u.TipoUsuario,
                    e.Departamento,
                    e.Puesto,
                    e.idEmpresa as IdEmpresa
                FROM PlaniFy.Persona p
                INNER JOIN PlaniFy.Usuario u ON p.Id = u.idPersona
                LEFT JOIN PlaniFy.Empleado e ON p.Id = e.idPersona
                WHERE p.Id = @IdPersona
                AND (e.Estado IS NULL OR e.Estado != @StatusInactive)";

            return await connection.QueryFirstOrDefaultAsync<UserDataDto>(
                query, 
                new { IdPersona = idPersona, StatusInactive = EmployeeConstants.StatusInactive });
        }

        public async Task<bool> CreateUserAsync(Usuario usuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.Usuario (idPersona, TipoUsuario, Contrasena)
                    VALUES (@idPersona, @tipoUsuario, @contrasena)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", usuario.IdPersona);
                command.Parameters.AddWithValue("@tipoUsuario", usuario.TipoUsuario);
                command.Parameters.AddWithValue("@contrasena", usuario.Contrasena);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(int personaId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(1) FROM PlaniFy.Usuario WHERE idPersona = @idPersona";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", personaId);

                var result = await command.ExecuteScalarAsync();
                if (result == null)
                {
                    return false;
                }
                
                var count = Convert.ToInt32(result);
                return count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Usuario?> GetUserByIdAsync(int personaId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT u.idPersona as IdPersona, u.TipoUsuario, u.Contrasena
                    FROM PlaniFy.Usuario u
                    WHERE u.idPersona = @idPersona";

                var user = await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { idPersona = personaId });
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Usuario?> GetUserByEmailAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT u.idPersona as IdPersona, u.TipoUsuario, u.Contrasena
                    FROM PlaniFy.Usuario u
                    INNER JOIN PlaniFy.Persona p ON u.idPersona = p.Id
                    WHERE p.Correo = @email";

                var user = await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { email });
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserPasswordAsync(int personaId, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    UPDATE PlaniFy.Usuario 
                    SET Contrasena = @contrasena 
                    WHERE idPersona = @idPersona";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", personaId);
                command.Parameters.AddWithValue("@contrasena", password);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int personaId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "DELETE FROM PlaniFy.Usuario WHERE idPersona = @idPersona";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPersona", personaId);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public Usuario? GetByVerificationHash(string hash)
        {
            return _context.Usuario.FirstOrDefault(u => u.VerificationTokenHash == hash && !u.IsVerified);
        }

        public void MarkVerified(Usuario usuario)
        {
            usuario.IsVerified = true;
            usuario.VerificationTokenHash = null;
            usuario.VerificationTokenExpires = null;
            _context.SaveChanges();
        }
    }
}
