using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for HttpClient registration
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Adds a HttpClient if not already registered
    /// </summary>
    public static IServiceCollection TryAddHttpClient<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        // Check if already registered
        if (services.Any(s => s.ServiceType == typeof(TClient) && s.ImplementationType == typeof(TImplementation)))
        {
            return services;
        }

        services.AddHttpClient<TClient, TImplementation>();
        return services;
    }
}