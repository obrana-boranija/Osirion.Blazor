using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling GitHub webhook requests
/// </summary>
public class GitHubWebhookMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GitHubWebhookMiddleware> _logger;

    public GitHubWebhookMiddleware(RequestDelegate next, ILogger<GitHubWebhookMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, IGitHubWebhookHandler webhookHandler)
    {
        // Only process POST requests to the webhook path
        if (context.Request.Method == "POST" &&
            context.Request.Path.StartsWithSegments("/api/github/webhook", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Received GitHub webhook request");

            try
            {
                bool success = await webhookHandler.HandleWebhookAsync(context.Request);

                // Return 200 OK if the webhook was processed successfully
                if (success)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("Webhook processed successfully");
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Failed to process webhook");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GitHub webhook");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An error occurred while processing the webhook");
            }
        }
        else
        {
            // Not a webhook request, continue the pipeline
            await _next(context);
        }
    }
}