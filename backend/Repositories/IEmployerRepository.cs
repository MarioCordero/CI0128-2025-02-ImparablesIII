using backend.DTOs;

namespace backend.Repositories
{
    public interface IEmployerRepository
    {
        Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular);
        Task<int> CreatePersonaAsync(string email, string nombre, string apellidos, DateTime fechaNacimiento, string cedula, string rol, int? telefono, int direccionId);
        Task<bool> CreateUsuarioAsync(int personaId, string password, string tipoUsuario);
        Task<EmployerResponseDto?> GetEmployerByIdAsync(int personaId);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
    }
}
