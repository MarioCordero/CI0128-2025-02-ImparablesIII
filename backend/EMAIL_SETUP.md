# Email Configuration Setup

## ⚠️ IMPORTANT: Email Configuration Required

The email functionality requires a configuration file that is **NOT** included in the repository for security reasons.

## Setup Instructions:

1. **Copy the template file:**
   ```bash
   cp backend/emailconfig.template.json backend/emailconfig.json
   ```

2. **Update the configuration:**
   Edit `backend/emailconfig.json` with your actual email credentials:
   ```json
   {
     "EmailSettings": {
       "SmtpServer": "smtp.gmail.com",
       "SmtpPort": 465,
       "SenderEmail": "your-email@gmail.com",
       "SenderPassword": "your-app-specific-password-here",
       "SenderName": "Your App Name"
     }
   }
   ```

3. **Gmail App Password Setup:**
   - Enable 2-factor authentication on your Gmail account
   - Generate an app-specific password: https://myaccount.google.com/apppasswords
   - Use this app password (not your regular Gmail password)

## Current Production Configuration:
- **Email**: imparables111111@gmail.com
- **SMTP Server**: smtp.gmail.com:465 (SSL)
- **App Password**: Contact team for current password

## Security Notes:
- Never commit `emailconfig.json` to git
- The file is automatically ignored by `.gitignore`
- Use environment variables in production deployments
- Rotate app passwords regularly
