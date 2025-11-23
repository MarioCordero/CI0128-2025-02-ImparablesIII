using backend.DTOs;

namespace backend.Services
{
    public interface IEmailVerificationService
    {
        Task<bool> SendVerificationEmailAsync(SendVerificationEmailDto dto);
        Task<(bool IsValid, int PersonaId)> VerifyTokenAsync(string email, string token);
        Task<bool> IsTokenExpiredAsync(string email);
        Task<(bool Success, int PersonaId, string Rol)> VerifyLinkTokenAsync(string token);
    }
}