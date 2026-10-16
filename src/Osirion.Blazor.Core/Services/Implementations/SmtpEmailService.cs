using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Core.Services.Implementations;

/// <summary>
/// SMTP email service implementation
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SmtpEmailService"/> class.
/// </remarks>
/// <param name="emailOptions">The email options configuration.</param>
/// <param name="logger">The logger instance.</param>
public class SmtpEmailService(IOptions<EmailOptions> emailOptions, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    /// <inheritdoc />
    public async Task<EmailResult> SendEmailAsync(ContactFormModel contactForm, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(contactForm);

            logger.LogInformation("Sending email via SMTP to {ToEmail}", _emailOptions.ToEmail);

            await client.SendMailAsync(message, cancellationToken);

            logger.LogInformation("Email sent successfully via SMTP");
            return EmailResult.Success("Email sent via SMTP");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email via SMTP");
            return EmailResult.Failure($"SMTP error: {ex.Message}", ex.ToString());
        }
    }

    /// <summary>
    /// Creates an SMTP client with configuration from email options.
    /// </summary>
    /// <returns>A configured SMTP client.</returns>
    private SmtpClient CreateSmtpClient()
    {
        var smtp = _emailOptions.Smtp;

        var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtp.Username, smtp.Password)
        };

        return client;
    }

    /// <summary>
    /// Creates a mail message from the contact form data.
    /// </summary>
    /// <param name="contactForm">The contact form data.</param>
    /// <returns>A configured mail message.</returns>
    private MailMessage CreateMailMessage(ContactFormModel contactForm)
    {
        var smtp = _emailOptions.Smtp;
        var subject = _emailOptions.SubjectTemplate.Replace("{subject}", contactForm.Subject);

        var message = new MailMessage
        {
            From = new MailAddress(smtp.FromEmail, _emailOptions.FromName),
            Subject = subject,
            Body = ContactEmailBodyBuilder.Build(contactForm, DateTime.UtcNow),
            IsBodyHtml = true
        };

        message.To.Add(_emailOptions.ToEmail);
        message.ReplyToList.Add(new MailAddress(contactForm.Email, contactForm.Name));

        return message;
    }

}
