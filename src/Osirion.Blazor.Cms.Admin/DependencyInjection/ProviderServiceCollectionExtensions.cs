using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

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
        services.Configure<GitHubAdminOptions>(
            configuration.GetSection("Osirion:Cms:GitHub:Admin"));

        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.AddScoped<IGitHubAdminService, GitHubAdminService>();

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