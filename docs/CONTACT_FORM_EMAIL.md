# Contact Form Email Integration

The OsirionContactForm component now supports integrated email sending through multiple providers, designed to work with Static SSR.

## Features

- **Multiple Email Providers**: SMTP, SendGrid, and Infobip support
- **Static SSR Compatible**: No client-side JavaScript required
- **Spam Protection**: Built-in honeypot field
- **Configuration-based**: Configure via appsettings.json or code
- **Proper Status Handling**: Visual feedback using query parameters and page reloads

## Quick Setup

### 1. Register Services

In your `Program.cs`:

```csharp
// Option 1: Configuration-based (recommended)
builder.Services.AddOsirion(builder.Configuration);

// Option 2: Code-based configuration
builder.Services.AddOsirion(osirion =>
{
    osirion.UseEmailServices(email =>
    {
        email.Provider = EmailProviderType.Smtp;
        email.ToEmail = "contact@yoursite.com";
        email.FromName = "Contact Form";
        email.Smtp.Host = "smtp.gmail.com";
        email.Smtp.Port = 587;
        email.Smtp.EnableSsl = true;
        email.Smtp.Username = "your-email@gmail.com";
        email.Smtp.Password = "your-app-password";
        email.Smtp.FromEmail = "your-email@gmail.com";
    });
});
```

### 2. Configure appsettings.json

```json
{
  "Osirion": {
    "Email": {
      "Provider": "Smtp",
      "ToEmail": "contact@yoursite.com",
      "FromName": "Contact Form",
      "SubjectTemplate": "Contact Form: {subject}",
      "Smtp": {
        "Host": "smtp.gmail.com",
        "Port": 587,
        "EnableSsl": true,
        "Username": "your-email@gmail.com",
        "Password": "your-app-password",
        "FromEmail": "your-email@gmail.com"
      }
    }
  }
}
```

### 3. Use the Component

```razor
<OsirionContactForm 
    Id="contact-form"
    UseBuiltInEmailService="true"
    FormTitle="Get in Touch"
    FormDescription="We'd love to hear from you. Send us a message and we'll respond as soon as possible."
    ShowContactInformation="true"
    ContactInfo="@contactInfo" />

@code {
    private ContactInformation contactInfo = new()
    {
        Address = "123 Main St, City, State 12345",
        Phone = "+1 (555) 123-4567",
        Email = "info@yoursite.com",
        Website = "https://yoursite.com",
        Message = "We're here to help!"
    };
}
```

## Email Providers

### SMTP Configuration

Best for: Self-hosted email or standard email providers

```json
{
  "Osirion": {
    "Email": {
      "Provider": "Smtp",
      "Smtp": {
        "Host": "smtp.gmail.com",
        "Port": 587,
        "EnableSsl": true,
        "Username": "your-email@gmail.com",
        "Password": "your-app-password",
        "FromEmail": "your-email@gmail.com"
      }
    }
  }
}
```

### SendGrid Configuration

Best for: High-volume email sending with advanced features

```json
{
  "Osirion": {
    "Email": {
      "Provider": "SendGrid",
      "SendGrid": {
        "ApiKey": "SG.your-sendgrid-api-key",
        "FromEmail": "noreply@yoursite.com",
        "TemplateId": null
      }
    }
  }
}
```

### Infobip Configuration

Best for: International businesses with omnichannel communication needs

```json
{
  "Osirion": {
    "Email": {
      "Provider": "Infobip",
      "Infobip": {
        "BaseUrl": "https://api.infobip.com",
        "ApiKey": "your-infobip-api-key",
        "FromEmail": "noreply@yoursite.com"
      }
    }
  }
}
```

## Component Parameters

### Built-in Email Service

- `UseBuiltInEmailService`: Set to `true` to use the configured email service
- `OnSubmit`: Optional callback for custom handling (called even when using built-in service)

### Status Messages

The component uses query parameters to show status messages after form submission:

- Success: Shows green success message
- Error: Shows red error message
- Spam Detected: Shows yellow warning message

### Custom Handling

You can also handle form submission manually:

```razor
<OsirionContactForm 
    Id="contact-form"
    UseBuiltInEmailService="false"
    OnSubmit="HandleCustomSubmit" />

@code {
    private async Task HandleCustomSubmit(ContactFormModel model)
    {
        // Your custom email sending logic
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        var result = await emailService.SendEmailAsync(model);
        
        if (result.IsSuccess)
        {
            // Handle success
            navigationManager.NavigateTo("?SubmissionResult=1", forceLoad: true);
        }
        else
        {
            // Handle error
            navigationManager.NavigateTo("?SubmissionResult=3", forceLoad: true);
        }
    }
}
```

## Security Considerations

1. **Honeypot Protection**: Built-in spam protection using hidden fields
2. **Input Validation**: Server-side validation for all fields
3. **HTML Encoding**: All user input is properly encoded in emails
4. **Rate Limiting**: Consider implementing rate limiting on your server

## Troubleshooting

### Common Issues

1. **Email not sending**: Check your email provider configuration and credentials
2. **Status messages not showing**: Ensure the component has a unique `Id` parameter
3. **SMTP authentication errors**: Use app passwords for Gmail and similar providers

### Logging

Enable logging to see detailed email sending information:

```json
{
  "Logging": {
    "LogLevel": {
      "Osirion.Blazor.Core.Services": "Information"
    }
  }
}
```

## Examples

See the complete working example in the Bootstrap example project for a full implementation.