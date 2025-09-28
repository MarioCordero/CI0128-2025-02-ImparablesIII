using backend_lab_c28730.DTOs;

namespace backend_lab_c28730.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDto> SendEmailAsync(SendEmailDto emailDto);
    }
}
