using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Configuration;

namespace backend.Extensions
{
    public static class EmailExtensions
    {
        public static async Task<bool> SendEmailAsync(this IServiceProvider serviceProvider, string receiverEmail, string subject, string body, bool isHtml = false)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            return await EmailHelper.SendEmailAsync(receiverEmail, subject, body, emailSettings, isHtml);
        }

        public static async Task<EmailResponseDto> SendEmailWithResponseAsync(this IServiceProvider serviceProvider, string receiverEmail, string subject, string body, bool isHtml = false)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);            
            return await EmailHelper.SendEmailWithResponseAsync(receiverEmail, subject, body, emailSettings, isHtml);
        }
    }
}
