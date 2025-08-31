using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Core.Services;

/// <summary>
/// Interface for email services
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously
    /// </summary>
    /// <param name="contactForm">The contact form data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the email sending operation</returns>
    Task<EmailResult> SendEmailAsync(ContactFormModel contactForm, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an email sending operation
/// </summary>
public class EmailResult
{
    /// <summary>
    /// Indicates if the email was sent successfully
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if the email failed to send
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional details about the operation
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static EmailResult Success(string? details = null) => new()
    {
        IsSuccess = true,
        Details = details
    };

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static EmailResult Failure(string errorMessage, string? details = null) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        Details = details
    };
}