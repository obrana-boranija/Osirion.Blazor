using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Simplified webhook handler for GitHub events
/// </summary>
public class GitHubWebhookHandler : IGitHubWebhookHandler
{
    private readonly IContentProviderManager _providerManager;
    private readonly IGitHubApiClientFactory _apiClientFactory;
    private readonly ILogger<GitHubWebhookHandler> _logger;

    public GitHubWebhookHandler(
        IContentProviderManager providerManager,
        IGitHubApiClientFactory apiClientFactory,
        ILogger<GitHubWebhookHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleWebhookAsync(HttpRequest request)
    {
        try
        {
            var eventType = request.Headers["X-GitHub-Event"].ToString();
            var signature = request.Headers["X-Hub-Signature-256"].ToString();

            string payload;
            using (var reader = new StreamReader(request.Body))
            {
                payload = await reader.ReadToEndAsync();
                //request.Body.Position = 0; // Reset for other middleware
            }

            _logger.LogInformation("Received GitHub webhook: {EventType}", eventType);

            return eventType switch
            {
                "ping" => true, // Just acknowledge ping
                "push" => await HandlePushEventAsync(payload, signature),
                _ => true // Ignore other events
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return false;
        }
    }

    public Task<bool> ProcessWebhookAsync(string eventType, string signature, string payload)
    {
        return Task.FromResult(true);
    }

    private async Task<bool> HandlePushEventAsync(string payload, string signature)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            var owner = root.GetProperty("repository").GetProperty("owner").GetProperty("login").GetString();
            var repo = root.GetProperty("repository").GetProperty("name").GetString();
            var branch = root.GetProperty("ref").GetString()?.Split('/').LastOrDefault();
            var commitSha = root.GetProperty("after").GetString();

            _logger.LogInformation("Push event: {Owner}/{Repo} branch {Branch} commit {Sha}",
                owner, repo, branch, commitSha);

            string? matchingProvider = null;
            foreach (var providerName in _apiClientFactory.GetProviderNames())
            {
                var options = _apiClientFactory.GetProviderOptions(providerName);
                if (options is not null &&
                    string.Equals(options.Owner, owner, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(options.Repository, repo, StringComparison.OrdinalIgnoreCase))
                {
                    // Validate signature if configured
                    //if (!string.IsNullOrWhiteSpace(options.WebhookSecret))
                    //{
                    //    if (!ValidateSignature(signature, payload, options.WebhookSecret))
                    //    {
                    //        _logger.LogWarning("Invalid webhook signature for {Provider}", providerName);
                    //        return false;
                    //    }
                    //}

                    // Check branch
                    if (!string.IsNullOrWhiteSpace(options.Branch) && branch != options.Branch)
                    {
                        _logger.LogInformation("Ignoring push to branch {Branch}, expected {Expected}",
                            branch, options.Branch);
                        return true;
                    }

                    matchingProvider = options.ProviderId;
                    break;
                }
            }

            if (matchingProvider is null)
            {
                _logger.LogWarning("No provider found for {Owner}/{Repo}", owner, repo);
                return false;
            }

            var providers = _providerManager.GetAllProviders()
                .OfType<IContentCacheUpdater>()
                .Where(p => p.ProviderId.Contains(matchingProvider));

            foreach (var provider in providers)
            {
                _logger.LogInformation("Updating cache for provider {ProviderId}", provider.ProviderId);
                await provider.UpdateCacheAsync(commitSha, true);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling push event");
            return false;
        }
    }

    private bool ValidateSignature(string signature, string payload, string secret)
    {
        if (string.IsNullOrWhiteSpace(signature) || !signature.StartsWith("sha256="))
            return false;

        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(secretBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        var expected = "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return signature.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Model for GitHub ping event
/// </summary>
public class GitHubPingEvent
{
    [JsonPropertyName("zen")]
    public string? Zen { get; set; }

    [JsonPropertyName("hook_id")]
    public int HookId { get; set; }

    [JsonPropertyName("hook")]
    public GitHubWebhook? Hook { get; set; }

    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    [JsonPropertyName("sender")]
    public Sender? Sender { get; set; }
}

/// <summary>
/// Model for GitHub webhook configuration
/// </summary>
public class GitHubWebhook
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("events")]
    public List<string>? Events { get; set; }

    [JsonPropertyName("config")]
    public GitHubWebhookConfig? Config { get; set; }
}

/// <summary>
/// Model for GitHub webhook configuration details
/// </summary>
public class GitHubWebhookConfig
{
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("insecure_ssl")]
    public string? InsecureSsl { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Model for GitHub repository events
/// </summary>
public class GitHubRepositoryEvent
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    [JsonPropertyName("sender")]
    public Sender? Sender { get; set; }
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


