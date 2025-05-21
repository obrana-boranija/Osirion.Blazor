using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Handles GitHub webhook requests to update content when repository changes occur
/// </summary>
public class GitHubWebhookHandler : IGitHubWebhookHandler
{
    private readonly IContentProviderManager _providerManager;
    private readonly ILogger<GitHubWebhookHandler> _logger;
    private readonly GitHubOptions _options;

    // Rate limiting fields
    private readonly SemaphoreSlim _rateLimiter;
    private DateTime _lastWebhookProcessed = DateTime.MinValue;
    private const int MinimumSecondsBetweenWebhooks = 2;

    public GitHubWebhookHandler(
        IContentProviderManager providerManager,
        IOptions<GitHubOptions> options,
        ILogger<GitHubWebhookHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Create a semaphore with a limit of 3 concurrent webhook processes
        _rateLimiter = new SemaphoreSlim(3, 3);
    }

    /// <summary>
    /// Handles a GitHub webhook request directly from middleware
    /// </summary>
    public async Task<bool> HandleWebhookAsync(HttpRequest request)
    {
        // Apply simple rate limiting
        if ((DateTime.UtcNow - _lastWebhookProcessed).TotalSeconds < MinimumSecondsBetweenWebhooks)
        {
            _logger.LogWarning("Webhook request rejected due to rate limiting. " +
                             "Minimum {Seconds} seconds between requests required.",
                             MinimumSecondsBetweenWebhooks);
            return false;
        }

        // Read everything from request now, before using it in a Task.Run
        _lastWebhookProcessed = DateTime.UtcNow;

        // Copy essential data from the request
        string requestBody;
        string eventType;
        string signature;

        try
        {
            eventType = request.Headers["X-GitHub-Event"].ToString();
            signature = request.Headers["X-Hub-Signature-256"].ToString() ?? string.Empty;

            // Make a copy of the request body
            request.EnableBuffering();
            using (var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Reset the position so other middleware can read it
            request.Body.Position = 0;

            _logger.LogInformation("Received GitHub webhook event: {EventType}", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading webhook request data");
            return false;
        }

        // Queue the actual processing
        _ = Task.Run(async () =>
        {
            try
            {
                await ProcessWebhookAsync(eventType, signature, requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background webhook processing");
            }
        });

        // Return true immediately to respond to GitHub
        return true;
    }

    /// <summary>
    /// Processes webhook data after the request has been read
    /// </summary>
    public async Task<bool> ProcessWebhookAsync(string eventType, string signature, string payload)
    {
        // Acquire rate limiting permit or timeout after 5 seconds
        bool rateLimitPermitAcquired = false;
        try
        {
            rateLimitPermitAcquired = await _rateLimiter.WaitAsync(TimeSpan.FromSeconds(5));
            if (!rateLimitPermitAcquired)
            {
                _logger.LogWarning("Too many concurrent webhook requests. Request rejected.");
                return false;
            }

            _logger.LogInformation("Processing GitHub webhook event: {EventType}", eventType);

            // Validate the signature if we have a webhook secret
            //if (!string.IsNullOrEmpty(_options.WebhookSecret) &&
            //    !ValidateSignature(signature, payload))
            //{
            //    _logger.LogWarning("Invalid webhook signature");
            //    return false;
            //}

            // Process the event based on its type
            return eventType switch
            {
                "ping" => HandlePingEvent(payload),
                "push" => await HandlePushEventAsync(payload),
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GitHub webhook payload");
            return false;
        }
        finally
        {
            if (rateLimitPermitAcquired)
            {
                _rateLimiter.Release();
            }
        }
    }

    /// <summary>
    /// Validates the signature of webhook payload
    /// </summary>
    private bool ValidateSignature(string signature, string payload)
    {
        if (string.IsNullOrEmpty(_options.WebhookSecret))
            return true; // Skip validation if secret not configured

        if (string.IsNullOrEmpty(signature) || !signature.StartsWith("sha256="))
            return false;

        // Compute the expected signature
        var secretBytes = Encoding.UTF8.GetBytes(_options.WebhookSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(payload);

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

            _logger.LogInformation("Processing push event for repository {Repository}, branch {Branch}, commit {Sha}",
                pushEvent.Repository?.FullName, pushEvent.Ref, pushEvent.After);

            // Refresh the content cache for all affected providers
            var providers = _providerManager.GetAllProviders()
                .OfType<IContentCacheUpdater>()
                .Where(p => p.ProviderId.Contains(_options.ProviderId ?? $"github-{_options.Owner}-{_options.Repository}"));

            foreach (var provider in providers)
            {
                // Always force background updates for webhook calls
                await provider.UpdateCacheAsync(pushEvent.After, true);
                _logger.LogInformation("Queued background cache update for provider: {ProviderId}", provider.ProviderId);
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
    [JsonPropertyName("ref")]
    public string? Ref { get; set; }

    /// <summary>
    /// Gets or sets the before SHA
    /// </summary>
    [JsonPropertyName("before")]
    public string? Before { get; set; }

    /// <summary>
    /// Gets or sets the after SHA
    /// </summary>
    [JsonPropertyName("after")]
    public string? After { get; set; }

    /// <summary>
    /// Gets or sets the repository information
    /// </summary>
    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    [JsonPropertyName("pusher")]
    public Pusher? Pusher { get; set; }

    [JsonPropertyName("sender")]
    public Sender? Sender { get; set; }

    [JsonPropertyName("created")]
    public bool? Created { get; set; }

    [JsonPropertyName("deleted")]
    public bool? Deleted { get; set; }

    [JsonPropertyName("forced")]
    public bool? Forced { get; set; }

    [JsonPropertyName("base_ref")]
    public object? BaseRef { get; set; }

    [JsonPropertyName("compare")]
    public string? Compare { get; set; }

    [JsonPropertyName("commits")]
    public List<Commit>? Commits { get; set; }

    [JsonPropertyName("head_commit")]
    public HeadCommit? HeadCommit { get; set; }
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
    public string? FullName { get; set; } = string.Empty;
}

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Author
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public class Commit
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("tree_id")]
    public string? TreeId { get; set; }

    [JsonPropertyName("distinct")]
    public bool? Distinct { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("committer")]
    public Committer Committer { get; set; }

    [JsonPropertyName("added")]
    public List<object> Added { get; set; }

    [JsonPropertyName("removed")]
    public List<object> Removed { get; set; }

    [JsonPropertyName("modified")]
    public List<string> Modified { get; set; }
}

public class Committer
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public class HeadCommit
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("tree_id")]
    public string? TreeId { get; set; }

    [JsonPropertyName("distinct")]
    public bool? Distinct { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("committer")]
    public Committer Committer { get; set; }

    [JsonPropertyName("added")]
    public List<object> Added { get; set; }

    [JsonPropertyName("removed")]
    public List<object> Removed { get; set; }

    [JsonPropertyName("modified")]
    public List<string> Modified { get; set; }
}

public class License
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("spdx_id")]
    public string? SpdxId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }
}

public class Owner
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public string? GravatarId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public string? FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public string? FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public string? GistsUrl { get; set; }

    [JsonPropertyName("starred_url")]
    public string? StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public string? SubscriptionsUrl { get; set; }

    [JsonPropertyName("organizations_url")]
    public string? OrganizationsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public string? ReposUrl { get; set; }

    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public string? ReceivedEventsUrl { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("user_view_type")]
    public string? UserViewType { get; set; }

    [JsonPropertyName("site_admin")]
    public bool? SiteAdmin { get; set; }
}

public class Pusher
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public class Repository
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    [JsonPropertyName("owner")]
    public Owner Owner { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fork")]
    public bool? Fork { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("forks_url")]
    public string? ForksUrl { get; set; }

    [JsonPropertyName("keys_url")]
    public string? KeysUrl { get; set; }

    [JsonPropertyName("collaborators_url")]
    public string? CollaboratorsUrl { get; set; }

    [JsonPropertyName("teams_url")]
    public string? TeamsUrl { get; set; }

    [JsonPropertyName("hooks_url")]
    public string? HooksUrl { get; set; }

    [JsonPropertyName("issue_events_url")]
    public string? IssueEventsUrl { get; set; }

    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    [JsonPropertyName("assignees_url")]
    public string? AssigneesUrl { get; set; }

    [JsonPropertyName("branches_url")]
    public string? BranchesUrl { get; set; }

    [JsonPropertyName("tags_url")]
    public string? TagsUrl { get; set; }

    [JsonPropertyName("blobs_url")]
    public string? BlobsUrl { get; set; }

    [JsonPropertyName("git_tags_url")]
    public string? GitTagsUrl { get; set; }

    [JsonPropertyName("git_refs_url")]
    public string? GitRefsUrl { get; set; }

    [JsonPropertyName("trees_url")]
    public string? TreesUrl { get; set; }

    [JsonPropertyName("statuses_url")]
    public string? StatusesUrl { get; set; }

    [JsonPropertyName("languages_url")]
    public string? LanguagesUrl { get; set; }

    [JsonPropertyName("stargazers_url")]
    public string? StargazersUrl { get; set; }

    [JsonPropertyName("contributors_url")]
    public string? ContributorsUrl { get; set; }

    [JsonPropertyName("subscribers_url")]
    public string? SubscribersUrl { get; set; }

    [JsonPropertyName("subscription_url")]
    public string? SubscriptionUrl { get; set; }

    [JsonPropertyName("commits_url")]
    public string? CommitsUrl { get; set; }

    [JsonPropertyName("git_commits_url")]
    public string? GitCommitsUrl { get; set; }

    [JsonPropertyName("comments_url")]
    public string? CommentsUrl { get; set; }

    [JsonPropertyName("issue_comment_url")]
    public string? IssueCommentUrl { get; set; }

    [JsonPropertyName("contents_url")]
    public string? ContentsUrl { get; set; }

    [JsonPropertyName("compare_url")]
    public string? CompareUrl { get; set; }

    [JsonPropertyName("merges_url")]
    public string? MergesUrl { get; set; }

    [JsonPropertyName("archive_url")]
    public string? ArchiveUrl { get; set; }

    [JsonPropertyName("downloads_url")]
    public string? DownloadsUrl { get; set; }

    [JsonPropertyName("issues_url")]
    public string? IssuesUrl { get; set; }

    [JsonPropertyName("pulls_url")]
    public string? PullsUrl { get; set; }

    [JsonPropertyName("milestones_url")]
    public string? MilestonesUrl { get; set; }

    [JsonPropertyName("notifications_url")]
    public string? NotificationsUrl { get; set; }

    [JsonPropertyName("labels_url")]
    public string? LabelsUrl { get; set; }

    [JsonPropertyName("releases_url")]
    public string? ReleasesUrl { get; set; }

    [JsonPropertyName("deployments_url")]
    public string? DeploymentsUrl { get; set; }

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("pushed_at")]
    public int? PushedAt { get; set; }

    [JsonPropertyName("git_url")]
    public string? GitUrl { get; set; }

    [JsonPropertyName("ssh_url")]
    public string? SshUrl { get; set; }

    [JsonPropertyName("clone_url")]
    public string? CloneUrl { get; set; }

    [JsonPropertyName("svn_url")]
    public string? SvnUrl { get; set; }

    [JsonPropertyName("homepage")]
    public object Homepage { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int? StargazersCount { get; set; }

    [JsonPropertyName("watchers_count")]
    public int? WatchersCount { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("has_issues")]
    public bool? HasIssues { get; set; }

    [JsonPropertyName("has_projects")]
    public bool? HasProjects { get; set; }

    [JsonPropertyName("has_downloads")]
    public bool? HasDownloads { get; set; }

    [JsonPropertyName("has_wiki")]
    public bool? HasWiki { get; set; }

    [JsonPropertyName("has_pages")]
    public bool? HasPages { get; set; }

    [JsonPropertyName("has_discussions")]
    public bool? HasDiscussions { get; set; }

    [JsonPropertyName("forks_count")]
    public int? ForksCount { get; set; }

    [JsonPropertyName("mirror_url")]
    public object MirrorUrl { get; set; }

    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    [JsonPropertyName("open_issues_count")]
    public int? OpenIssuesCount { get; set; }

    [JsonPropertyName("license")]
    public License License { get; set; }

    [JsonPropertyName("allow_forking")]
    public bool? AllowForking { get; set; }

    [JsonPropertyName("is_template")]
    public bool? IsTemplate { get; set; }

    [JsonPropertyName("web_commit_signoff_required")]
    public bool? WebCommitSignoffRequired { get; set; }

    [JsonPropertyName("topics")]
    public List<object> Topics { get; set; }

    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }

    [JsonPropertyName("forks")]
    public int? Forks { get; set; }

    [JsonPropertyName("open_issues")]
    public int? OpenIssues { get; set; }

    [JsonPropertyName("watchers")]
    public int? Watchers { get; set; }

    [JsonPropertyName("default_branch")]
    public string? DefaultBranch { get; set; }

    [JsonPropertyName("stargazers")]
    public int? Stargazers { get; set; }

    [JsonPropertyName("master_branch")]
    public string? MasterBranch { get; set; }
}

public class Sender
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public string? GravatarId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public string? FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public string? FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public string? GistsUrl { get; set; }

    [JsonPropertyName("starred_url")]
    public string? StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public string? SubscriptionsUrl { get; set; }

    [JsonPropertyName("organizations_url")]
    public string? OrganizationsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public string? ReposUrl { get; set; }

    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public string? ReceivedEventsUrl { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("user_view_type")]
    public string? UserViewType { get; set; }

    [JsonPropertyName("site_admin")]
    public bool? SiteAdmin { get; set; }
}


