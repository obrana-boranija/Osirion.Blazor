using System.Net.Http.Headers;
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
        // The API key is attached per request, so a shared or pooled HttpClient is never mutated.
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

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send") { Content = content };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _emailOptions.SendGrid.ApiKey);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            
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
                    value = ContactEmailBodyBuilder.Build(contactForm, DateTime.UtcNow)
                }
            }
        };

        return payload;
    }

}
