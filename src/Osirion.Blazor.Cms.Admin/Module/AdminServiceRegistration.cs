using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;

namespace Osirion.Blazor.Cms.Admin.Module;

public static class AdminServiceRegistration
{
    public static IServiceCollection AddOsirionCmsAdmin(this IServiceCollection services)
    {
        // Register core services
        services.AddScoped<CmsApplicationState>();
        services.AddScoped<StateManager>();
        services.AddSingleton<CmsEventMediator>();

        // Register adapters
        services.AddScoped<IContentRepositoryAdapter, GitHubRepositoryAdapter>();

        // Register feature services
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ContentBrowserService>();
        services.AddScoped<ContentEditorService>();
        services.AddScoped<RepositoryService>();

        // Register view models
        services.AddScoped<ContentBrowserViewModel>();
        services.AddScoped<ContentEditorViewModel>();
        services.AddScoped<RepositorySelectorViewModel>();
        services.AddScoped<BranchSelectorViewModel>();

        return services;
    }
}