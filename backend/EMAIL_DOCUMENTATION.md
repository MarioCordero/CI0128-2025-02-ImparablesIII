# Email Sending Documentation

This project implements **two approaches** for sending emails: a **REST API approach** for external testing and integration, and a **class method approach** for internal application use.

## 🚀 Quick Start

### 1. API Approach (For Testing & External Integration)

Use HTTP requests to send emails via REST endpoints.

#### Send Email Endpoint
```bash
curl -X POST "http://localhost:5011/api/Email/send" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverEmail": "recipient@example.com",
    "subject": "Your Subject Here",
    "body": "Your email content here..."
  }'
```

#### Health Check
```bash
curl -X GET "http://localhost:5011/api/Email/health"
```

### 2. Class Method Approach (For Internal Application Use)

Use direct method calls within your C# code for better performance and integration.

#### Option A: Using EmailHelper (Static Methods)
```csharp
using backend.Services;
using backend.Models;

// Simple send (returns bool)
var emailSettings = GetEmailSettingsFromConfiguration();
bool success = await EmailHelper.SendEmailAsync(
    "recipient@example.com",
    "Subject",
    "Body content",
    emailSettings
);

// Detailed send (returns EmailResponseDto)
var result = await EmailHelper.SendEmailWithResponseAsync(
    "recipient@example.com",
    "Subject", 
    "Body content",
    emailSettings
);
```

#### Option B: Using Extension Methods (Recommended)
```csharp
using backend.Extensions;

// In any controller or service with IServiceProvider access
public class MyController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    
    public MyController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task SomeAction()
    {
        // Simple send
        bool success = await _serviceProvider.SendEmailAsync(
            "recipient@example.com",
            "Subject",
            "Body content"
        );
        
        // Detailed send
        var result = await _serviceProvider.SendEmailWithResponseAsync(
            "recipient@example.com",
            "Subject",
            "Body content"
        );
    }
}
```

## 📁 Configuration

Email settings are stored in `emailconfig.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 465,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "Your App Name"
  }
}
```

## 🎯 When to Use Each Approach

### Use API Approach When:
- Testing email functionality with curl/Postman
- Frontend applications need to send emails
- External systems need to integrate
- You want to monitor/log all email requests
- You need rate limiting or authentication

### Use Class Method Approach When:
- Sending emails from within your C# application
- You need better performance (no HTTP overhead)
- Sending emails as part of business logic (user registration, password reset, etc.)
- You want type safety and compile-time checking
- You need to send emails in background processes

## 📋 Examples in This Project

### API Examples:
- `POST /api/Email/send` - Direct email sending
- `GET /api/Email/health` - Service health check

### Class Method Examples:
- `POST /api/User/register` - User registration with welcome email
- `POST /api/User/forgot-password` - Password reset with email notification

## 🏗️ Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Frontend      │    │   External       │    │   Internal      │
│   (Vue.js)      │    │   Services       │    │   C# Code       │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         v                       v                       │
┌─────────────────────────────────────────┐              │
│          Email API Endpoints            │              │
│     (EmailController.cs)                │              │
└─────────────────────────────────────────┘              │
         │                                                │
         │                                                │
         v                                                v
┌─────────────────────────────────────────────────────────────────┐
│                    Email Service Layer                          │
│  • EmailService.cs (DI Service)                                │
│  • EmailHelper.cs (Static Methods)                             │
│  • EmailExtensions.cs (Extension Methods)                      │
└─────────────────────────────────────────────────────────────────┘
         │
         v
┌─────────────────────────────────────────┐
│          MailKit/MimeKit                │
│        (SMTP Implementation)            │
└─────────────────────────────────────────┘
```

## 🔧 Benefits of Hybrid Approach

1. **Flexibility**: Choose the right approach for each use case
2. **Performance**: Class methods avoid HTTP overhead for internal use
3. **Testability**: API endpoints make testing and integration easy
4. **Maintainability**: Shared email logic in helper classes
5. **Scalability**: Can easily add features like queuing, templates, etc.

## 📝 Response Formats

### API Response
```json
{
  "success": true,
  "message": "Email sent successfully",
  "sentAt": "2025-09-28T20:27:09.338175Z"
}
```

### Class Method Response
```csharp
public class EmailResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }
}
```
