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
            var emailHelper = serviceProvider.GetRequiredService<IEmailHelper>();
            return await emailHelper.SendEmailAsync(receiverEmail, subject, body, isHtml);
        }

        public static async Task<EmailResponseDto> SendEmailWithResponseAsync(this IServiceProvider serviceProvider, string receiverEmail, string subject, string body, bool isHtml = false)
        {
            var emailHelper = serviceProvider.GetRequiredService<IEmailHelper>();           
            return await emailHelper.SendEmailWithResponseAsync(receiverEmail, subject, body, isHtml);
        }
    }
}