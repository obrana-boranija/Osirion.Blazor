using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Registers the per-user GitHub services with a scoped lifetime, so every circuit or request holds its own
/// authentication state and its own GitHub API client.
/// </summary>
/// <remarks>
/// <c>AddHttpClient&lt;TClient, TImplementation&gt;</c> registers a transient factory without an
/// implementation type, which a TryAdd guard cannot recognize. These methods therefore remove every earlier
/// registration of the service before adding the scoped one, so the scoped lifetime holds whichever
/// registration method ran first.
/// </remarks>
internal static class GitHubAdminServiceRegistration
{
    /// <summary>
    /// Name of the <see cref="IHttpClientFactory"/> client the authentication service uses. It matches the
    /// name of the typed client registered by earlier versions, so host configuration for it still applies.
    /// </summary>
    internal const string AuthenticationHttpClientName = nameof(IAuthenticationService);

    /// <summary>
    /// Registers <see cref="IAuthenticationService"/> as scoped, replacing any earlier registration.
    /// </summary>
    internal static IServiceCollection AddScopedAuthenticationService(this IServiceCollection services)
    {
        services.AddGitHubHttpClientCookiePolicy();
        services.AddHttpClient(AuthenticationHttpClientName);

        services.RemoveAll<IAuthenticationService>();
        services.AddScoped<IAuthenticationService>(CreateAuthenticationService);

        return services;
    }

    /// <summary>
    /// Registers <see cref="IGitHubAdminService"/> as scoped, replacing any earlier registration.
    /// </summary>
    internal static IServiceCollection AddScopedGitHubAdminService(this IServiceCollection services)
    {
        services.AddGitHubHttpClientCookiePolicy();

        services.RemoveAll<IGitHubAdminService>();
        services.AddScoped<IGitHubAdminService, GitHubAdminService>();

        return services;
    }

    /// <summary>
    /// Turns off cookies on the primary handler of every <see cref="IHttpClientFactory"/> client that calls GitHub.
    /// </summary>
    /// <remarks>
    /// Registered as a post-configuration, so a primary handler the host configures after this call is covered too.
    /// </remarks>
    internal static IServiceCollection AddGitHubHttpClientCookiePolicy(this IServiceCollection services)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostConfigureOptions<HttpClientFactoryOptions>, GitHubHttpClientCookiePolicy>());

        return services;
    }

    private static AuthenticationService CreateAuthenticationService(IServiceProvider serviceProvider)
    {
        var httpClient = serviceProvider
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(AuthenticationHttpClientName);

        return ActivatorUtilities.CreateInstance<AuthenticationService>(serviceProvider, httpClient);
    }
}
