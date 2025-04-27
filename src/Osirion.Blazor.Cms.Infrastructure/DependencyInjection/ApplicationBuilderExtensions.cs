using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Initializes Osirion CMS providers
    /// </summary>
    public static IApplicationBuilder InitializeOsirionCms(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IContentProviderInitializer>();

        // Initialize providers synchronously at startup
        initializer.InitializeProvidersAsync().GetAwaiter().GetResult();

        return app;
    }
}
