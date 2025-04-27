using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Service that initializes content providers
/// </summary>
public class ContentProviderInitializer : IContentProviderInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderInitializer> _logger;

    /// <summary>
    /// Initializes a new instance of the ContentProviderInitializer class
    /// </summary>
    public ContentProviderInitializer(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderInitializer> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task InitializeProvidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing content providers");

        try
        {
            // Register all content providers
            var registrations = _serviceProvider.GetServices<IProviderRegistration>();
            foreach (var registration in registrations)
            {
                try
                {
                    registration.Register(_serviceProvider);
                    _logger.LogInformation("Registered provider: {ProviderId}", registration.ProviderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error registering provider: {ProviderId}", registration.ProviderId);
                }
            }

            // Process any default provider from registrations
            var defaultProvider = registrations.FirstOrDefault(r => r.IsDefault);
            if (defaultProvider != null)
            {
                var factory = _serviceProvider.GetRequiredService<IContentProviderFactory>();
                factory.SetDefaultProvider(defaultProvider.ProviderId);
                _logger.LogInformation("Set default provider from registration: {ProviderId}",
                    defaultProvider.ProviderId);
            }

            // Process any explicit default provider setters
            var defaultSetters = _serviceProvider.GetServices<IDefaultProviderSetter>();
            foreach (var setter in defaultSetters)
            {
                try
                {
                    setter.SetDefault(_serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting default provider");
                }
            }

            // Initialize providers - we'll use the initialization method that is actually defined
            var providerManager = _serviceProvider.GetRequiredService<IContentProviderManager>();
            foreach (var provider in providerManager.GetAllProviders())
            {
                try
                {
                    // Check for IContentCaching interface that might have InitializeAsync
                    if (provider is Domain.Interfaces.Content.IContentCaching cachingProvider)
                    {
                        await cachingProvider.InitializeAsync(cancellationToken);
                        _logger.LogInformation("Initialized provider: {ProviderId}", provider.ProviderId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing provider: {ProviderId}", provider.ProviderId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during content provider initialization");
        }
    }
}