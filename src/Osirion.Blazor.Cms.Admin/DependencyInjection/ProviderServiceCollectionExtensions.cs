using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Admin.DependencyInjection;

/// <summary>
/// Extension methods for registering content provider services
/// </summary>
public static class ProviderServiceCollectionExtensions
{
    /// <summary>
    /// Adds GitHub admin services
    /// </summary>
    public static IServiceCollection AddGitHubAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CmsAdminOptions>(
            configuration.GetSection(CmsAdminOptions.Section));

        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

        // Scoped, replacing any earlier registration, with cookies turned off on the GitHub HTTP clients: a transient
        // or shared admin service, or a pooled handler that keeps cookies, would let one user's GitHub token,
        // repository, branch or cookies reach another user
        services.AddScopedGitHubAdminService();

        return services;
    }

    /// <summary>
    /// Adds FileSystem admin services
    /// </summary>
    public static IServiceCollection AddFileSystemAdminServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileSystemAdminOptions>(
            configuration.GetSection("Osirion:Cms:FileSystem:Admin"));

        // Register file system services
        // Note: Implementation will depend on actual services in the Infrastructure layer
        //services.AddScoped<IFileSystemAdminService, FileSystemAdminService>();

        return services;
    }
}
