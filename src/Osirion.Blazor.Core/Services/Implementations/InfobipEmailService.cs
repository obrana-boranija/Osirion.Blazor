using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Core.Services.Implementations;

/// <summary>
/// Infobip email service implementation
/// </summary>
public class InfobipEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<InfobipEmailService> _logger;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfobipEmailService"/> class.
    /// </summary>
    /// <param name="emailOptions">The email options configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="httpClient">The HTTP client for API requests.</param>
    public InfobipEmailService(EmailOptions emailOptions, ILogger<InfobipEmailService> logger, HttpClient httpClient)
    {
        _emailOptions = emailOptions;
        _logger = logger;
        _httpClient = httpClient;
        // Authorization and the target URL are applied per request, so a shared or pooled HttpClient is never mutated.
    }

    /// <inheritdoc />
    public async Task<EmailResult> SendEmailAsync(ContactFormModel contactForm, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailData = CreateInfobipPayload(contactForm);
            var jsonContent = JsonSerializer.Serialize(emailData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending email via Infobip to {ToEmail}", _emailOptions.ToEmail);

            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(_emailOptions.Infobip.BaseUrl, UriKind.Absolute), "/email/3/send")) { Content = content };
            request.Headers.Authorization = new AuthenticationHeaderValue("App", _emailOptions.Infobip.ApiKey);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully via Infobip");
                return EmailResult.Success("Email sent via Infobip");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Infobip API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return EmailResult.Failure($"Infobip API error: {response.StatusCode}", errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via Infobip");
            return EmailResult.Failure($"Infobip error: {ex.Message}", ex.ToString());
        }
    }

    /// <summary>
    /// Creates the Infobip API payload from the contact form data.
    /// </summary>
    /// <param name="contactForm">The contact form data.</param>
    /// <returns>An object representing the Infobip API payload.</returns>
    private object CreateInfobipPayload(ContactFormModel contactForm)
    {
        var infobip = _emailOptions.Infobip;
        var subject = _emailOptions.SubjectTemplate.Replace("{subject}", contactForm.Subject);

        var payload = new
        {
            messages = new[]
            {
                new
                {
                    from = new
                    {
                        email = infobip.FromEmail,
                        name = _emailOptions.FromName
                    },
                    to = new[]
                    {
                        new
                        {
                            email = _emailOptions.ToEmail
                        }
                    },
                    replyTo = new[]
                    {
                        new
                        {
                            email = contactForm.Email,
                            name = contactForm.Name
                        }
                    },
                    subject = subject,
                    content = new
                    {
                        templateId = (string?)null, // Can be used for custom templates
                        html = ContactEmailBodyBuilder.Build(contactForm, DateTime.UtcNow)
                    }
                }
            }
        };

        return payload;
    }

}
