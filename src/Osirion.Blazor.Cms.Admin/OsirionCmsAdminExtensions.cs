using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;

namespace Osirion.Blazor.Cms.Admin;

public static class OsirionCmsAdminExtensions
{
    public static IServiceCollection AddOsirionCmsAdmin(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<IEventPublisher, EventBus>();
        services.AddSingleton<IEventSubscriber>(provider => provider.GetRequiredService<IEventPublisher>() as EventBus
            ?? throw new InvalidOperationException("IEventPublisher is not of type EventBus."));

        // Register adapters
        services.AddScoped<IContentRepositoryAdapter, ContentRepositoryAdapter>();

        // Register features
        services.AddFeatures();

        // Register feature services
        services.AddScoped<IContentEditorService, ContentEditorService>();

        // Register view models
        services.AddScoped<Features.ContentEditor.ViewModels.ContentEditorViewModel>();

        return services;
    }
}
