using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Registry for content providers
/// </summary>
public class ContentProviderRegistry : IContentProviderRegistry
{
    private readonly IEnumerable<IContentProvider> _providers;
    private readonly ILogger<ContentProviderRegistry> _logger;
    private string? _defaultProviderId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderRegistry"/> class
    /// </summary>
    /// <param name="providers">The collection of content providers</param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException">Thrown when required dependencies are null</exception>
    public ContentProviderRegistry(
        IEnumerable<IContentProvider> providers,
        ILogger<ContentProviderRegistry> logger)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (_providers.Any())
        {
            _defaultProviderId = _providers.First().ProviderId;
            _logger.LogInformation("Set initial default provider: {ProviderId}", _defaultProviderId);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IContentProvider> GetAllProviders() => _providers;

    /// <inheritdoc />
    public IContentProvider? GetDefaultProvider()
    {
        if (string.IsNullOrWhiteSpace(_defaultProviderId))
        {
            return _providers.FirstOrDefault();
        }

        return _providers.FirstOrDefault(p => p.ProviderId == _defaultProviderId);
    }

    /// <inheritdoc />
    public IContentProvider? GetProvider(string providerId)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        var provider = _providers.FirstOrDefault(p => p.ProviderId == providerId);

        if (provider is null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", providerId);
        }

        return provider;
    }

    /// <inheritdoc />
    public void SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        if (!_providers.Any(p => p.ProviderId == providerId))
        {
            throw new ContentProviderException($"No provider registered with ID: {providerId}");
        }

        _defaultProviderId = providerId;
        _logger.LogInformation("Set default provider: {ProviderId}", providerId);
    }
}