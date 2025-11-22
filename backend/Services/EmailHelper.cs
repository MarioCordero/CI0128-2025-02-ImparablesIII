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

        public EmailHelper(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString("N"); // 32 chars
        }

        public string HashToken(string raw)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw))); // 64 hex chars
        }

        public async Task<bool> SendVerificationLinkAsync(string email, string rawToken)
        {
            var link = $"http://localhost:5173/verify?token={rawToken}";
            var body = $@"
                <p>Verifica tu cuenta:</p>
                <p><a href=""{link}"">Activar ahora</a></p>
                <p>Si no funciona, copia y pega: {link}</p>
                <p>El enlace expira en 24 horas.</p>";
            return await SendEmailAsync(email, "Verificación de cuenta", body, true);
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