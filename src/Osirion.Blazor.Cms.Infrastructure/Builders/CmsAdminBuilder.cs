using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Builders;

/// <summary>
/// Implementation of the CMS admin builder
/// </summary>
internal class CmsAdminBuilder : CmsBuilderBase, ICmsAdminBuilder
{
    /// <summary>
    /// Initializes a new instance of the CmsAdminBuilder class
    /// </summary>
    public CmsAdminBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger<CmsAdminBuilder> logger)
        : base(services, configuration, logger)
    {
    }

    /// <inheritdoc/>
    public ICmsAdminBuilder Configure(Action<CmsAdminOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        // Configure CMS admin options
        Services.Configure(configure);

        // Register necessary HTTP clients first
        RegisterHttpClient<IGitHubApiClient, GitHubApiClient>();
        RegisterHttpClient<IGitHubTokenProvider, GitHubTokenProvider>();
        RegisterHttpClient<IAuthenticationService, AuthenticationService>();
        RegisterHttpClient<IGitHubAdminService, GitHubAdminService>();

        // Register admin services
        //Services.AddScoped<ICmsAdminService, CmsAdminService>();

        Logger.LogInformation("Configured CMS admin options");
        return this;
    }

    /// <inheritdoc/>
    public ICmsAdminBuilder AddGitHubAuth(Action<GithubAuthorizationOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        // Configure GitHub auth options
        Services.Configure(configure);

        // Register necessary HTTP clients
        RegisterHttpClient<IGitHubTokenProvider, GitHubTokenProvider>();
        RegisterHttpClient<IAuthenticationService, AuthenticationService>();

        // Register auth services
        //Services.AddScoped<IGitHubAuthService, GitHubAuthService>();

        Logger.LogInformation("Added GitHub authentication for CMS admin");
        return this;
    }

    public ICmsAdminBuilder Configure(Action<GitHubOptions> configure)
    {
        throw new NotImplementedException();
    }
}