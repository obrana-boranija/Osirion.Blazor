using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
    public static IApplicationBuilder UseGitHubWebhook(this IApplicationBuilder app)
    {
        app.UseMiddleware<GitHubWebhookMiddleware>();
        return app;
    }
}