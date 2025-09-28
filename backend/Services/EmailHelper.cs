using backend_lab_c28730.DTOs;
using backend_lab_c28730.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace backend_lab_c28730.Services
{
    /// <summary>
    /// Static helper class for sending emails directly without going through the API
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Send an email directly using the email settings
        /// </summary>
        /// <param name="receiverEmail">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="emailSettings">Email configuration settings</param>
        /// <param name="isHtml">Whether the body contains HTML content</param>
        /// <returns>True if email was sent successfully, false otherwise</returns>
        public static async Task<bool> SendEmailAsync(string receiverEmail, string subject, string body, EmailSettings emailSettings, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                
                // Set sender
                message.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
                
                // Set recipient
                message.To.Add(new MailboxAddress("", receiverEmail));
                
                // Set subject and body
                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain")
                {
                    Text = body
                };

                // Send the email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort, true);
                    await client.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.SenderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send an email with detailed response information
        /// </summary>
        /// <param name="receiverEmail">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="emailSettings">Email configuration settings</param>
        /// <param name="isHtml">Whether the body contains HTML content</param>
        /// <returns>EmailResponseDto with detailed result information</returns>
        public static async Task<EmailResponseDto> SendEmailWithResponseAsync(string receiverEmail, string subject, string body, EmailSettings emailSettings, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                
                // Set sender
                message.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
                
                // Set recipient
                message.To.Add(new MailboxAddress("", receiverEmail));
                
                // Set subject and body
                message.Subject = subject;
                message.Body = new TextPart(isHtml ? "html" : "plain")
                {
                    Text = body
                };

                // Send the email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort, true);
                    await client.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.SenderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

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
