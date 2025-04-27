using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring Osirion CMS Infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCmsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register options
        services.Configure<GitHubOptions>(configuration.GetSection("Osirion:Cms:GitHub"));

        // Register GitHub services
        services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();
        services.AddScoped<GitHubContentRepository>();

        // Register factories
        services.AddScoped<IRepositoryFactory>(sp =>
        {
            var serviceProvider = sp.GetRequiredService<IServiceProvider>();
            var logger = sp.GetRequiredService<ILogger<RepositoryFactory>>();

            // Get default provider from configuration
            var defaultProvider = configuration["Osirion:Cms:DefaultProvider"] ?? "github";

            return new RepositoryFactory(serviceProvider, logger, defaultProvider);
        });

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}