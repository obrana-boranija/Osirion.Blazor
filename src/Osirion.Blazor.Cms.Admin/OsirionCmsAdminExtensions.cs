using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;

namespace Osirion.Blazor.Cms.Admin;

/// <summary>
/// Extension methods for adding Osirion CMS Admin to the service collection
/// </summary>
public static class OsirionCmsAdminExtensions
{
    /// <summary>
    /// Adds Osirion CMS Admin services to the service collection
    /// </summary>
    public static IServiceCollection AddOsirionCmsAdmin(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<IEventPublisher, EventBus>();
        services.AddSingleton<IEventSubscriber>(provider => provider.GetRequiredService<IEventPublisher>() as EventBus
            ?? throw new InvalidOperationException("IEventPublisher is not of type EventBus."));

        services.AddSingleton<CmsEventMediator>();

        // Add state management
        services.AddScoped<CmsState>();
        services.AddScoped<StateManager>();
        services.AddScoped<CmsAdminStartupService>();

        // Register adapters
        services.AddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        services.AddScoped<IContentRepositoryAdapter>(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register services
        services.AddScoped<ErrorHandlingService>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ContentBrowserService>();
        services.AddScoped<IContentEditorService, ContentEditorService>();
        services.AddScoped<IMarkdownEditorService, MarkdownEditorService>();
        services.AddScoped<RepositoryService>();

        // Register view models
        services.AddScoped<LoginViewModel>();
        services.AddScoped<ContentBrowserViewModel>();
        services.AddScoped<ContentEditorViewModel>();
        services.AddScoped<FileExplorerViewModel>();
        services.AddScoped<RepositorySelectorViewModel>();
        services.AddScoped<BranchSelectorViewModel>();

        // Register features
        services.AddFeatures();

        return services;
    }
}