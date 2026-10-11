using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling GitHub webhook requests
/// </summary>
/// <remarks>
/// Handles <c>POST /api/github/webhook</c> (exact path, case-insensitive) and passes every other
/// request on. Bodies declared larger than 5 MiB are refused with 413 before the handler runs.
/// Every response carries a short generic body, so status code pages do not re-execute the request.
/// </remarks>
public class GitHubWebhookMiddleware
{
    /// <summary>
    /// The server request body limit, in bytes, applied to webhook requests.
    /// </summary>
    /// <remarks>
    /// Kestrel counts more than the decoded data of a chunked body against its limit, so a limit
    /// equal to the payload cap refuses chunked bodies a few kilobytes under it. The limit leaves
    /// room for that overhead, and the handler's exact 5 MiB read cap decides.
    /// </remarks>
    internal const long MaxRequestBodySize = 2L * GitHubWebhookHandler.MaxPayloadBytes;

    private readonly RequestDelegate _next;
    private readonly ILogger<GitHubWebhookMiddleware> _logger;
    private readonly PathString _webhookPath;

    /// <summary>Performs the GitHubWebhookMiddleware operation.</summary>
    public GitHubWebhookMiddleware(
        RequestDelegate next,
        ILogger<GitHubWebhookMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _webhookPath = new PathString("/api/github/webhook");
    }

    /// <summary>Performs the Invoke operation asynchronously.</summary>
    public async Task InvokeAsync(HttpContext context, IGitHubWebhookHandler webhookHandler)
    {
        // Only process POST requests to the webhook path
        if (!HttpMethods.IsPost(context.Request.Method) ||
            !context.Request.Path.Equals(_webhookPath, StringComparison.OrdinalIgnoreCase))
        {
            // Not a webhook request, continue the pipeline
            await _next(context);
            return;
        }

        _logger.LogInformation("Received GitHub webhook request");

        if (context.Request.ContentLength > GitHubWebhookHandler.MaxPayloadBytes)
        {
            _logger.LogWarning("Rejected GitHub webhook request: the declared body length exceeds {MaxPayloadBytes} bytes",
                GitHubWebhookHandler.MaxPayloadBytes);
            await WriteResponseAsync(context, StatusCodes.Status413PayloadTooLarge, "Webhook payload too large");
            return;
        }

        LimitRequestBodySize(context);

        try
        {
            // Process webhook and get result (this won't wait for background processing to complete)
            bool accepted = await webhookHandler.HandleWebhookAsync(context.Request);

            // Respond immediately to GitHub
            if (accepted)
            {
                await WriteResponseAsync(context, StatusCodes.Status202Accepted, "Webhook received and is being processed");
            }
            else
            {
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Webhook request rejected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub webhook request");
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "An error occurred while processing the webhook");
        }
    }

    private static void LimitRequestBodySize(HttpContext context)
    {
        // Defence in depth: the handler also stops reading past the limit, which is the only guard
        // when the server does not expose a writable limit (for example behind IIS out of process).
        // A stricter limit set by the host is kept.
        var bodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (bodySizeFeature is { IsReadOnly: false } &&
            (bodySizeFeature.MaxRequestBodySize is null || bodySizeFeature.MaxRequestBodySize > MaxRequestBodySize))
        {
            bodySizeFeature.MaxRequestBodySize = MaxRequestBodySize;
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
}
