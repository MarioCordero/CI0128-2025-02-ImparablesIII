namespace backend.Services
{
    public interface IEmailTemplates
    {
        string GetWelcomeEmailTemplate(string userName);
        string GetVerificationTemplate(string nombre, string token, string rol);
        string GetVerificationLinkTemplate(string verificationLink);
        string GetPasswordResetTemplate(string resetToken);
        string GetNotificationTemplate(string title, string message, string? actionUrl = null, string? actionText = null);
        string GetEmployeeNotificationTemplate(string employeeName, string notificationType, string details);
        string GetPasswordSetupTemplate(string employeeName, string setupUrl);
    }
}