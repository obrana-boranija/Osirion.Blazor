using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features;
using Osirion.Blazor.Cms.Admin.Features.Authentication.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Interfaces;
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
    public static IServiceCollection AddOsirionCmsAdminDI(this IServiceCollection services)
    {
        // Register core services
        services.TryAddSingleton<IEventPublisher, EventBus>();
        services.TryAddSingleton<IEventSubscriber>(provider => provider.GetRequiredService<IEventPublisher>() as EventBus
            ?? throw new InvalidOperationException("IEventPublisher is not of type EventBus."));

        services.TryAddSingleton<CmsEventMediator>();

        // Add state management
        services.TryAddScoped<CmsState>();
        services.TryAddScoped<StateManager>();
        services.TryAddScoped<CmsAdminStartupService>();

        // Register adapters
        services.TryAddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        services.TryAddScoped<IContentRepositoryAdapter>(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register services
        services.TryAddScoped<ErrorHandlingService>();
        services.TryAddScoped<ContentBrowserService>();
        services.TryAddScoped<IContentEditorService, ContentEditorService>();
        services.TryAddScoped<IMarkdownEditorService, MarkdownEditorService>();
        services.TryAddScoped<IAdminContentService, AdminContentService>();
        services.TryAddScoped<RepositoryService>();

        // Register view models
        services.TryAddScoped<LoginViewModel>();
        services.TryAddScoped<ContentBrowserViewModel>();
        services.TryAddScoped<ContentEditorViewModel>();
        services.TryAddScoped<FileExplorerViewModel>();
        services.TryAddScoped<RepositorySelectorViewModel>();
        services.TryAddScoped<BranchSelectorViewModel>();

        services.AddBlazoredLocalStorage();

        // Register features
        services.AddFeatures();

        return services;
    }
}