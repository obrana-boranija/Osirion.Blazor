using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Provides a strategy pattern implementation for content providers
/// </summary>
public class ContentProviderStrategy
{
    private readonly Dictionary<string, Func<IServiceProvider, IContentProvider>> _providerFactories;
    private readonly ILogger<ContentProviderStrategy> _logger;
    private readonly IServiceProvider _serviceProvider;
    private string? _defaultProviderId;

    /// <summary>
    /// Initializes a new instance of the ContentProviderStrategy class
    /// </summary>
    public ContentProviderStrategy(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderStrategy> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerFactories = new Dictionary<string, Func<IServiceProvider, IContentProvider>>();
    }

    /// <summary>
    /// Registers a content provider factory
    /// </summary>
    /// <param name="providerId">The provider ID</param>
    /// <param name="factory">Factory function to create the provider</param>
    public void RegisterProvider(string providerId, Func<IServiceProvider, IContentProvider> factory)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        _providerFactories[providerId] = factory;
        _logger.LogInformation("Registered content provider: {ProviderId}", providerId);

        // If this is the first provider registered, set it as default
        if (_defaultProviderId == null)
        {
            _defaultProviderId = providerId;
        }
    }

    /// <summary>
    /// Sets the default provider ID
    /// </summary>
    public void SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_providerFactories.ContainsKey(providerId))
            throw new KeyNotFoundException($"Content provider '{providerId}' is not registered");

        _defaultProviderId = providerId;
        _logger.LogInformation("Set default provider: {ProviderId}", providerId);
    }

    /// <summary>
    /// Gets the default provider ID
    /// </summary>
    public string? GetDefaultProviderId() => _defaultProviderId;

    /// <summary>
    /// Gets all available provider IDs
    /// </summary>
    public IEnumerable<string> GetAvailableProviderIds() => _providerFactories.Keys;

    /// <summary>
    /// Creates a provider instance by ID
    /// </summary>
    public IContentProvider CreateProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_providerFactories.TryGetValue(providerId, out var factory))
            throw new KeyNotFoundException($"Content provider '{providerId}' is not registered");

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

    /// <summary>
    /// Gets the default provider
    /// </summary>
    public IContentProvider? GetDefaultProvider()
    {
        if (_defaultProviderId == null)
            return null;

        return CreateProvider(_defaultProviderId);
    }
}