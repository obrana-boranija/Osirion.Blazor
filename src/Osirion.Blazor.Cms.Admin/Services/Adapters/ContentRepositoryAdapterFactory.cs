using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Services.Adapters;

public class ContentRepositoryAdapterFactory : IContentRepositoryAdapterFactory
{
    private readonly IGitHubAdminService _gitHubService;
    private readonly ILogger<GitHubRepositoryAdapter> _gitHubLogger;
    private readonly CmsAdminOptions _options;

    public ContentRepositoryAdapterFactory(
        IGitHubAdminService gitHubService,
        ILogger<GitHubRepositoryAdapter> gitHubLogger,
        IOptions<CmsAdminOptions> options)
    {
        _gitHubService = gitHubService;
        _gitHubLogger = gitHubLogger;
        _options = options.Value;
    }

    public IContentRepositoryAdapter CreateAdapter(string providerType)
    {
        return providerType.ToLowerInvariant() switch
        {
            "github" => new GitHubRepositoryAdapter(_gitHubService, _gitHubLogger),
            // Add other providers here as needed
            _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
        };
    }

    public IContentRepositoryAdapter CreateDefaultAdapter()
    {
        return CreateAdapter(_options.DefaultContentProvider);
    }
}