using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Tests.Fakes;

/// <summary>
/// Creates a service collection holding the services every ASP.NET Core host registers before application code runs.
/// </summary>
internal static class HostServices
{
    /// <summary>
    /// Creates a service collection with logging and an empty <see cref="IConfiguration"/>.
    /// </summary>
    public static ServiceCollection Create() => Create(new ConfigurationBuilder().Build());

    /// <summary>
    /// Creates a service collection with logging and the given <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="configuration">The configuration the host exposes to application code.</param>
    public static ServiceCollection Create(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        return services;
    }
}
