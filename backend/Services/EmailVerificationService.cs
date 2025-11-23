using backend.DTOs;
using backend.Services;
using backend.Repositories;

namespace backend.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailTemplates _emailTemplates;
        private static Dictionary<string, (string token, DateTime expiry, int personaId)> _verificationTokens = new();

        public EmailVerificationService(
            IUsuarioRepository usuarioRepository,
            IEmailService emailService,
            ILogger<EmailVerificationService> logger,
            IEmailTemplates emailTemplates)
        {
            _usuarioRepository = usuarioRepository;
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

        public async Task<(bool Success, int PersonaId, string Rol)> VerifyLinkTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, 0, string.Empty);

            // Hash the token to compare with stored hash
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashedToken = Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token)));
            
            var usuario = _usuarioRepository.GetByVerificationHash(hashedToken);
            if (usuario == null || usuario.IsVerified)
            {
                _logger.LogWarning($"Invalid or already verified token");
                return (false, 0, string.Empty);
            }

            if (usuario.VerificationTokenExpires < DateTime.UtcNow)
            {
                _logger.LogWarning($"Verification token expired");
                return (false, 0, string.Empty);
            }

            usuario.IsVerified = true;
            usuario.VerificationTokenExpires = null;
            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation($"Email verified successfully for PersonaId: {usuario.IdPersona}");
            return (true, usuario.IdPersona, usuario.TipoUsuario);
        }
    }
}