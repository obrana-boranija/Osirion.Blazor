using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Factories;

/// <summary>
/// Factory for creating repository instances
/// </summary>
public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RepositoryFactory> _logger;
    private readonly string _defaultProviderId;
    private readonly HashSet<string> _availableProviders;

    public RepositoryFactory(
        IServiceProvider serviceProvider,
        ILogger<RepositoryFactory> logger,
        string defaultProviderId)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _defaultProviderId = defaultProviderId ?? throw new ArgumentNullException(nameof(defaultProviderId));

        // Populate available providers - could come from configuration or auto-discovery
        _availableProviders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "github",
            "filesystem"
            // Add other providers as they become available
        };
    }

    public IContentRepository CreateContentRepository(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        try
        {
            // Handle GitHub provider
            if (providerId.StartsWith("github", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Creating GitHub content repository");
                return _serviceProvider.GetRequiredService<GitHubContentRepository>();
            }

            // Handle FileSystem provider
            if (providerId.StartsWith("filesystem", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Creating FileSystem content repository");
                throw new NotImplementedException("FileSystem content repository is not yet implemented");
                //return _serviceProvider.GetRequiredService<FileSystemContentRepository>();
            }

            throw new ArgumentException($"Unsupported provider type: {providerId}", nameof(providerId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content repository for provider {ProviderId}", providerId);
            throw;
        }
    }

    public IDirectoryRepository CreateDirectoryRepository(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        try
        {
            // Handle GitHub provider
            if (providerId.StartsWith("github", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Creating GitHub directory repository");
                throw new NotImplementedException("GitHub directory repository is not yet implemented");
                //return _serviceProvider.GetRequiredService<GitHubDirectoryRepository>();
            }

            // Handle FileSystem provider
            if (providerId.StartsWith("filesystem", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Creating FileSystem directory repository");
                throw new NotImplementedException("FileSystem directory repository is not yet implemented");
                //return _serviceProvider.GetRequiredService<FileSystemDirectoryRepository>();
            }

            throw new ArgumentException($"Unsupported provider type: {providerId}", nameof(providerId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating directory repository for provider {ProviderId}", providerId);
            throw;
        }
    }

    public string GetDefaultProviderId()
    {
        return _defaultProviderId;
    }

    public IEnumerable<string> GetAvailableProviderIds()
    {
        return _availableProviders;
    }
}