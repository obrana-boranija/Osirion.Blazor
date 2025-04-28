using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    public static IApplicationBuilder UseOsirionCms(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IContentProviderInitializer>();

        // Initialize providers synchronously at startup
        try
        {
            initializer.InitializeProvidersAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the application
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IContentProviderInitializer>>();
            logger.LogError(ex, "Error initializing content providers");
        }

        return app;
    }
}