using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class ContentProviderInitializer : IContentProviderInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderInitializer> _logger;

    public ContentProviderInitializer(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderInitializer> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeProvidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing content providers");

        try
        {
            // Process default provider setters
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

            // Initialize providers
            var registry = _serviceProvider.GetRequiredService<IContentProviderRegistry>();
            foreach (var provider in registry.GetAllProviders())
            {
                try
                {
                    _logger.LogInformation("Found provider: {ProviderId}", provider.ProviderId);

                    // Initialize if it supports it
                    if (provider is IContentCaching cachingProvider)
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