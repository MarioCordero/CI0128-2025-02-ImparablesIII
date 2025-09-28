namespace backend.Services
{
    /// <summary>
    /// HTML email templates that match the EmployeeList component styling
    /// </summary>
    public static class EmailTemplates
    {
        private static string GetBaseTemplate(string title, string content)
        {
            return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #dbeafe;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #eaf4fa;
            border-radius: 40px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
            padding: 40px;
        }}
        .header {{
            text-align: center;
            margin-bottom: 32px;
        }}
        .logo {{
            font-size: 48px;
            font-weight: 900;
            color: #000000;
            margin-bottom: 8px;
            letter-spacing: 0.05em;
        }}
        .subtitle {{
            color: #6b7280;
            font-size: 18px;
            margin: 0;
        }}
        .content {{
            background-color: #ffffff;
            border-radius: 20px;
            padding: 32px;
            margin: 24px 0;
            box-shadow: inset 0 2px 4px 0 rgba(0, 0, 0, 0.06);
        }}
        .greeting {{
            font-size: 20px;
            font-weight: 600;
            color: #2d384b;
            margin-bottom: 16px;
        }}
        .message {{
            color: #4b5563;
            font-size: 16px;
            margin-bottom: 20px;
        }}
        .highlight-box {{
            background-color: #dbeafe;
            border-radius: 16px;
            padding: 20px;
            margin: 24px 0;
            text-align: center;
        }}
        .token {{
            font-size: 32px;
            font-weight: 900;
            color: #2d384b;
            letter-spacing: 4px;
            margin: 12px 0;
        }}
        .button {{
            display: inline-block;
            background-color: #2d384b;
            color: #ffffff;
            text-decoration: none;
            font-weight: 600;
            border-radius: 9999px;
            padding: 12px 32px;
            margin: 16px 0;
            transition: background-color 0.3s;
        }}
        .button:hover {{
            background-color: #1e293b;
        }}
        .footer {{
            text-align: center;
            margin-top: 32px;
            padding-top: 24px;
            border-top: 1px solid #e5e7eb;
            color: #6b7280;
            font-size: 14px;
        }}
        .team-signature {{
            color: #2d384b;
            font-weight: 600;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""logo"">Imparables</div>
            <p class=""subtitle"">Tu plataforma de gestión empresarial</p>
        </div>
        
        <div class=""content"">
            {content}
        </div>
        
        <div class=""footer"">
            <p>Este correo fue enviado automáticamente desde la plataforma Imparables.</p>
            <p class=""team-signature"">Equipo Imparables</p>
            <p style=""font-size: 12px; margin-top: 16px;"">
                © 2025 Imparables. Todos los derechos reservados.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Welcome email template for new user registration
        /// </summary>
        public static string GetWelcomeEmailTemplate(string userName)
        {
            var content = $@"
                <div class=""greeting"">¡Bienvenido a Imparables, {userName}!</div>
                <div class=""message"">
                    Nos alegra tenerte en nuestra plataforma. Tu registro ha sido completado exitosamente 
                    y ya puedes comenzar a utilizar todas las funcionalidades disponibles.
                </div>
                
                <div class=""highlight-box"">
                    <h3 style=""margin-top: 0; color: #2d384b;"">¿Qué puedes hacer ahora?</h3>
                    <ul style=""text-align: left; color: #4b5563; margin: 0;"">
                        <li>Gestionar información de empleados</li>
                        <li>Acceder a herramientas de administración</li>
                        <li>Configurar tu perfil empresarial</li>
                        <li>Explorar todas nuestras funcionalidades</li>
                    </ul>
                </div>
                
                <div class=""message"">
                    Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos. 
                    Estamos aquí para apoyarte en cada paso del camino.
                </div>
                
                <div style=""text-align: center;"">
                    <a href=""#"" class=""button"">Explorar Plataforma</a>
                </div>";

            return GetBaseTemplate("¡Bienvenido a Imparables!", content);
        }

        /// <summary>
        /// Password reset email template
        /// </summary>
        public static string GetPasswordResetTemplate(string resetToken)
        {
            var content = $@"
                <div class=""greeting"">Solicitud de Restablecimiento de Contraseña</div>
                <div class=""message"">
                    Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en la plataforma Imparables.
                </div>
                
                <div class=""highlight-box"">
                    <h3 style=""margin-top: 0; color: #2d384b;"">Tu código de verificación:</h3>
                    <div class=""token"">{resetToken}</div>
                    <p style=""color: #6b7280; font-size: 14px; margin-bottom: 0;"">
                        Este código expira en 30 minutos
                    </p>
                </div>
                
                <div class=""message"">
                    <strong>Instrucciones:</strong>
                    <ol style=""color: #4b5563; padding-left: 20px;"">
                        <li>Ingresa el código de verificación en la página de restablecimiento</li>
                        <li>Crea una nueva contraseña segura</li>
                        <li>Confirma tu nueva contraseña</li>
                    </ol>
                </div>
                
                <div class=""message"" style=""background-color: #fef3c7; border-radius: 12px; padding: 16px; border-left: 4px solid #f59e0b;"">
                    <strong>⚠️ Importante:</strong> Si no solicitaste este restablecimiento de contraseña, 
                    ignora este correo. Tu cuenta permanecerá segura.
                </div>";

            return GetBaseTemplate("Restablecimiento de Contraseña - Imparables", content);
        }

        /// <summary>
        /// General notification email template
        /// </summary>
        public static string GetNotificationTemplate(string title, string message, string? actionUrl = null, string? actionText = null)
        {
            var actionButton = string.IsNullOrEmpty(actionUrl) ? "" : 
                $@"<div style=""text-align: center;"">
                    <a href=""{actionUrl}"" class=""button"">{actionText ?? "Ver Detalles"}</a>
                </div>";

            var content = $@"
                <div class=""greeting"">{title}</div>
                <div class=""message"">{message}</div>
                {actionButton}";

            return GetBaseTemplate(title, content);
        }

        /// <summary>
        /// Employee notification email template
        /// </summary>
        public static string GetEmployeeNotificationTemplate(string employeeName, string notificationType, string details)
        {
            var content = $@"
                <div class=""greeting"">Notificación de Empleado</div>
                <div class=""message"">
                    Se ha realizado una actualización relacionada con el empleado: <strong>{employeeName}</strong>
                </div>
                
                <div class=""highlight-box"">
                    <h3 style=""margin-top: 0; color: #2d384b;"">Tipo de Notificación</h3>
                    <p style=""font-size: 18px; font-weight: 600; color: #2d384b; margin: 8px 0;"">{notificationType}</p>
                </div>
                
                <div class=""message"">
                    <strong>Detalles:</strong><br>
                    {details}
                </div>
                
                <div style=""text-align: center;"">
                    <a href=""#"" class=""button"">Ver Lista de Empleados</a>
                </div>";

            return GetBaseTemplate($"Notificación de Empleado - {employeeName}", content);
        }

        /// <summary>
        /// Password setup email template for new employees
        /// </summary>
        public static string GetPasswordSetupTemplate(string employeeName, string setupUrl)
        {
            var content = $@"
                <div class=""greeting"">¡Bienvenido a Imparables, {employeeName}!</div>
                <div class=""message"">
                    Tu registro como empleado ha sido completado exitosamente. Para acceder a la plataforma, 
                    necesitas configurar tu contraseña.
                </div>
                
                <div class=""highlight-box"">
                    <h3 style=""margin-top: 0; color: #2d384b;"">Configura tu contraseña</h3>
                    <p style=""color: #4b5563; margin: 16px 0;"">
                        Haz clic en el botón de abajo para configurar tu contraseña y activar tu cuenta.
                    </p>
                    <div style=""text-align: center;"">
                        <a href=""{setupUrl}"" class=""button"">Configurar Contraseña</a>
                    </div>
                    <p style=""color: #6b7280; font-size: 14px; margin-top: 16px; margin-bottom: 0;"">
                        Este enlace expira en 30 minutos por seguridad
                    </p>
                </div>
                
                <div class=""message"">
                    <strong>Instrucciones:</strong>
                    <ol style=""color: #4b5563; padding-left: 20px;"">
                        <li>Haz clic en el botón de arriba</li>
                        <li>Crea una contraseña segura (8-16 caracteres)</li>
                        <li>Confirma tu contraseña</li>
                        <li>¡Ya podrás acceder a la plataforma!</li>
                    </ol>
                </div>
                
                <div class=""message"" style=""background-color: #fef3c7; border-radius: 12px; padding: 16px; border-left: 4px solid #f59e0b;"">
                    <strong>⚠️ Importante:</strong> Si no solicitaste este registro, 
                    contacta inmediatamente con el administrador del sistema.
                </div>";

            return GetBaseTemplate("Configuración de Contraseña - Imparables", content);
        }
    }
}
