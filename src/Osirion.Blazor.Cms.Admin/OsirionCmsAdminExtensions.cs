using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        // Register the event buses as scoped, so an event stays inside the circuit or request that raised it.
        // A process-wide bus delivered one admin's content, authentication and error events to the view models
        // of every other admin, for example replacing another admin's unsaved draft.
        services.TryAddScoped<IEventPublisher, EventBus>();
        services.TryAddScoped<IEventSubscriber>(provider => provider.GetRequiredService<IEventPublisher>() as EventBus
            ?? throw new InvalidOperationException("IEventPublisher is not of type EventBus."));

        services.TryAddScoped<CmsEventMediator>();

        // Add state management
        services.TryAddScoped<CmsState>();
#pragma warning disable CS0618 // Obsolete types stay registered until the next major release so existing resolutions keep working
        services.TryAddScoped<StateManager>();
        services.TryAddScoped<CmsAdminStartupService>();
#pragma warning restore CS0618

        // Register adapters
        services.TryAddScoped<IContentRepositoryAdapterFactory, ContentRepositoryAdapterFactory>();
        services.TryAddScoped(sp => {
            var factory = sp.GetRequiredService<IContentRepositoryAdapterFactory>();
            return factory.CreateDefaultAdapter();
        });

        // Register services
#pragma warning disable CS0618
        services.TryAddScoped<ErrorHandlingService>();
#pragma warning restore CS0618
        services.TryAddScoped<ContentBrowserService>();
        services.TryAddScoped<IContentEditorService, ContentEditorService>();
        services.TryAddScoped<IAdminContentService, AdminContentService>();
        services.TryAddScoped<RepositoryService>();

        // Register GitHub credential stores (encrypted browser storage, bound to the host user). The protected
        // browser storage they use exists only with interactive server components; factories keep build-time
        // validation from requiring it in hosts that never render the admin.
        services.TryAddScoped<IGitHubTokenStore>(
            serviceProvider => ActivatorUtilities.CreateInstance<ProtectedGitHubTokenStore>(serviceProvider));
        services.TryAddScoped<IGitHubSessionRestorer, GitHubSessionRestorer>();
        services.TryAddScoped<IGitHubOAuthStateStore>(
            serviceProvider => ActivatorUtilities.CreateInstance<ProtectedGitHubOAuthStateStore>(serviceProvider));

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
