using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class ContentProviderFactory : IContentProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContentProviderFactory> _logger;
    private readonly Dictionary<string, Func<IServiceProvider, IContentProvider>> _factories;
    private string? _defaultProviderId;

    public ContentProviderFactory(
        IServiceProvider serviceProvider,
        ILogger<ContentProviderFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _factories = new Dictionary<string, Func<IServiceProvider, IContentProvider>>();
    }

    public IContentProvider CreateProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_factories.TryGetValue(providerId, out var factory))
            throw new KeyNotFoundException($"No content provider registered with ID '{providerId}'");

        try
        {
            return factory(_serviceProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content provider: {ProviderId}", providerId);
            throw new ContentProviderException($"Failed to create content provider: {providerId}", ex);
        }
    }

    public IEnumerable<string> GetAvailableProviderTypes()
    {
        return _factories.Keys;
    }

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

            // If this is the first provider, make it the default
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

    public void SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_factories.ContainsKey(providerId))
            throw new KeyNotFoundException($"No content provider registered with ID '{providerId}'");

        _defaultProviderId = providerId;
        _logger.LogInformation("Set default provider: {ProviderId}", providerId);
    }

    public string? GetDefaultProviderId()
    {
        return _defaultProviderId;
    }
}