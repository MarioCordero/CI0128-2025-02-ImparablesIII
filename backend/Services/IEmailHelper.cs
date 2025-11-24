using backend.DTOs;

namespace backend.Services
{
    public interface IEmailHelper
    {
        string GenerateVerificationToken();
        string HashToken(string raw);
        Task<bool> SendVerificationLinkAsync(string email, string rawToken);
        Task<bool> SendEmailAsync(string receiverEmail, string subject, string body, bool isHtml = false);
        Task<EmailResponseDto> SendEmailWithResponseAsync(string receiverEmail, string subject, string body, bool isHtml = false);
    }
}