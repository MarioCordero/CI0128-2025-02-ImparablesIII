using backend.DTOs;

namespace backend.Services
{
    public interface IPasswordSetupService
    {
        Task<string> GeneratePasswordSetupTokenAsync(int personaId, string email);
        Task<bool> ValidateTokenAsync(string token);
        Task<PasswordSetupResponseDto> SetupPasswordAsync(PasswordSetupRequestDto request);
        Task<bool> CreateUserAsync(int personaId, string password, string tipoUsuario = "Empleado");
    }
}
