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
    private readonly string _webhookPath;

    public GitHubWebhookMiddleware(
        RequestDelegate next,
        ILogger<GitHubWebhookMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _webhookPath = "/api/github/webhook";
    }

    public async Task InvokeAsync(HttpContext context, IGitHubWebhookHandler webhookHandler)
    {
        // Only process POST requests to the webhook path
        if (context.Request.Method == HttpMethods.Post &&
            context.Request.Path.StartsWithSegments(_webhookPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Received GitHub webhook request");

            try
            {
                // Process webhook and get result (this won't wait for background processing to complete)
                bool accepted = await webhookHandler.HandleWebhookAsync(context.Request);

                // Respond immediately to GitHub
                if (accepted)
                {
                    context.Response.StatusCode = StatusCodes.Status202Accepted;
                    await context.Response.WriteAsync("Webhook received and is being processed");
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Webhook request rejected");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling GitHub webhook request");
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