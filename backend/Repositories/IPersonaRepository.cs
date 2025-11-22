using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IPersonaRepository
    {
        Task<int> CreatePersonaAsync(Persona persona);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
        Task<bool> TestConnectionAsync();
    }
}
