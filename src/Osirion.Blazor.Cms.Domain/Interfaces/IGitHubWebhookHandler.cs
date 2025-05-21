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

    /// <summary>
    /// Processes webhook data after the request has been read
    /// </summary>
    /// <param name="eventType">GitHub event type (e.g. "ping", "push")</param>
    /// <param name="signature">Signature header from GitHub</param>
    /// <param name="payload">Request body payload</param>
    /// <returns>True if processing was successful</returns>
    Task<bool> ProcessWebhookAsync(string eventType, string signature, string payload);
}