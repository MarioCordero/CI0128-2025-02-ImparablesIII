using backend.DTOs;

namespace backend.Repositories
{
    public interface IPersonaRepository
    {
        Task<int> CreatePersonaAsync(string email, string nombre, string? segundoNombre, string apellidos, DateTime fechaNacimiento, string cedula, string rol, int? telefono, int direccionId);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
        Task<bool> TestConnectionAsync();
    }
}
