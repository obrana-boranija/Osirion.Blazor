using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Builders;

/// <summary>
/// Base implementation for CMS builders
/// </summary>
public abstract class CmsBuilderBase : ICmsBuilder
{
    protected readonly IConfiguration Configuration;
    protected readonly ILogger Logger;

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the CmsBuilderBase class
    /// </summary>
    protected CmsBuilderBase(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger logger)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers an HttpClient for a service with proper error handling
    /// </summary>
    protected void RegisterHttpClient<TClient, TImplementation>()
        where TClient : class
        where TImplementation : class, TClient
    {
        // Check if TClient is already registered as an HttpClient
        if (!Services.Any(d => d.ServiceType == typeof(TClient) &&
                          d.ImplementationType == typeof(TImplementation)))
        {
            try
            {
                // Register an HTTP client with default configuration
                Services.AddHttpClient<TClient, TImplementation>();
                Logger.LogDebug("Registered HTTP client for {ServiceType}", typeof(TClient).Name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to register HTTP client for {ServiceType}", typeof(TClient).Name);

                // Fallback: register the service directly
                Services.AddScoped<TClient, TImplementation>();
                Logger.LogWarning("Registered {ServiceType} without HTTP client integration", typeof(TClient).Name);
            }
        }
    }
}