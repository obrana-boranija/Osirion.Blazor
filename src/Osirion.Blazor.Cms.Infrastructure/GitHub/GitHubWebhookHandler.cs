using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Handles GitHub webhook requests to update content when repository changes occur
/// </summary>
public class GitHubWebhookHandler : IGitHubWebhookHandler
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GitHubWebhookHandler> _logger;
    private readonly GitHubOptions _options;

    public GitHubWebhookHandler(
        IContentProviderManager providerManager,
        IOptions<GitHubOptions> options,
        ILogger<GitHubWebhookHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Handles a GitHub webhook request
    /// </summary>
    /// <param name="httpRequest">The HTTP request</param>
    /// <returns>True if the webhook was processed successfully</returns>
    public async Task<bool> HandleWebhookAsync(HttpRequest httpRequest)
    {
        try
        {
            // Validate the request
            if (!await ValidateRequestAsync(httpRequest))
            {
                _logger.LogWarning("Invalid GitHub webhook request");
                return false;
            }

            // Read the request body
            string requestBody;
            using (var reader = new StreamReader(httpRequest.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Get the event type from headers
            var eventType = httpRequest.Headers["X-GitHub-Event"].ToString();
            _logger.LogInformation("Received GitHub webhook event: {EventType}", eventType);

            // Process the event based on its type
            return eventType switch
            {
                "ping" => HandlePingEvent(requestBody),
                "push" => await HandlePushEventAsync(requestBody),
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GitHub webhook");
            return false;
        }
    }

    // Update the ValidateRequestAsync method to use IHttpRequestFeature for enabling buffering
    private async Task<bool> ValidateRequestAsync(HttpRequest request)
    {
        if (string.IsNullOrEmpty(_options.WebhookSecret))
        {
            _logger.LogWarning("Webhook secret is not configured. Skipping signature validation.");
            return true; // Skip validation if no secret is configured
        }

        // Get the signature from the headers
        if (!request.Headers.TryGetValue("X-Hub-Signature-256", out var signatureHeader))
        {
            _logger.LogWarning("No signature header found in request");
            return false;
        }

        var signature = signatureHeader.ToString();
        if (string.IsNullOrEmpty(signature) || !signature.StartsWith("sha256="))
        {
            _logger.LogWarning("Invalid signature format: {Signature}", signature);
            return false;
        }

        // Enable buffering using IHttpRequestFeature
        var requestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>();
        if (requestFeature != null)
        {
            requestFeature.Body = new MemoryStream();
            await request.Body.CopyToAsync(requestFeature.Body);
            requestFeature.Body.Position = 0;
        }

        // Read the request body
        string requestBody;
        using (var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: -1,
            leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        // Reset the body position so it can be read again
        request.Body.Position = 0;

        // Compute the expected signature
        var secretBytes = Encoding.UTF8.GetBytes(_options.WebhookSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);

        using var hasher = new HMACSHA256(secretBytes);
        var hash = hasher.ComputeHash(bodyBytes);
        var expectedSignature = "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        // Compare signatures
        var isValid = string.Equals(signature, expectedSignature, StringComparison.OrdinalIgnoreCase);
        if (!isValid)
        {
            _logger.LogWarning("Signature validation failed. Expected: {Expected}, Actual: {Actual}",
                expectedSignature, signature);
        }

        return isValid;
    }

    /// <summary>
    /// Handles a ping event from GitHub
    /// </summary>
    private bool HandlePingEvent(string requestBody)
    {
        _logger.LogInformation("Successfully received ping from GitHub webhook");
        return true;
    }

    /// <summary>
    /// Handles a push event from GitHub
    /// </summary>
    private async Task<bool> HandlePushEventAsync(string requestBody)
    {
        try
        {
            // Parse the push event payload
            var pushEvent = JsonSerializer.Deserialize<GitHubPushEvent>(requestBody);
            if (pushEvent == null)
            {
                _logger.LogWarning("Failed to parse push event payload");
                return false;
            }

            // Check if this push is for the branch we're interested in
            if (!string.IsNullOrEmpty(_options.Branch) &&
                !pushEvent.Ref.EndsWith($"/{_options.Branch}"))
            {
                _logger.LogInformation("Ignoring push event for branch {Branch} (configured branch: {ConfiguredBranch})",
                    pushEvent.Ref, _options.Branch);
                return true; // Not an error, just not our branch
            }

            // Check if this is for the repository we're monitoring
            var repoFullName = $"{_options.Owner}/{_options.Repository}";
            if (pushEvent.Repository?.FullName != repoFullName)
            {
                _logger.LogInformation("Ignoring push event for repository {Repository} (configured repository: {ConfiguredRepository})",
                    pushEvent.Repository?.FullName, repoFullName);
                return true; // Not an error, just not our repo
            }

            _logger.LogInformation("Processing push event for repository {Repository}, branch {Branch}",
                pushEvent.Repository?.FullName, pushEvent.Ref);

            // Refresh the content cache for all affected providers
            var providers = _providerManager.GetAllProviders()
                .OfType<IContentCacheUpdater>()
                .Where(p => p.ProviderId.Contains(_options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}"));

            foreach (var provider in providers)
            {
                await provider.UpdateCacheAsync(pushEvent.After);
                _logger.LogInformation("Updated cache for provider: {ProviderId}", provider.ProviderId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing push event");
            return false;
        }
    }
}

/// <summary>
/// Model for GitHub push event payloads
/// </summary>
public class GitHubPushEvent
{
    /// <summary>
    /// Gets or sets the reference (e.g., "refs/heads/main")
    /// </summary>
    public string Ref { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the before SHA
    /// </summary>
    public string Before { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the after SHA
    /// </summary>
    public string After { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository information
    /// </summary>
    public GitHubPushEventRepository? Repository { get; set; }
}

/// <summary>
/// Model for the repository field in a push event
/// </summary>
public class GitHubPushEventRepository
{
    /// <summary>
    /// Gets or sets the repository ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the full repository name (e.g., "owner/repo")
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}