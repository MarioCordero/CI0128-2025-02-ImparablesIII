using backend.DTOs;
using backend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class EmailHelper : IEmailHelper
    {
        private readonly EmailSettings _settings;
        private readonly IEmailTemplates _emailTemplates;


        public EmailHelper(IOptions<EmailSettings> options, IEmailTemplates emailTemplates)
        {
            _settings = options.Value;
            _emailTemplates = emailTemplates;
        }

        public string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string HashToken(string raw)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));
        }

        public async Task<bool> SendVerificationLinkAsync(string email, string rawToken, string userType)
        {
            string link;
            string body;
            string subject;

            if (userType == "Empleador")
            {
                // RECORDAR CAMBIAR POR HOST DE FRONTEND PARA EMPLEADORES
                link = $"http://localhost:8080/verify?token={rawToken}";
                body = _emailTemplates.GetVerificationLinkTemplate(link);
                subject = "Verifica tu Cuenta de Empleador - Imparables";
            }
            else
            {
                // RECORDAR CAMBIAR POR HOST DE FRONTEND PARA EMPLEADOS
                link = $"http://localhost:8080/password-setup?token={rawToken}";
                body = _emailTemplates.GetWelcomeEmailTemplate(link);
                subject = "Verifica tu Cuenta de Empleado - Imparables";
            }

            return await SendEmailAsync(email, subject, body, true);
        }

        public async Task<bool> SendEmailAsync(string receiverEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress("", receiverEmail));
                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, true);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmailResponseDto> SendEmailWithResponseAsync(string receiverEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress("", receiverEmail));
                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, true);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return new EmailResponseDto
                {
                    Success = true,
                    Message = "Email sent successfully",
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new EmailResponseDto
                {
                    Success = false,
                    Message = $"Failed to send email: {ex.Message}",
                    SentAt = DateTime.UtcNow
                };
            }
        }
    }
}