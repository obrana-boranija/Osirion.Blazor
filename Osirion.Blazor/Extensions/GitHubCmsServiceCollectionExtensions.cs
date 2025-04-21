using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services.GitHub;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for configuring GitHub CMS services in the DI container
/// </summary>
public static class GitHubCmsServiceCollectionExtensions
{
    /// <summary>
    /// Adds GitHub CMS service to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGitHubCms(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            return services;
        }

        services.Configure<GitHubCmsOptions>(configuration.GetSection(GitHubCmsOptions.Section));

        // Register the HttpClient for GitHub API
        services.AddHttpClient<IGitHubCmsService, GitHubCmsService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if (OperatingSystem.IsBrowser())
                {
                    return new HttpClientHandler(); // Use a compatible handler for browser environments
                }
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            });

        // Register the service as singleton since it uses internal caching
        services.AddSingleton<IGitHubCmsService, GitHubCmsService>();

        return services;
    }

    /// <summary>
    /// Adds GitHub CMS service to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">The configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGitHubCms(this IServiceCollection services, Action<GitHubCmsOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            return services;
        }

        services.Configure(configureOptions);

        // Register the HttpClient for GitHub API
        services.AddHttpClient<IGitHubCmsService, GitHubCmsService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if (OperatingSystem.IsBrowser())
                {
                    return new HttpClientHandler();
                }
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            });

        // Register the service as singleton since it uses internal caching
        services.AddSingleton<IGitHubCmsService, GitHubCmsService>();

        return services;
    }
}
