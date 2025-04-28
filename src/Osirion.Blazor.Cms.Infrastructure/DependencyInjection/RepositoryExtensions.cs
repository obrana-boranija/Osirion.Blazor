using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering repositories
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Adds repository services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionRepositories(this IServiceCollection services)
    {
        // Register GitHub repositories
        services.TryAddScoped<GitHubContentRepository>();
        services.TryAddScoped<GitHubDirectoryRepository>();

        // Register FileSystem repositories
        services.TryAddScoped<FileSystemContentRepository>();
        services.TryAddScoped<FileSystemDirectoryRepository>();

        // Register other repositories and services
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.TryAddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddScoped<IAuthenticationService, AuthenticationService>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();

        return services;
    }
}