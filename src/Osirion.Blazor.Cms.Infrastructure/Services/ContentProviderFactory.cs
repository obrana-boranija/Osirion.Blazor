using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Factory for creating content providers using the strategy pattern
/// </summary>
public class ContentProviderFactory : IContentProviderFactory
{
    private readonly ContentProviderStrategy _strategy;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the ContentProviderFactory class
    /// </summary>
    public ContentProviderFactory(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create the strategy with the service provider
        var strategyLogger = _serviceProvider.GetService(typeof(ILogger<ContentProviderStrategy>)) as ILogger<ContentProviderStrategy>
            ?? throw new ArgumentNullException("Logger<ContentProviderStrategy>");
        _strategy = new ContentProviderStrategy(_serviceProvider, strategyLogger);
    }

    /// <inheritdoc/>
    public IContentProvider CreateProvider(string providerId)
    {
        return _strategy.CreateProvider(providerId);
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetAvailableProviderTypes()
    {
        return _strategy.GetAvailableProviderIds();
    }

    /// <inheritdoc/>
    public void RegisterProvider<T>(Func<IServiceProvider, T> factory) where T : class, IContentProvider
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        try
        {
            // Create a temporary instance to get the provider ID
            var tempProvider = factory(_serviceProvider);
            var providerId = tempProvider.ProviderId;

            if (string.IsNullOrEmpty(providerId))
                throw new InvalidOperationException("Provider ID cannot be null or empty");

            _strategy.RegisterProvider(providerId, sp => factory(sp));
            _logger.LogInformation("Registered content provider: {ProviderId}", providerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register provider");
            throw;
        }
    }

    /// <inheritdoc/>
    public void SetDefaultProvider(string providerId)
    {
        _strategy.SetDefaultProvider(providerId);
    }

    /// <inheritdoc/>
    public string? GetDefaultProviderId()
    {
        return _strategy.GetDefaultProviderId();
    }
}