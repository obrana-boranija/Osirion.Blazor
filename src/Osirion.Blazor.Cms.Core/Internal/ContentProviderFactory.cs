using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Interfaces;

namespace Osirion.Blazor.Cms.Internal;

/// <summary>
/// Factory for creating content providers
/// </summary>
public class ContentProviderFactory : IContentProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderFactory> _logger;
    private readonly Dictionary<string, Func<IServiceProvider, IContentProvider>> _factories = new();
    private string? _defaultProviderId;

    /// <summary>
    /// Initializes a new instance of the ContentProviderFactory class
    /// </summary>
    public ContentProviderFactory(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderFactory>? logger = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? (_serviceProvider.GetService(typeof(ILogger<ContentProviderFactory>)) as ILogger<ContentProviderFactory>)
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public IContentProvider CreateProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (_factories.TryGetValue(providerId, out var factory))
        {
            try
            {
                return factory(_serviceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating provider {ProviderId}", providerId);
                throw;
            }
        }

        throw new KeyNotFoundException($"Content provider '{providerId}' is not registered");
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetAvailableProviderTypes()
    {
        return _factories.Keys.ToList();
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

            _factories[providerId] = sp => factory(sp);
            _logger.LogInformation("Registered content provider: {ProviderId}", providerId);

            // If this is the first provider registered, set it as default
            if (_defaultProviderId == null)
            {
                _defaultProviderId = providerId;
                _logger.LogInformation("Set default provider: {ProviderId}", providerId);
            }
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
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_factories.ContainsKey(providerId))
            throw new KeyNotFoundException($"Content provider '{providerId}' is not registered");

        _defaultProviderId = providerId;
        _logger.LogInformation("Set default provider: {ProviderId}", providerId);
    }

    /// <inheritdoc/>
    public string? GetDefaultProviderId()
    {
        return _defaultProviderId;
    }
}