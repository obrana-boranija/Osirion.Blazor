using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

/// <summary>Defines the ContentRepositoryAdapterFactory API contract.</summary>
public class ContentRepositoryAdapterFactory : IContentRepositoryAdapterFactory
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly ILogger<GitHubRepositoryAdapter> _gitHubLogger;
    private readonly CmsAdminOptions _options;

    /// <summary>Performs the ContentRepositoryAdapterFactory operation.</summary>
    public ContentRepositoryAdapterFactory(
        IGitHubAdminService gitHubService,
        ILogger<GitHubRepositoryAdapter> gitHubLogger,
        IOptions<CmsAdminOptions> options)
    {
        _gitHubService = gitHubService;
        _gitHubLogger = gitHubLogger;
        _options = options.Value;
    }

    /// <summary>Gets or sets the CreateAdapter value.</summary>
    public IContentRepositoryAdapter CreateAdapter(string providerType)
    {
        return providerType.ToLowerInvariant() switch
        {
            "github" => (IContentRepositoryAdapter)new GitHubRepositoryAdapter(_gitHubService, _gitHubLogger),
            // Add other providers here as needed
            _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
        };
    }

    /// <summary>Performs the CreateDefaultAdapter operation.</summary>
    public IContentRepositoryAdapter CreateDefaultAdapter()
    {
        return CreateAdapter(_options.DefaultContentProvider);
    }
}
