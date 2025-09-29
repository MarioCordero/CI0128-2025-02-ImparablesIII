using backend_lab.DTOs;
using backend_lab.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace backend_lab.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<EmailResponseDto> SendEmailAsync(SendEmailDto emailDto)
        {
            _logger.LogInformation($"Attempting to send email to {emailDto.ReceiverEmail} with subject: {emailDto.Subject}");
            
            var result = await EmailHelper.SendEmailWithResponseAsync(
                emailDto.ReceiverEmail, 
                emailDto.Subject, 
                emailDto.Body, 
                _emailSettings,
                emailDto.IsHtml
            );

            if (result.Success)
            {
                _logger.LogInformation($"Email sent successfully to {emailDto.ReceiverEmail}");
            }
            else
            {
                _logger.LogError($"Failed to send email to {emailDto.ReceiverEmail}: {result.Message}");
            }
            
            return result;
        }
    }
}
