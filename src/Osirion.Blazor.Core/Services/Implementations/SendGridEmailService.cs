using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Core.Services.Implementations;

/// <summary>
/// SendGrid email service implementation
/// </summary>
public class SendGridEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendGridEmailService"/> class.
    /// </summary>
    /// <param name="emailOptions">The email options configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="httpClient">The HTTP client for API requests.</param>
    public SendGridEmailService(EmailOptions emailOptions, ILogger<SendGridEmailService> logger, HttpClient httpClient)
    {
        _emailOptions = emailOptions;
        _logger = logger;
        _httpClient = httpClient;
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_emailOptions.SendGrid.ApiKey}");
    }

    /// <inheritdoc />
    public async Task<EmailResult> SendEmailAsync(ContactFormModel contactForm, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailData = CreateSendGridPayload(contactForm);
            var jsonContent = JsonSerializer.Serialize(emailData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending email via SendGrid to {ToEmail}", _emailOptions.ToEmail);

            var response = await _httpClient.PostAsync("https://api.sendgrid.com/v3/mail/send", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully via SendGrid");
                return EmailResult.Success("Email sent via SendGrid");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("SendGrid API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return EmailResult.Failure($"SendGrid API error: {response.StatusCode}", errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SendGrid");
            return EmailResult.Failure($"SendGrid error: {ex.Message}", ex.ToString());
        }
    }

    /// <summary>
    /// Creates the SendGrid API payload from the contact form data.
    /// </summary>
    /// <param name="contactForm">The contact form data.</param>
    /// <returns>An object representing the SendGrid API payload.</returns>
    private object CreateSendGridPayload(ContactFormModel contactForm)
    {
        var sendGrid = _emailOptions.SendGrid;
        var subject = _emailOptions.SubjectTemplate.Replace("{subject}", contactForm.Subject);

        var payload = new
        {
            personalizations = new[]
            {
                new
                {
                    to = new[]
                    {
                        new
                        {
                            email = _emailOptions.ToEmail
                        }
                    },
                    subject = subject
                }
            },
            from = new
            {
                email = sendGrid.FromEmail,
                name = _emailOptions.FromName
            },
            reply_to = new
            {
                email = contactForm.Email,
                name = contactForm.Name
            },
            content = new[]
            {
                new
                {
                    type = "text/html",
                    value = CreateEmailBody(contactForm)
                }
            }
        };

        return payload;
    }

    /// <summary>
    /// Creates the HTML email body from the contact form data.
    /// </summary>
    /// <param name="contactForm">The contact form data.</param>
    /// <returns>A formatted HTML string for the email body.</returns>
    private static string CreateEmailBody(ContactFormModel contactForm)
    {
        var body = new StringBuilder();
        body.AppendLine("<html><body>");
        body.AppendLine("<h2>New Contact Form Submission</h2>");
        body.AppendLine($"<p><strong>Name:</strong> {WebUtility.HtmlEncode(contactForm.Name)}</p>");
        body.AppendLine($"<p><strong>Email:</strong> {WebUtility.HtmlEncode(contactForm.Email)}</p>");
        body.AppendLine($"<p><strong>Subject:</strong> {WebUtility.HtmlEncode(contactForm.Subject)}</p>");
        body.AppendLine($"<p><strong>Message:</strong></p>");
        body.AppendLine($"<div style='border: 1px solid #ccc; padding: 10px; background-color: #f9f9f9;'>");
        body.AppendLine(WebUtility.HtmlEncode(contactForm.Message).Replace("\n", "<br>"));
        body.AppendLine("</div>");
        
        if (contactForm.SubscribeToUpdates)
        {
            body.AppendLine("<p><em>The sender has requested to receive updates and newsletters.</em></p>");
        }
        
        body.AppendLine($"<p><small>Submitted on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</small></p>");
        body.AppendLine("</body></html>");
        
        return body.ToString();
    }
}