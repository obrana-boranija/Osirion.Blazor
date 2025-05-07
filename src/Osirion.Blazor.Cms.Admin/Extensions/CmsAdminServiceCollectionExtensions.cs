using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Admin.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor.Cms.Admin services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CmsAdminOptions>? configureOptions = null)
    {
        // Register configuration
        if (configureOptions != null)
        {
            services.Configure<CmsAdminOptions>(configureOptions);
        }
        else
        {
            services.Configure<CmsAdminOptions>(
                configuration.GetSection("Osirion:Cms:Admin"));
        }

        // Core services
        services.AddScoped<CmsState>();
        services.AddScoped<StatePersistenceService>();
        services.AddSingleton<EventBus>();

        // Infrastructure
        services.AddScoped<IContentRepositoryAdapter, GitHubRepositoryAdapter>();
        services.AddScoped<IGitHubApiClient, GitHubApiClient>();

        // Feature Services
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ContentBrowserService>();
        services.AddScoped<ContentEditorService>();
        services.AddScoped<RepositoryService>();

        // ViewModels
        services.AddScoped<LoginViewModel>();
        services.AddScoped<FileExplorerViewModel>();
        services.AddScoped<ContentEditorViewModel>();
        services.AddScoped<RepositorySelectorViewModel>();
        services.AddScoped<BranchSelectorViewModel>();

        return services;
    }
}