using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor;

/// <summary>
/// Extension methods to add the ContentEditor feature to the service collection
/// </summary>
public static class ContentEditorModule
{
    /// <summary>
    /// Adds content editor services to the specified IServiceCollection
    /// </summary>
    public static IServiceCollection AddContentEditor(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<ContentEditorService>();

        // Register view models
        services.AddScoped<ContentEditorViewModel>();

        return services;
    }
}