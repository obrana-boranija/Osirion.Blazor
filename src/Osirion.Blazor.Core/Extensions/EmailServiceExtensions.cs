using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;

namespace Osirion.Blazor.Core.Extensions;

/// <summary>
/// Extension methods for configuring email services
/// </summary>
public static class EmailServiceExtensions
{
    /// <summary>
    /// Name of the <see cref="HttpClient"/> the SendGrid provider uses.
    /// </summary>
    internal const string SendGridHttpClientName = "Osirion.Email.SendGrid";

    /// <summary>
    /// Name of the <see cref="HttpClient"/> the Infobip provider uses.
    /// </summary>
    internal const string InfobipHttpClientName = "Osirion.Email.Infobip";

    /// <summary>
    /// Upper bound for one call to an email provider API.
    /// </summary>
    internal static readonly TimeSpan ProviderRequestTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.Section));

        return services.AddEmailServiceRegistrations();
    }

    /// <summary>
    /// Adds email services with specific configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure email options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionEmailServices(this IServiceCollection services, Action<EmailOptions> configureOptions)
    {
        services.Configure(configureOptions);

        return services.AddEmailServiceRegistrations();
    }

    /// <summary>
    /// Registers the provider implementations, their pooled HTTP clients, the factory and <see cref="IEmailService"/>.
    /// </summary>
    private static IServiceCollection AddEmailServiceRegistrations(this IServiceCollection services)
    {
        // IHttpClientFactory pools handlers (no socket exhaustion, DNS changes are honoured) and the timeout bounds
        // a stalled provider so a contact form submission cannot hang.
        services.AddHttpClient(SendGridHttpClientName, client => client.Timeout = ProviderRequestTimeout);
        services.AddHttpClient(InfobipHttpClientName, client => client.Timeout = ProviderRequestTimeout);

        services.AddTransient<SmtpEmailService>();
        services.AddTransient(provider => new SendGridEmailService(
            provider.GetRequiredService<IOptions<EmailOptions>>().Value,
            provider.GetRequiredService<ILogger<SendGridEmailService>>(),
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(SendGridHttpClientName)));
        services.AddTransient(provider => new InfobipEmailService(
            provider.GetRequiredService<IOptions<EmailOptions>>().Value,
            provider.GetRequiredService<ILogger<InfobipEmailService>>(),
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(InfobipHttpClientName)));

        services.AddTransient<EmailServiceFactory>();
        services.AddTransient<IEmailService>(provider => provider.GetRequiredService<EmailServiceFactory>().CreateEmailService());

        return services;
    }
}
