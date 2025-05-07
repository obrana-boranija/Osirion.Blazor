using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

namespace Osirion.Blazor.Cms.Admin;

public static class OsirionCmsAdminExtensions
{
    public static IServiceCollection AddOsirionCmsAdmin(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<IEventPublisher, EventBus>();
        services.AddSingleton<IEventSubscriber>(provider => provider.GetRequiredService<IEventPublisher>() as EventBus);

        // Register adapters
        services.AddScoped<IContentRepositoryAdapter, ContentRepositoryAdapter>();

        // Register feature services
        services.AddScoped<IContentEditorService, ContentEditorService>();

        // Register view models
        services.AddScoped<ContentEditorViewModel>();

        return services;
    }
}