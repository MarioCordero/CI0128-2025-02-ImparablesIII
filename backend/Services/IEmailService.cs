using backend.DTOs;

namespace backend.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDto> SendEmailAsync(SendEmailDto emailDto);
    }
}
