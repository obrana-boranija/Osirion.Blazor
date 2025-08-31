# OsirionContactForm Component

A comprehensive, accessible contact form component with validation, spam protection, and contact information display. Built following Osirion's framework-agnostic design principles.

## Features

- ? **Comprehensive Validation**: Built-in validation using DataAnnotations
- ? **Accessibility**: WCAG 2.1 compliant with proper ARIA labels and keyboard navigation
- ? **Spam Protection**: Honeypot field for basic spam protection
- ? **Framework Agnostic**: Works with Bootstrap, FluentUI, MudBlazor, Radzen, or standalone
- ? **Responsive Design**: Mobile-first responsive layout
- ? **Contact Information**: Optional contact information display section
- ? **Flexible Layout**: Side-by-side or stacked layout options
- ? **Theme Support**: Light and dark theme support
- ? **Privacy Compliance**: Optional privacy policy agreement checkbox
- ? **Status Messages**: Success, error, and processing status indicators
- ? **SSR Compatible**: Works with server-side rendering

## Basic Usage

```razor
@page "/contact"
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<PageTitle>Contact Us</PageTitle>

<div class="container my-5">
    <OsirionContactForm 
        FormTitle="Get in Touch"
        FormDescription="We'd love to hear from you. Send us a message and we'll respond as soon as possible."
        OnSubmit="HandleContactSubmit"
        SubmissionResult="@submissionResult"
        SubmissionResultChanged="OnSubmissionResultChanged" />
</div>

@code {
    private ContactFormSubmissionResult submissionResult = ContactFormSubmissionResult.None;

    private async Task HandleContactSubmit(ContactFormModel model)
    {
        try
        {
            // Your form submission logic here
            await SubmitContactForm(model);
            submissionResult = ContactFormSubmissionResult.Success;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting contact form");
            submissionResult = ContactFormSubmissionResult.ServerError;
        }
    }

    private async Task OnSubmissionResultChanged(ContactFormSubmissionResult result)
    {
        submissionResult = result;
        StateHasChanged();
    }

    private async Task SubmitContactForm(ContactFormModel model)
    {
        // Example: Send email, save to database, call API, etc.
        await Task.Delay(1000); // Simulate API call
        
        // Your actual implementation here
        // await emailService.SendContactEmailAsync(model);
        // await database.SaveContactFormAsync(model);
    }
}
```

## Advanced Configuration

```razor
<OsirionContactForm 
    FormTitle="Contact Our Team"
    FormDescription="We're here to help with any questions you might have."
    
    @* Form field placeholders *@
    NamePlaceholder="Enter your full name"
    EmailPlaceholder="your.email@company.com"
    SubjectPlaceholder="What can we help you with?"
    MessagePlaceholder="Tell us more about your inquiry..."
    MessageRows="8"
    
    @* Button configuration *@
    SubmitButtonText="Send Message"
    SubmittingText="Sending..."
    ShowResetButton="true"
    ResetButtonText="Clear Form"
    ShowRequiredIndicators="true"
    
    @* Privacy and subscription options *@
    PrivacyPolicyText="I agree to the <a href='/privacy' target='_blank'>Privacy Policy</a> and <a href='/terms' target='_blank'>Terms of Service</a>"
    ShowSubscribeOption="true"
    SubscribeText="Subscribe to our newsletter for updates and insights"
    
    @* Contact information *@
    ShowContactInformation="true"
    ContactInfoTitle="How to reach us"
    ContactInfo="@contactInfo"
    
    @* Layout and theming *@
    SideBySideLayout="true"
    UseDarkTheme="false"
    
    @* Status messages *@
    SuccessMessage="Thank you! Your message has been sent successfully. We'll get back to you within 24 hours."
    ErrorMessage="We're sorry, but there was an error sending your message. Please try again or contact us directly."
    SpamMessage="Your submission appears to be spam and has been blocked. Please try again."
    
    @* Events *@
    OnSubmit="HandleContactSubmit"
    OnReset="HandleFormReset"
    OnValidationFailed="HandleValidationFailed"
    SubmissionResult="@submissionResult"
    SubmissionResultChanged="OnSubmissionResultChanged" />

@code {
    private ContactFormSubmissionResult submissionResult = ContactFormSubmissionResult.None;
    
    private ContactInformation contactInfo = new()
    {
        Address = "123 Business Street, Suite 100, City, ST 12345",
        Phone = "+1 (555) 123-4567",
        Email = "hello@yourcompany.com",
        Website = "https://yourcompany.com",
        Message = "We're available Monday through Friday, 9 AM to 6 PM EST. For urgent matters, please call our main line."
    };

    private async Task HandleContactSubmit(ContactFormModel model)
    {
        submissionResult = ContactFormSubmissionResult.Processing;
        StateHasChanged();

        try
        {
            // Validate honeypot (basic spam protection)
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                submissionResult = ContactFormSubmissionResult.SpamDetected;
                return;
            }

            // Submit the form
            await SubmitContactForm(model);
            submissionResult = ContactFormSubmissionResult.Success;
        }
        catch (ValidationException)
        {
            submissionResult = ContactFormSubmissionResult.ValidationError;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error submitting contact form for {Email}", model.Email);
            submissionResult = ContactFormSubmissionResult.ServerError;
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task HandleFormReset()
    {
        submissionResult = ContactFormSubmissionResult.None;
        // Additional reset logic if needed
    }

    private async Task HandleValidationFailed(EditContext editContext)
    {
        submissionResult = ContactFormSubmissionResult.ValidationError;
        // Log validation errors if needed
        var errors = editContext.GetValidationMessages();
        Logger.LogWarning("Contact form validation failed: {Errors}", string.Join(", ", errors));
    }

    private async Task OnSubmissionResultChanged(ContactFormSubmissionResult result)
    {
        submissionResult = result;
    }
}
```

## Integration with Email Services

### Using SendGrid

```csharp
public class EmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ISendGridClient sendGridClient, ILogger<EmailService> logger)
    {
        _sendGridClient = sendGridClient;
        _logger = logger;
    }

    public async Task SendContactEmailAsync(ContactFormModel model)
    {
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("noreply@yourcompany.com", "Your Company"),
            Subject = $"Contact Form: {model.Subject}",
            PlainTextContent = model.Message,
            HtmlContent = $@"
                <h2>New Contact Form Submission</h2>
                <p><strong>Name:</strong> {model.Name}</p>
                <p><strong>Email:</strong> {model.Email}</p>
                <p><strong>Subject:</strong> {model.Subject}</p>
                <p><strong>Message:</strong></p>
                <p>{model.Message.Replace("\n", "<br>")}</p>
                
                {(model.SubscribeToUpdates ? "<p><em>User requested to subscribe to updates</em></p>" : "")}
            "
        };

        msg.AddTo(new EmailAddress("contact@yourcompany.com", "Contact Team"));
        
        if (model.SubscribeToUpdates)
        {
            // Add to mailing list logic here
        }

        var response = await _sendGridClient.SendEmailAsync(msg);
        
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            _logger.LogError("Failed to send contact email. Status: {StatusCode}", response.StatusCode);
            throw new InvalidOperationException("Failed to send email");
        }
    }
}
```

### Using SMTP

```csharp
public class SmtpEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            Credentials = new NetworkCredential(
                _configuration["Smtp:Username"], 
                _configuration["Smtp:Password"]),
            EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"])
        };
    }

    public async Task SendContactEmailAsync(ContactFormModel model)
    {
        var message = new MailMessage()
        {
            From = new MailAddress(_configuration["Smtp:FromEmail"], _configuration["Smtp:FromName"]),
            Subject = $"Contact Form: {model.Subject}",
            Body = $@"
                Name: {model.Name}
                Email: {model.Email}
                Subject: {model.Subject}
                
                Message:
                {model.Message}
                
                Subscribe to updates: {(model.SubscribeToUpdates ? "Yes" : "No")}
            ",
            IsBodyHtml = false
        };

        message.To.Add(_configuration["Contact:ToEmail"]);
        
        await _smtpClient.SendMailAsync(message);
    }
}
```

## Database Integration

```csharp
public class ContactFormEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool AgreeToPrivacyPolicy { get; set; }
    public bool SubscribeToUpdates { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class ContactFormService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public async Task ProcessContactFormAsync(ContactFormModel model, HttpContext httpContext)
    {
        // Save to database
        var entity = new ContactFormEntity
        {
            Name = model.Name,
            Email = model.Email,
            Subject = model.Subject,
            Message = model.Message,
            AgreeToPrivacyPolicy = model.AgreeToPrivacyPolicy,
            SubscribeToUpdates = model.SubscribeToUpdates,
            SubmittedAt = DateTime.UtcNow,
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString()
        };

        _context.ContactForms.Add(entity);
        await _context.SaveChangesAsync();

        // Send email notification
        await _emailService.SendContactEmailAsync(model);

        // Subscribe to newsletter if requested
        if (model.SubscribeToUpdates)
        {
            await _newsletterService.SubscribeAsync(model.Email, model.Name);
        }
    }
}
```

## CSS Framework Integration

The component automatically adapts to your chosen CSS framework:

### Bootstrap
```razor
<!-- Bootstrap classes are automatically applied -->
<OsirionContactForm Class="shadow rounded" />
```

### MudBlazor
```razor
<!-- MudBlazor styles are automatically applied -->
<MudContainer>
    <OsirionContactForm />
</MudContainer>
```

### Custom Styling
```css
/* Override specific styles */
.osirion-contact-form-container {
    --osirion-contact-accent: #your-brand-color;
    --osirion-contact-radius: 1rem;
}

/* Custom form styling */
.my-custom-contact .osirion-contact-submit {
    background: linear-gradient(45deg, #ff6b6b, #ee5a24);
}
```

## Accessibility Features

- **Keyboard Navigation**: Full keyboard support for all interactive elements
- **Screen Reader Support**: Proper ARIA labels and roles
- **Focus Management**: Visible focus indicators and logical tab order
- **Error Handling**: Clear error messages linked to form fields
- **High Contrast**: Supports high contrast mode
- **Reduced Motion**: Respects user's motion preferences

## API Reference

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `FormTitle` | `string` | `"Write us"` | Title displayed above the form |
| `FormDescription` | `string` | `""` | Description text below the title |
| `NamePlaceholder` | `string` | `"Your name"` | Placeholder for name field |
| `EmailPlaceholder` | `string` | `"Your email address"` | Placeholder for email field |
| `SubjectPlaceholder` | `string` | `"Subject"` | Placeholder for subject field |
| `MessagePlaceholder` | `string` | `"Your message"` | Placeholder for message field |
| `MessageRows` | `int` | `6` | Number of rows for message textarea |
| `SubmitButtonText` | `string` | `"Send Message"` | Text for submit button |
| `SubmittingText` | `string` | `"Sending..."` | Text shown while submitting |
| `ShowResetButton` | `bool` | `false` | Whether to show reset button |
| `ResetButtonText` | `string` | `"Reset"` | Text for reset button |
| `ShowRequiredIndicators` | `bool` | `true` | Whether to show asterisks for required fields |
| `PrivacyPolicyText` | `string?` | `"I agree to the privacy policy"` | Privacy policy checkbox text (HTML supported) |
| `ShowSubscribeOption` | `bool` | `true` | Whether to show newsletter subscription option |
| `SubscribeText` | `string` | `"I would like to receive updates"` | Subscription checkbox text |
| `ShowContactInformation` | `bool` | `true` | Whether to show contact info section |
| `ContactInfoTitle` | `string` | `"Contact information"` | Title for contact info section |
| `ContactInfo` | `ContactInformation?` | `null` | Contact information to display |
| `SideBySideLayout` | `bool` | `true` | Whether to show form and contact info side by side |
| `UseDarkTheme` | `bool` | `false` | Whether to use dark theme styling |
| `SuccessMessage` | `string` | `"Thank you! Your message has been sent successfully..."` | Success message text |
| `ErrorMessage` | `string` | `"Sorry, there was an error..."` | Error message text |
| `SpamMessage` | `string` | `"Your submission appears to be spam..."` | Spam detection message |
| `SubmissionResult` | `ContactFormSubmissionResult` | `None` | Current submission status |

### Events

| Event | Type | Description |
|-------|------|-------------|
| `OnSubmit` | `EventCallback<ContactFormModel>` | Fired when form is submitted with valid data |
| `OnReset` | `EventCallback` | Fired when form is reset |
| `OnValidationFailed` | `EventCallback<EditContext>` | Fired when form validation fails |
| `SubmissionResultChanged` | `EventCallback<ContactFormSubmissionResult>` | Fired when submission result changes |

### Models

#### ContactFormModel
```csharp
public class ContactFormModel
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Subject { get; set; } = string.Empty;
    [Required] public string Message { get; set; } = string.Empty;
    [Range(typeof(bool), "true", "true")] public bool AgreeToPrivacyPolicy { get; set; }
    public bool SubscribeToUpdates { get; set; }
    public string Website { get; set; } = string.Empty; // Honeypot field
}
```

#### ContactInformation
```csharp
public class ContactInformation
{
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

#### ContactFormSubmissionResult
```csharp
public enum ContactFormSubmissionResult
{
    None,
    Success,
    ValidationError,
    ServerError,
    SpamDetected,
    Processing
}
```

## Best Practices

1. **Always validate server-side**: Client-side validation is for UX, server-side is for security
2. **Implement rate limiting**: Prevent abuse by limiting submissions per IP/user
3. **Use CAPTCHA for high-traffic sites**: Add reCAPTCHA or similar for additional spam protection
4. **Log submissions**: Keep audit trails for debugging and analytics
5. **Provide feedback**: Always give users clear feedback about submission status
6. **Test accessibility**: Verify the form works with screen readers and keyboard-only navigation

## Examples

See the `/examples` directory for complete working examples of the contact form in different scenarios and frameworks.