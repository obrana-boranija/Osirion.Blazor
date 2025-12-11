using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services.Implementations;

namespace Osirion.Blazor.Core.Services;

/// <summary>
/// Factory for creating email services based on configuration
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailServiceFactory"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider to resolve email service implementations.</param>
/// <param name="emailOptions">The email configuration options.</param>
public class EmailServiceFactory(IServiceProvider serviceProvider, IOptions<EmailOptions> emailOptions)
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    /// <summary>
    /// Creates the appropriate email service based on configuration
    /// </summary>
    /// <returns>An implementation of <see cref="IEmailService"/> based on the configured provider.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported email provider is configured.</exception>
    public IEmailService CreateEmailService()
    {
        return _emailOptions.Provider switch
        {
            EmailProviderType.Smtp => serviceProvider.GetRequiredService<SmtpEmailService>(),
            EmailProviderType.SendGrid => serviceProvider.GetRequiredService<SendGridEmailService>(),
            EmailProviderType.Infobip => serviceProvider.GetRequiredService<InfobipEmailService>(),
            _ => throw new InvalidOperationException($"Unsupported email provider: {_emailOptions.Provider}")
        };
    }
}