using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IUsuarioRepository
    {
        // Login tasks
        Task<Usuario?> GetUserByEmailAsync(string email);
        Task<UserDataDto?> GetUserDataAsync(int idPersona);
        // Another user-related tasks
        Task<bool> CreateUserAsync(int personaId, string password, string tipoUsuario = "Empleado");
        Task<bool> UserExistsAsync(int personaId);
        Task<Usuario?> GetUserByIdAsync(int personaId);
        Task<bool> UpdateUserPasswordAsync(int personaId, string password);
        Task<bool> DeleteUserAsync(int personaId);
        Task<bool> TestConnectionAsync();
    }
}
