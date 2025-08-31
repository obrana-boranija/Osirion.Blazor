using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services.Implementations;

namespace Osirion.Blazor.Core.Services;

/// <summary>
/// Factory for creating email services based on configuration
/// </summary>
public class EmailServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EmailOptions _emailOptions;

    public EmailServiceFactory(IServiceProvider serviceProvider, IOptions<EmailOptions> emailOptions)
    {
        _serviceProvider = serviceProvider;
        _emailOptions = emailOptions.Value;
    }

    /// <summary>
    /// Creates the appropriate email service based on configuration
    /// </summary>
    public IEmailService CreateEmailService()
    {
        return _emailOptions.Provider switch
        {
            EmailProviderType.Smtp => _serviceProvider.GetRequiredService<SmtpEmailService>(),
            EmailProviderType.SendGrid => _serviceProvider.GetRequiredService<SendGridEmailService>(),
            EmailProviderType.Infobip => _serviceProvider.GetRequiredService<InfobipEmailService>(),
            _ => throw new InvalidOperationException($"Unsupported email provider: {_emailOptions.Provider}")
        };
    }
}