using backend.Models;

namespace backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> CreateUserAsync(int personaId, string password, string tipoUsuario = "Empleado");
        Task<bool> UserExistsAsync(int personaId);
        Task<Usuario?> GetUserByIdAsync(int personaId);
        Task<Usuario?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserPasswordAsync(int personaId, string password);
        Task<bool> DeleteUserAsync(int personaId);
        Task<bool> TestConnectionAsync();
    }
}
