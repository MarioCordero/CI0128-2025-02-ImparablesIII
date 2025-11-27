using backend.DTOs;

namespace backend.Services
{
    public interface IEmployerService
    {
        Task<bool> RegisterEmployerAsync(SignUpEmployerDto form);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsCedulaAvailableAsync(string cedula);
        Task<bool> VerifyAndCreateUserAsync(int personaId, string password);
        Task<KPIResponseDTO?> GetKPIAsync(int userId);
    }
}