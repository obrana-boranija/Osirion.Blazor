using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Middleware;

namespace Osirion.Blazor.Cms.Infrastructure.Extensions;

/// <summary>
/// Extension methods for GitHub webhook integration
/// </summary>
public static class GitHubWebhookExtensions
{
    /// <summary>
    /// Adds GitHub webhook and polling services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGitHubWebhookAndPolling(this IServiceCollection services)
    {
        // The handler needs the factory. Registering it here keeps a host without a GitHub provider startable;
        // UseGitHubWebhook then reports the missing provider instead of a service resolution error.
        services.TryAddSingleton<IGitHubApiClientFactory, GitHubApiClientFactory>();

        // Register the webhook handler
        services.AddScoped<IGitHubWebhookHandler, GitHubWebhookHandler>();

        // Register the polling service (hosted service)
        services.AddHostedService<GitHubPollingService>();

        return services;
    }

    /// <summary>
    /// Adds the GitHub webhook middleware to the application pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    /// <exception cref="InvalidOperationException">No GitHub provider is configured.</exception>
    /// <remarks>
    /// Fails at startup when no GitHub provider is configured, because a webhook without a provider can verify
    /// no signature and refresh nothing, and the failure would otherwise surface as an error on every request.
    /// </remarks>
    public static IApplicationBuilder UseGitHubWebhook(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var factory = app.ApplicationServices.GetService<IGitHubApiClientFactory>();
        if (factory is null || !factory.GetProviderNames().Any())
        {
            throw new InvalidOperationException(
                "UseGitHubWebhook needs a GitHub provider, and none is configured. Add one under " +
                "Osirion:Cms:Web:GitHub:<Name> (or Osirion:Cms:GitHub), or remove UseGitHubWebhook and AddGitHubWebhookAndPolling.");
        }

        app.UseMiddleware<GitHubWebhookMiddleware>();
        return app;
    }
}
