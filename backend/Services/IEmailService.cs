using backend_lab.DTOs;

namespace backend_lab.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDto> SendEmailAsync(SendEmailDto emailDto);
    }
}
