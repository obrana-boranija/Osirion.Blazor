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
    /// <remarks>
    /// Implementations must verify the <c>X-Hub-Signature-256</c> header against the configured
    /// webhook secret before acting on the payload, and reject the request when it does not verify.
    /// </remarks>
    /// <param name="httpRequest">The HTTP request</param>
    /// <returns>True if the webhook was verified and accepted; false if it was rejected</returns>
    Task<bool> HandleWebhookAsync(HttpRequest httpRequest);

    /// <summary>
    /// Processes webhook data after the request has been read
    /// </summary>
    /// <remarks>
    /// The signature is verified against <paramref name="payload"/> exactly as for
    /// <see cref="HandleWebhookAsync(HttpRequest)"/>; unsigned payloads are rejected.
    /// </remarks>
    /// <param name="eventType">GitHub event type (e.g. "ping", "push")</param>
    /// <param name="signature">Signature header from GitHub</param>
    /// <param name="payload">Request body payload</param>
    /// <returns>True if the webhook was verified and accepted; false if it was rejected</returns>
    Task<bool> ProcessWebhookAsync(string eventType, string signature, string payload);
}
