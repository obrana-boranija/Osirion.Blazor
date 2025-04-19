using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure Osirion.Blazor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Osirion.Blazor services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddOsirionBlazor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register any services here if needed in the future
        // Currently there are no service dependencies as components are designed to be self-contained

        return services;
    }
}