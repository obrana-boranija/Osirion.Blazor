using Microsoft.AspNetCore.Http;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Interface for handling GitHub webhook requests
/// </summary>
public interface IGitHubWebhookHandler
{
    /// <summary>
    /// Handles a GitHub webhook request
    /// </summary>
    /// <param name="httpRequest">The HTTP request</param>
    /// <returns>True if the webhook was processed successfully</returns>
    Task<bool> HandleWebhookAsync(HttpRequest httpRequest);
}