using backend.DTOs;
using backend.Services;

namespace backend.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly IEmailTemplates _emailTemplates;
        private static Dictionary<string, (string token, DateTime expiry, int personaId)> _verificationTokens = new();

        public EmailVerificationService(
            IEmailService emailService,
            ILogger<EmailVerificationService> logger,
            IEmailTemplates emailTemplates)
        {
            _emailService = emailService;
            _logger = logger;
            _emailTemplates = emailTemplates;
        }

        public async Task<bool> SendVerificationEmailAsync(SendVerificationEmailDto dto)
        {
            try
            {
                var token = GenerateToken(6);
                var expiry = DateTime.UtcNow.AddHours(24);
                
                _verificationTokens[dto.Email] = (token, expiry, dto.PersonaId);
                _logger.LogInformation($"Generated verification token for {dto.Email}");

                var verificationContent = GetVerificationEmailContent(dto.Nombre, token, dto.Rol);
                
                var emailDto = new SendEmailDto
                {
                    ReceiverEmail = dto.Email,
                    Subject = $"Verifica tu correo - {dto.Rol}",
                    Body = verificationContent,
                    IsHtml = true
                };

                var result = await _emailService.SendEmailAsync(emailDto);
                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending verification email: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool IsValid, int PersonaId)> VerifyTokenAsync(string email, string token)
        {
            if (!_verificationTokens.TryGetValue(email, out var verification))
            {
                _logger.LogWarning($"No verification token found for {email}");
                return (false, 0);
            }

            if (verification.expiry < DateTime.UtcNow)
            {
                _verificationTokens.Remove(email);
                _logger.LogWarning($"Verification token expired for {email}");
                return (false, 0);
            }

            if (verification.token != token)
            {
                _logger.LogWarning($"Invalid token for {email}");
                return (false, 0);
            }

            _verificationTokens.Remove(email);
            _logger.LogInformation($"Email verified successfully for {email}");
            return (true, verification.personaId);
        }

        public Task<bool> IsTokenExpiredAsync(string email)
        {
            if (_verificationTokens.TryGetValue(email, out var verification))
                return Task.FromResult(verification.expiry < DateTime.UtcNow);
            
            return Task.FromResult(true);
        }

        private string GenerateToken(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(36)])
                .ToArray());
        }

        private string GetVerificationEmailContent(string nombre, string token, string rol)
        {
            return _emailTemplates.GetVerificationTemplate(nombre, token, rol);
        }
    }
}