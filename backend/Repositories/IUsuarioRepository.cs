using backend.Models;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUserByEmailAsync(string email);
        Task<UserDataDto?> GetUserDataAsync(int idPersona);
        Task<bool> CreateUserAsync(Usuario usuario);
        Task<bool> UserExistsAsync(int personaId);
        Task<Usuario?> GetUserByIdAsync(int personaId);
        Task<bool> VerifyUserPasswordAsync(int personaId, string plainTextPassword);
        Task<bool> UpdateUserPasswordAsync(int personaId, string password);
        Task<bool> DeleteUserAsync(int personaId);
        Usuario? GetByVerificationHash(string hash);
        void MarkVerified(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
    }
}