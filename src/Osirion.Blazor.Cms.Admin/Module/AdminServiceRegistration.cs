using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;

namespace Osirion.Blazor.Cms.Admin.Module;

public static class AdminServiceRegistration
{
    public static IServiceCollection AddOsirionCmsAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CmsAdminOptions>? configureOptions = null)
    {
        // Register configuration options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CmsAdminOptions>(
                configuration.GetSection("Osirion:Cms:Admin"));
        }

        // Register core services
        services.AddScoped<CmsApplicationState>();
        services.AddScoped<StateManager>();
        services.AddSingleton<CmsEventMediator>();

        // Register adapters with factory pattern
        services.AddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        services.AddScoped<IContentRepositoryAdapter>(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register feature services
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ContentBrowserService>();
        services.AddScoped<ContentEditorService>();
        services.AddScoped<RepositoryService>();

        // Register view models
        services.AddScoped<Features.ContentBrowser.ViewModels.ContentBrowserViewModel>();
        services.AddScoped<Features.ContentEditor.ViewModels.ContentEditorViewModel>();
        services.AddScoped<Features.Repository.ViewModels.RepositorySelectorViewModel>();
        services.AddScoped<Features.Repository.ViewModels.BranchSelectorViewModel>();
        services.AddScoped<Features.Authentication.ViewModels.LoginViewModel>();

        return services;
    }
}