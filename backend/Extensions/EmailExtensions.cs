using backend_lab.DTOs;
using backend_lab.Models;
using backend_lab.Services;
using Microsoft.Extensions.Configuration;

namespace backend_lab.Extensions
{
    /// <summary>
    /// Extension methods for easy email sending throughout the application
    /// </summary>
    public static class EmailExtensions
    {
        /// <summary>
        /// Send email using IServiceProvider to get email settings from DI container
        /// </summary>
        /// <param name="serviceProvider">Service provider to resolve dependencies</param>
        /// <param name="receiverEmail">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="isHtml">Whether the body contains HTML content</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        public static async Task<bool> SendEmailAsync(this IServiceProvider serviceProvider, string receiverEmail, string subject, string body, bool isHtml = false)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            
            return await EmailHelper.SendEmailAsync(receiverEmail, subject, body, emailSettings, isHtml);
        }

        /// <summary>
        /// Send email with detailed response using IServiceProvider
        /// </summary>
        /// <param name="serviceProvider">Service provider to resolve dependencies</param>
        /// <param name="receiverEmail">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="isHtml">Whether the body contains HTML content</param>
        /// <returns>EmailResponseDto with detailed result information</returns>
        public static async Task<EmailResponseDto> SendEmailWithResponseAsync(this IServiceProvider serviceProvider, string receiverEmail, string subject, string body, bool isHtml = false)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            
            return await EmailHelper.SendEmailWithResponseAsync(receiverEmail, subject, body, emailSettings, isHtml);
        }
    }
}
