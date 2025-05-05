using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Interfaces;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Admin.Extensions;

/// <summary>
/// Extension methods for adding Osirion.Blazor.Cms.Admin services
/// </summary>
public static class CmsAdminServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure CMS admin services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        Action<ICmsAdminBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Register core services
        services.TryAddScoped<IGitHubApiClient, GitHubApiClient>();
        services.TryAddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddScoped<IAuthenticationService, AuthenticationService>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<CmsAdminState, CmsAdminStatePersistent>();
        services.TryAddScoped<IAdminContentService, AdminContentService>();

        // Create builder and apply configuration
        var serviceProvider = services.BuildServiceProvider();
        var builder = new CmsAdminBuilder(
            services,
            serviceProvider.GetRequiredService<IConfiguration>(),
            serviceProvider.GetRequiredService<ILogger<CmsAdminBuilder>>());

        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services using configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Configure CMS admin options
        var section = configuration.GetSection(CmsAdminOptions.Section);
        if (section.Exists())
        {
            services.Configure<CmsAdminOptions>(section);
        }

        // Configure GitHub auth options
        var authSection = configuration.GetSection("Osirion:Cms:GitHub:Authorization");
        if (authSection.Exists())
        {
            services.Configure<GithubAuthorizationOptions>(authSection);
        }

        // Register core services
        services.TryAddScoped<IGitHubApiClient, GitHubApiClient>();
        services.TryAddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddScoped<IAuthenticationService, AuthenticationService>();
        services.TryAddScoped<IGitHubAdminService, GitHubAdminService>();
        services.TryAddScoped<CmsAdminState, CmsAdminStatePersistent>();
        services.TryAddScoped<IAdminContentService, AdminContentService>();

        return services;
    }
}