using backend.DTOs;
using backend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailHelper _emailHelper;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger, IEmailHelper emailHelper)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _emailHelper = emailHelper;
        }

        public async Task<EmailResponseDto> SendEmailAsync(SendEmailDto emailDto)
        {
            _logger.LogInformation($"Attempting to send email to {emailDto.ReceiverEmail} with subject: {emailDto.Subject}");
            
            var result = await _emailHelper.SendEmailWithResponseAsync(
                emailDto.ReceiverEmail, 
                emailDto.Subject, 
                emailDto.Body, 
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