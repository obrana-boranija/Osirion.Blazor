using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure email options
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.Section));

        // Register all email service implementations
        services.AddTransient<SmtpEmailService>();
        services.AddTransient<SendGridEmailService>(provider =>
        {
            var emailOptions = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SendGridEmailService>>();
            var httpClient = new HttpClient();
            return new SendGridEmailService(emailOptions, logger, httpClient);
        });
        services.AddTransient<InfobipEmailService>(provider =>
        {
            var emailOptions = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<InfobipEmailService>>();
            var httpClient = new HttpClient();
            return new InfobipEmailService(emailOptions, logger, httpClient);
        });

        // Register the factory
        services.AddTransient<EmailServiceFactory>();

        // Register the primary email service interface
        services.AddTransient<IEmailService>(provider =>
        {
            var factory = provider.GetRequiredService<EmailServiceFactory>();
            return factory.CreateEmailService();
        });

        return services;
    }

    /// <summary>
    /// Adds email services with specific configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure email options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddOsirionEmailServices(this IServiceCollection services, Action<EmailOptions> configureOptions)
    {
        // Configure email options
        services.Configure(configureOptions);

        // Register all email service implementations
        services.AddTransient<SmtpEmailService>();
        services.AddTransient<SendGridEmailService>(provider =>
        {
            var emailOptions = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SendGridEmailService>>();
            var httpClient = new HttpClient();
            return new SendGridEmailService(emailOptions, logger, httpClient);
        });
        services.AddTransient<InfobipEmailService>(provider =>
        {
            var emailOptions = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<InfobipEmailService>>();
            var httpClient = new HttpClient();
            return new InfobipEmailService(emailOptions, logger, httpClient);
        });

        // Register the factory
        services.AddTransient<EmailServiceFactory>();

        // Register the primary email service interface
        services.AddTransient<IEmailService>(provider =>
        {
            var factory = provider.GetRequiredService<EmailServiceFactory>();
            return factory.CreateEmailService();
        });

        return services;
    }
}