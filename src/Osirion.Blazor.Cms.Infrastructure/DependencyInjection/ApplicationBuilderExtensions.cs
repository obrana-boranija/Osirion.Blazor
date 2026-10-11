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
    /// <remarks>
    /// Blocks the calling thread until the providers are initialized. Hosts with an asynchronous startup should call
    /// <see cref="UseOsirionCmsAsync"/> instead.
    /// </remarks>
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

    /// <summary>
    /// Initializes Osirion CMS providers without blocking the calling thread
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="cancellationToken">Cancels the initialization.</param>
    public static async Task<IApplicationBuilder> UseOsirionCmsAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IContentProviderInitializer>();

        try
        {
            await initializer.InitializeProvidersAsync().WaitAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Log the error but don't crash the application
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IContentProviderInitializer>>();
            logger.LogError(ex, "Error initializing content providers");
        }

        return app;
    }
}
