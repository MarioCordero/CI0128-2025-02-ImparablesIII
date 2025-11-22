using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IEmployerRepository
    {
        Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular);
        Task<int> CreatePersonaAsync(Persona persona);
        Task<bool> CreateUserAsync(Usuario usuario);
        Task<EmployerResponseDto?> GetEmployerByIdAsync(int personaId);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
        Task<Persona?> GetByCedulaAsync(string cedula);
    }
}