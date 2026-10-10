using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Handles GitHub webhook deliveries and refreshes the content cache of the providers a push affects.
/// </summary>
/// <remarks>
/// <para>
/// Every delivery must carry an <c>X-Hub-Signature-256</c> header that verifies against the
/// <see cref="GitHubOptions.WebhookSecret"/> of at least one configured GitHub provider. Deliveries
/// are rejected when no provider has a webhook secret, when the signature is missing or wrong, and
/// when the body is larger than 5 MiB. The payload is not parsed or logged before it is verified.
/// </para>
/// <para>
/// A verified push refreshes every provider whose owner, repository and branch match the push and
/// whose own secret verified the signature. Pushes to tags, deleted branches and untracked branches
/// are acknowledged without a refresh.
/// </para>
/// </remarks>
public class GitHubWebhookHandler : IGitHubWebhookHandler
{
    /// <summary>
    /// The largest webhook body, in bytes, that the handler reads.
    /// </summary>
    internal const int MaxPayloadBytes = 5 * 1024 * 1024;

    private const string EventHeaderName = "X-GitHub-Event";
    private const string DeliveryHeaderName = "X-GitHub-Delivery";
    private const string BranchRefPrefix = "refs/heads/";
    private const int ReadBufferSize = 81920;

    private readonly IContentProviderManager _providerManager;
    private readonly IGitHubApiClientFactory _apiClientFactory;
    private readonly ILogger<GitHubWebhookHandler> _logger;

    /// <summary>Initializes a GitHub webhook handler.</summary>
    public GitHubWebhookHandler(
        IContentProviderManager providerManager,
        IGitHubApiClientFactory apiClientFactory,
        ILogger<GitHubWebhookHandler> logger)
    {
        _providerManager = providerManager ?? throw new ArgumentNullException(nameof(providerManager));
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Rejects the delivery without reading the body when no provider has a webhook secret or the
    /// signature header is missing or malformed. Otherwise reads at most 5 MiB of the body without
    /// disposing it, verifies the signature against the raw bytes, and only then dispatches the event.
    /// Returns <see langword="false"/> for a rejected or malformed delivery and <see langword="true"/>
    /// for a verified one, including verified deliveries that match no provider.
    /// </remarks>
    public async Task<bool> HandleWebhookAsync(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cancellationToken = request.HttpContext.RequestAborted;

        try
        {
            if (request.ContentLength > MaxPayloadBytes)
            {
                _logger.LogWarning("Rejected GitHub webhook: the declared body length exceeds {MaxPayloadBytes} bytes", MaxPayloadBytes);
                return false;
            }

            // Checks that need only configuration and headers run first, so an anonymous request
            // that cannot verify is refused before its body is buffered.
            var signatureCheck = PrepareSignatureCheck(request.Headers[GitHubWebhookSignature.HeaderName].ToString());
            if (signatureCheck is null)
            {
                return false;
            }

            var payload = await TryReadPayloadAsync(request.Body, cancellationToken);
            if (payload is null)
            {
                return false;
            }

            return await VerifyAndDispatchAsync(
                request.Headers[EventHeaderName].ToString(),
                request.Headers[DeliveryHeaderName].ToString(),
                signatureCheck,
                payload);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("GitHub webhook request was aborted before it was processed");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return false;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// The signature is verified against the UTF-8 encoding of <paramref name="payload"/>, so the
    /// payload must be the exact body GitHub sent. Unsigned or wrongly signed payloads are rejected.
    /// </remarks>
    public async Task<bool> ProcessWebhookAsync(string eventType, string signature, string payload)
    {
        try
        {
            var signatureCheck = PrepareSignatureCheck(signature);
            if (signatureCheck is null)
            {
                return false;
            }

            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            if (payloadBytes.Length > MaxPayloadBytes)
            {
                _logger.LogWarning("Rejected GitHub webhook: the body exceeds {MaxPayloadBytes} bytes", MaxPayloadBytes);
                return false;
            }

            return await VerifyAndDispatchAsync(eventType, deliveryId: null, signatureCheck, payloadBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return false;
        }
    }

    private SignatureCheck? PrepareSignatureCheck(string? signature)
    {
        var providersWithSecret = GetProvidersWithWebhookSecret();
        if (providersWithSecret.Count == 0)
        {
            _logger.LogWarning("Rejected GitHub webhook: no GitHub provider has a WebhookSecret configured");
            return null;
        }

        if (!GitHubWebhookSignature.TryParse(signature, out var digest))
        {
            _logger.LogWarning("Rejected GitHub webhook: the {HeaderName} header is missing or malformed", GitHubWebhookSignature.HeaderName);
            return null;
        }

        return new SignatureCheck(providersWithSecret, digest);
    }

    private async Task<bool> VerifyAndDispatchAsync(string? eventType, string? deliveryId, SignatureCheck signatureCheck, byte[] payload)
    {
        var verifiedProviders = GetProvidersSignedBy(signatureCheck.ProvidersWithSecret, signatureCheck.Digest, payload);
        if (verifiedProviders.Count == 0)
        {
            _logger.LogWarning("Rejected GitHub webhook: the signature does not match any configured webhook secret");
            return false;
        }

        _logger.LogInformation("Received GitHub webhook {EventType} (delivery {DeliveryId})", eventType, deliveryId);

        return eventType switch
        {
            "push" => await HandlePushEventAsync(payload, verifiedProviders),
            _ => true // ping and other events are acknowledged without action
        };
    }

    private List<GitHubProviderOptions> GetProvidersWithWebhookSecret()
    {
        var providers = new List<GitHubProviderOptions>();
        foreach (var providerName in _apiClientFactory.GetProviderNames())
        {
            var options = _apiClientFactory.GetProviderOptions(providerName);
            if (options is not null && !string.IsNullOrWhiteSpace(options.WebhookSecret))
            {
                providers.Add(options);
            }
        }

        return providers;
    }

    private static List<GitHubProviderOptions> GetProvidersSignedBy(
        List<GitHubProviderOptions> providers,
        byte[] digest,
        byte[] payload)
    {
        var verifiedSecrets = providers
            .Select(options => options.WebhookSecret!)
            .Distinct(StringComparer.Ordinal)
            .Where(secret => GitHubWebhookSignature.Matches(digest, secret, payload))
            .ToHashSet(StringComparer.Ordinal);

        return providers
            .Where(options => verifiedSecrets.Contains(options.WebhookSecret!))
            .ToList();
    }

    private async Task<bool> HandlePushEventAsync(byte[] payload, List<GitHubProviderOptions> verifiedProviders)
    {
        if (!TryReadPushEvent(payload, out var push))
        {
            _logger.LogWarning("Rejected GitHub push webhook: the payload is not a JSON push event");
            return false;
        }

        if (push.Deleted || !push.Ref.StartsWith(BranchRefPrefix, StringComparison.Ordinal))
        {
            _logger.LogInformation("Ignoring push to {Ref} in {Owner}/{Repository}: not an updated branch",
                push.Ref, push.Owner, push.Repository);
            return true;
        }

        var branch = push.Ref[BranchRefPrefix.Length..];
        var providerIds = verifiedProviders
            .Where(options => TracksBranch(options, push.Owner, push.Repository, branch))
            .Select(GitHubProviderId.For)
            .ToHashSet(StringComparer.Ordinal);

        if (providerIds.Count == 0)
        {
            _logger.LogInformation("Ignoring push to {Owner}/{Repository} branch {Branch}: no provider tracks it",
                push.Owner, push.Repository, branch);
            return true;
        }

        _logger.LogInformation("Push event: {Owner}/{Repo} branch {Branch} commit {Sha}",
            push.Owner, push.Repository, branch, push.HeadSha);

        var cacheUpdaters = _providerManager.GetAllProviders()
            .OfType<IContentCacheUpdater>()
            .Where(updater => providerIds.Contains(updater.ProviderId));

        foreach (var cacheUpdater in cacheUpdaters)
        {
            _logger.LogInformation("Updating cache for provider {ProviderId}", cacheUpdater.ProviderId);
            await cacheUpdater.UpdateCacheAsync(push.HeadSha, true);
        }

        return true;
    }

    private static bool TracksBranch(GitHubOptions options, string owner, string repository, string branch)
    {
        // A provider without a branch follows the repository's default branch, which the push does not name.
        return string.Equals(options.Owner, owner, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(options.Repository, repository, StringComparison.OrdinalIgnoreCase) &&
               (string.IsNullOrWhiteSpace(options.Branch) || string.Equals(options.Branch, branch, StringComparison.Ordinal));
    }

    private async Task<byte[]?> TryReadPayloadAsync(Stream body, CancellationToken cancellationToken)
    {
        byte[]? payload;
        try
        {
            payload = await ReadPayloadAsync(body, cancellationToken);
        }
        catch (IOException ex)
        {
            // The server refuses a body it cannot accept, such as one past its own size limit or one
            // with broken chunked framing; Kestrel reports both as BadHttpRequestException, an IOException.
            _logger.LogWarning("Rejected GitHub webhook: the body could not be read ({Reason})", ex.Message);
            return null;
        }

        if (payload is null)
        {
            _logger.LogWarning("Rejected GitHub webhook: the body exceeds {MaxPayloadBytes} bytes", MaxPayloadBytes);
        }

        return payload;
    }

    private static async Task<byte[]?> ReadPayloadAsync(Stream body, CancellationToken cancellationToken)
    {
        using var payload = new MemoryStream();
        var buffer = new byte[ReadBufferSize];

        while (true)
        {
            // Reading one byte past the limit is enough to tell an oversized body from a full one.
            var remaining = MaxPayloadBytes + 1 - (int)payload.Length;
            var read = await body.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, remaining)), cancellationToken);
            if (read == 0)
            {
                return payload.ToArray();
            }

            payload.Write(buffer, 0, read);
            if (payload.Length > MaxPayloadBytes)
            {
                return null;
            }
        }
    }

    private static bool TryReadPushEvent(byte[] payload, out PushEvent push)
    {
        push = default;

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !TryGetString(root, "ref", out var gitRef) ||
                !root.TryGetProperty("repository", out var repository) ||
                repository.ValueKind != JsonValueKind.Object ||
                !TryGetString(repository, "name", out var repositoryName) ||
                !repository.TryGetProperty("owner", out var owner) ||
                owner.ValueKind != JsonValueKind.Object ||
                !TryGetString(owner, "login", out var ownerLogin))
            {
                return false;
            }

            TryGetString(root, "after", out var headSha);
            var deleted = root.TryGetProperty("deleted", out var deletedElement) &&
                          deletedElement.ValueKind == JsonValueKind.True;

            push = new PushEvent(ownerLogin, repositoryName, gitRef, headSha, deleted);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryGetString(JsonElement element, string propertyName, [NotNullWhen(true)] out string? value)
    {
        value = element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

        return !string.IsNullOrEmpty(value);
    }

    private readonly record struct PushEvent(string Owner, string Repository, string Ref, string? HeadSha, bool Deleted);

    private sealed record SignatureCheck(List<GitHubProviderOptions> ProvidersWithSecret, byte[] Digest);
}

/// <summary>
/// Model for GitHub ping event
/// </summary>
public class GitHubPingEvent
{
    /// <summary>Gets or sets GitHub's ping message.</summary>
    [JsonPropertyName("zen")]
    public string? Zen { get; set; }

    /// <summary>Gets or sets the webhook identifier.</summary>
    [JsonPropertyName("hook_id")]
    public int HookId { get; set; }

    /// <summary>Gets or sets the webhook configuration.</summary>
    [JsonPropertyName("hook")]
    public GitHubWebhook? Hook { get; set; }

    /// <summary>Gets or sets the repository associated with the webhook.</summary>
    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    /// <summary>Gets or sets the GitHub sender.</summary>
    [JsonPropertyName("sender")]
    public Sender? Sender { get; set; }
}

/// <summary>
/// Model for GitHub webhook configuration
/// </summary>
public class GitHubWebhook
{
    /// <summary>Gets or sets the webhook type.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the webhook identifier.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the webhook name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets whether the webhook is active.</summary>
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    /// <summary>Gets or sets the subscribed event names.</summary>
    [JsonPropertyName("events")]
    public List<string>? Events { get; set; }

    /// <summary>Gets or sets the webhook delivery configuration.</summary>
    [JsonPropertyName("config")]
    public GitHubWebhookConfig? Config { get; set; }
}

/// <summary>
/// Model for GitHub webhook configuration details
/// </summary>
public class GitHubWebhookConfig
{
    /// <summary>Gets or sets the webhook content type.</summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    /// <summary>Gets or sets the insecure SSL setting.</summary>
    [JsonPropertyName("insecure_ssl")]
    public string? InsecureSsl { get; set; }

    /// <summary>Gets or sets the webhook delivery URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Model for GitHub repository events
/// </summary>
public class GitHubRepositoryEvent
{
    /// <summary>Gets or sets the event action.</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>Gets or sets the repository associated with the event.</summary>
    [JsonPropertyName("repository")]
    public Repository? Repository { get; set; }

    /// <summary>Gets or sets the GitHub sender.</summary>
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

    /// <summary>Gets or sets the pusher information.</summary>
    [JsonPropertyName("pusher")]
    public Pusher? Pusher { get; set; }

    /// <summary>Gets or sets the GitHub sender.</summary>
    [JsonPropertyName("sender")]
    public Sender? Sender { get; set; }

    /// <summary>Gets or sets whether the push created a reference.</summary>
    [JsonPropertyName("created")]
    public bool? Created { get; set; }

    /// <summary>Gets or sets whether the push deleted a reference.</summary>
    [JsonPropertyName("deleted")]
    public bool? Deleted { get; set; }

    /// <summary>Gets or sets whether the push was forced.</summary>
    [JsonPropertyName("forced")]
    public bool? Forced { get; set; }

    /// <summary>Gets or sets the base reference.</summary>
    [JsonPropertyName("base_ref")]
    public object? BaseRef { get; set; }

    /// <summary>Gets or sets the comparison URL.</summary>
    [JsonPropertyName("compare")]
    public string? Compare { get; set; }

    /// <summary>Gets or sets the commits included in the push.</summary>
    [JsonPropertyName("commits")]
    public List<Commit>? Commits { get; set; }

    /// <summary>Gets or sets the head commit.</summary>
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
/// <summary>Represents the author information in a GitHub commit.</summary>
public class Author
{
    /// <summary>Gets or sets the author's name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the author's email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Gets or sets the author's GitHub username.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

/// <summary>Represents a commit in a GitHub push payload.</summary>
public class Commit
{
    /// <summary>Gets or sets the commit identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the tree identifier.</summary>
    [JsonPropertyName("tree_id")]
    public string? TreeId { get; set; }

    /// <summary>Gets or sets whether the commit is distinct.</summary>
    [JsonPropertyName("distinct")]
    public bool? Distinct { get; set; }

    /// <summary>Gets or sets the commit message.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>Gets or sets the commit timestamp.</summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>Gets or sets the commit URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the commit author.</summary>
    [JsonPropertyName("author")]
    public Author? Author { get; set; }

    /// <summary>Gets or sets the commit committer.</summary>
    [JsonPropertyName("committer")]
    public Committer? Committer { get; set; }

    /// <summary>Gets or sets files added by the commit.</summary>
    [JsonPropertyName("added")]
    public List<object>? Added { get; set; }

    /// <summary>Gets or sets files removed by the commit.</summary>
    [JsonPropertyName("removed")]
    public List<object>? Removed { get; set; }

    /// <summary>Gets or sets files modified by the commit.</summary>
    [JsonPropertyName("modified")]
    public List<string>? Modified { get; set; }
}

/// <summary>Represents the committer information in a GitHub commit.</summary>
public class Committer
{
    /// <summary>Gets or sets the committer's name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the committer's email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Gets or sets the committer's GitHub username.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

/// <summary>Represents the head commit in a GitHub push payload.</summary>
public class HeadCommit
{
    /// <summary>Gets or sets the commit identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the tree identifier.</summary>
    [JsonPropertyName("tree_id")]
    public string? TreeId { get; set; }

    /// <summary>Gets or sets whether the commit is distinct.</summary>
    [JsonPropertyName("distinct")]
    public bool? Distinct { get; set; }

    /// <summary>Gets or sets the commit message.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>Gets or sets the Timestamp value.</summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>Gets or sets the commit URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the commit author.</summary>
    [JsonPropertyName("author")]
    public Author? Author { get; set; }

    /// <summary>Gets or sets the commit committer.</summary>
    [JsonPropertyName("committer")]
    public Committer? Committer { get; set; }

    /// <summary>Gets or sets files added by the commit.</summary>
    [JsonPropertyName("added")]
    public List<object>? Added { get; set; }

    /// <summary>Gets or sets files removed by the commit.</summary>
    [JsonPropertyName("removed")]
    public List<object>? Removed { get; set; }

    /// <summary>Gets or sets files modified by the commit.</summary>
    [JsonPropertyName("modified")]
    public List<string>? Modified { get; set; }
}

/// <summary>Represents license metadata in a GitHub repository.</summary>
public class License
{
    /// <summary>Gets or sets the license key.</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>Gets or sets the license name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the SPDX identifier.</summary>
    [JsonPropertyName("spdx_id")]
    public string? SpdxId { get; set; }

    /// <summary>Gets or sets the license URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the GitHub node identifier.</summary>
    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }
}

/// <summary>Represents a GitHub user owning a repository.</summary>
public class Owner
{
    /// <summary>Gets or sets the user's display name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the user's email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Gets or sets the user's login.</summary>
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    /// <summary>Gets or sets the user's identifier.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>Gets or sets the user's node identifier.</summary>
    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    /// <summary>Gets or sets the avatar URL.</summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>Gets or sets the Gravatar identifier.</summary>
    [JsonPropertyName("gravatar_id")]
    public string? GravatarId { get; set; }

    /// <summary>Gets or sets the API URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the HTML profile URL.</summary>
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    /// <summary>Gets or sets the FollowersUrl value.</summary>
    [JsonPropertyName("followers_url")]
    public string? FollowersUrl { get; set; }

    /// <summary>Gets or sets the following API URL.</summary>
    [JsonPropertyName("following_url")]
    public string? FollowingUrl { get; set; }

    /// <summary>Gets or sets the gists API URL.</summary>
    [JsonPropertyName("gists_url")]
    public string? GistsUrl { get; set; }

    /// <summary>Gets or sets the starred repositories API URL.</summary>
    [JsonPropertyName("starred_url")]
    public string? StarredUrl { get; set; }

    /// <summary>Gets or sets the subscriptions API URL.</summary>
    [JsonPropertyName("subscriptions_url")]
    public string? SubscriptionsUrl { get; set; }

    /// <summary>Gets or sets the organizations API URL.</summary>
    [JsonPropertyName("organizations_url")]
    public string? OrganizationsUrl { get; set; }

    /// <summary>Gets or sets the repositories API URL.</summary>
    [JsonPropertyName("repos_url")]
    public string? ReposUrl { get; set; }

    /// <summary>Gets or sets the events API URL.</summary>
    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    /// <summary>Gets or sets the received events API URL.</summary>
    [JsonPropertyName("received_events_url")]
    public string? ReceivedEventsUrl { get; set; }

    /// <summary>Gets or sets the user type.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the user view type.</summary>
    [JsonPropertyName("user_view_type")]
    public string? UserViewType { get; set; }

    /// <summary>Gets or sets whether the user is a site administrator.</summary>
    [JsonPropertyName("site_admin")]
    public bool? SiteAdmin { get; set; }
}

/// <summary>Represents the pusher in a GitHub push payload.</summary>
public class Pusher
{
    /// <summary>Gets or sets the pusher's name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the pusher's email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

/// <summary>Represents repository metadata from a GitHub webhook payload.</summary>
public class Repository
{
    /// <summary>Gets or sets the repository identifier.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>Gets or sets the repository node identifier.</summary>
    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    /// <summary>Gets or sets the repository name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the full repository name.</summary>
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    /// <summary>Gets or sets whether the repository is private.</summary>
    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    /// <summary>Gets or sets the repository owner.</summary>
    [JsonPropertyName("owner")]
    public Owner? Owner { get; set; }

    /// <summary>Gets or sets the repository HTML URL.</summary>
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    /// <summary>Gets or sets the repository description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets whether the repository is a fork.</summary>
    [JsonPropertyName("fork")]
    public bool? Fork { get; set; }

    /// <summary>Gets or sets the repository API URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the forks API URL.</summary>
    [JsonPropertyName("forks_url")]
    public string? ForksUrl { get; set; }

    /// <summary>Gets or sets the deploy keys API URL.</summary>
    [JsonPropertyName("keys_url")]
    public string? KeysUrl { get; set; }

    /// <summary>Gets or sets the collaborators API URL.</summary>
    [JsonPropertyName("collaborators_url")]
    public string? CollaboratorsUrl { get; set; }

    /// <summary>Gets or sets the teams API URL.</summary>
    [JsonPropertyName("teams_url")]
    public string? TeamsUrl { get; set; }

    /// <summary>Gets or sets the hooks API URL.</summary>
    [JsonPropertyName("hooks_url")]
    public string? HooksUrl { get; set; }

    /// <summary>Gets or sets the issue events API URL.</summary>
    [JsonPropertyName("issue_events_url")]
    public string? IssueEventsUrl { get; set; }

    /// <summary>Gets or sets the events API URL.</summary>
    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    /// <summary>Gets or sets the assignees API URL.</summary>
    [JsonPropertyName("assignees_url")]
    public string? AssigneesUrl { get; set; }

    /// <summary>Gets or sets the branches API URL.</summary>
    [JsonPropertyName("branches_url")]
    public string? BranchesUrl { get; set; }

    /// <summary>Gets or sets the tags API URL.</summary>
    [JsonPropertyName("tags_url")]
    public string? TagsUrl { get; set; }

    /// <summary>Gets or sets the blobs API URL.</summary>
    [JsonPropertyName("blobs_url")]
    public string? BlobsUrl { get; set; }

    /// <summary>Gets or sets the Git tags API URL.</summary>
    [JsonPropertyName("git_tags_url")]
    public string? GitTagsUrl { get; set; }

    /// <summary>Gets or sets the Git references API URL.</summary>
    [JsonPropertyName("git_refs_url")]
    public string? GitRefsUrl { get; set; }

    /// <summary>Gets or sets the trees API URL.</summary>
    [JsonPropertyName("trees_url")]
    public string? TreesUrl { get; set; }

    /// <summary>Gets or sets the statuses API URL.</summary>
    [JsonPropertyName("statuses_url")]
    public string? StatusesUrl { get; set; }

    /// <summary>Gets or sets the LanguagesUrl value.</summary>
    [JsonPropertyName("languages_url")]
    public string? LanguagesUrl { get; set; }

    /// <summary>Gets or sets the stargazers API URL.</summary>
    [JsonPropertyName("stargazers_url")]
    public string? StargazersUrl { get; set; }

    /// <summary>Gets or sets the contributors API URL.</summary>
    [JsonPropertyName("contributors_url")]
    public string? ContributorsUrl { get; set; }

    /// <summary>Gets or sets the subscribers API URL.</summary>
    [JsonPropertyName("subscribers_url")]
    public string? SubscribersUrl { get; set; }

    /// <summary>Gets or sets the subscription API URL.</summary>
    [JsonPropertyName("subscription_url")]
    public string? SubscriptionUrl { get; set; }

    /// <summary>Gets or sets the commits API URL.</summary>
    [JsonPropertyName("commits_url")]
    public string? CommitsUrl { get; set; }

    /// <summary>Gets or sets the Git commits API URL.</summary>
    [JsonPropertyName("git_commits_url")]
    public string? GitCommitsUrl { get; set; }

    /// <summary>Gets or sets the comments API URL.</summary>
    [JsonPropertyName("comments_url")]
    public string? CommentsUrl { get; set; }

    /// <summary>Gets or sets the issue comment API URL.</summary>
    [JsonPropertyName("issue_comment_url")]
    public string? IssueCommentUrl { get; set; }

    /// <summary>Gets or sets the contents API URL.</summary>
    [JsonPropertyName("contents_url")]
    public string? ContentsUrl { get; set; }

    /// <summary>Gets or sets the compare API URL.</summary>
    [JsonPropertyName("compare_url")]
    public string? CompareUrl { get; set; }

    /// <summary>Gets or sets the merges API URL.</summary>
    [JsonPropertyName("merges_url")]
    public string? MergesUrl { get; set; }

    /// <summary>Gets or sets the archive API URL.</summary>
    [JsonPropertyName("archive_url")]
    public string? ArchiveUrl { get; set; }

    /// <summary>Gets or sets the downloads API URL.</summary>
    [JsonPropertyName("downloads_url")]
    public string? DownloadsUrl { get; set; }

    /// <summary>Gets or sets the issues API URL.</summary>
    [JsonPropertyName("issues_url")]
    public string? IssuesUrl { get; set; }

    /// <summary>Gets or sets the pull requests API URL.</summary>
    [JsonPropertyName("pulls_url")]
    public string? PullsUrl { get; set; }

    /// <summary>Gets or sets the milestones API URL.</summary>
    [JsonPropertyName("milestones_url")]
    public string? MilestonesUrl { get; set; }

    /// <summary>Gets or sets the notifications API URL.</summary>
    [JsonPropertyName("notifications_url")]
    public string? NotificationsUrl { get; set; }

    /// <summary>Gets or sets the labels API URL.</summary>
    [JsonPropertyName("labels_url")]
    public string? LabelsUrl { get; set; }

    /// <summary>Gets or sets the releases API URL.</summary>
    [JsonPropertyName("releases_url")]
    public string? ReleasesUrl { get; set; }

    /// <summary>Gets or sets the deployments API URL.</summary>
    [JsonPropertyName("deployments_url")]
    public string? DeploymentsUrl { get; set; }

    /// <summary>Gets or sets the repository creation timestamp.</summary>
    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; set; }

    /// <summary>Gets or sets the repository update timestamp.</summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Gets or sets the repository push timestamp.</summary>
    [JsonPropertyName("pushed_at")]
    public int? PushedAt { get; set; }

    /// <summary>Gets or sets the Git URL.</summary>
    [JsonPropertyName("git_url")]
    public string? GitUrl { get; set; }

    /// <summary>Gets or sets the SSH URL.</summary>
    [JsonPropertyName("ssh_url")]
    public string? SshUrl { get; set; }

    /// <summary>Gets or sets the clone URL.</summary>
    [JsonPropertyName("clone_url")]
    public string? CloneUrl { get; set; }

    /// <summary>Gets or sets the Subversion URL.</summary>
    [JsonPropertyName("svn_url")]
    public string? SvnUrl { get; set; }

    /// <summary>Gets or sets the repository homepage.</summary>
    [JsonPropertyName("homepage")]
    public object? Homepage { get; set; }

    /// <summary>Gets or sets the repository size.</summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>Gets or sets the stargazer count.</summary>
    [JsonPropertyName("stargazers_count")]
    public int? StargazersCount { get; set; }

    /// <summary>Gets or sets the watcher count.</summary>
    [JsonPropertyName("watchers_count")]
    public int? WatchersCount { get; set; }

    /// <summary>Gets or sets the primary repository language.</summary>
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    /// <summary>Gets or sets whether issues are enabled.</summary>
    [JsonPropertyName("has_issues")]
    public bool? HasIssues { get; set; }

    /// <summary>Gets or sets whether projects are enabled.</summary>
    [JsonPropertyName("has_projects")]
    public bool? HasProjects { get; set; }

    /// <summary>Gets or sets whether downloads are enabled.</summary>
    [JsonPropertyName("has_downloads")]
    public bool? HasDownloads { get; set; }

    /// <summary>Gets or sets whether the wiki is enabled.</summary>
    [JsonPropertyName("has_wiki")]
    public bool? HasWiki { get; set; }

    /// <summary>Gets or sets whether pages are enabled.</summary>
    [JsonPropertyName("has_pages")]
    public bool? HasPages { get; set; }

    /// <summary>Gets or sets whether discussions are enabled.</summary>
    [JsonPropertyName("has_discussions")]
    public bool? HasDiscussions { get; set; }

    /// <summary>Gets or sets the ForksCount value.</summary>
    [JsonPropertyName("forks_count")]
    public int? ForksCount { get; set; }

    /// <summary>Gets or sets the mirror URL.</summary>
    [JsonPropertyName("mirror_url")]
    public object? MirrorUrl { get; set; }

    /// <summary>Gets or sets whether the repository is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets whether the repository is disabled.</summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    /// <summary>Gets or sets the open issue count.</summary>
    [JsonPropertyName("open_issues_count")]
    public int? OpenIssuesCount { get; set; }

    /// <summary>Gets or sets the repository license.</summary>
    [JsonPropertyName("license")]
    public License? License { get; set; }

    /// <summary>Gets or sets whether forking is allowed.</summary>
    [JsonPropertyName("allow_forking")]
    public bool? AllowForking { get; set; }

    /// <summary>Gets or sets whether the repository is a template.</summary>
    [JsonPropertyName("is_template")]
    public bool? IsTemplate { get; set; }

    /// <summary>Gets or sets whether web commit signoff is required.</summary>
    [JsonPropertyName("web_commit_signoff_required")]
    public bool? WebCommitSignoffRequired { get; set; }

    /// <summary>Gets or sets repository topics.</summary>
    [JsonPropertyName("topics")]
    public List<object>? Topics { get; set; }

    /// <summary>Gets or sets the repository visibility.</summary>
    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }

    /// <summary>Gets or sets the fork count.</summary>
    [JsonPropertyName("forks")]
    public int? Forks { get; set; }

    /// <summary>Gets or sets the open issue count.</summary>
    [JsonPropertyName("open_issues")]
    public int? OpenIssues { get; set; }

    /// <summary>Gets or sets the watcher count.</summary>
    [JsonPropertyName("watchers")]
    public int? Watchers { get; set; }

    /// <summary>Gets or sets the default branch name.</summary>
    [JsonPropertyName("default_branch")]
    public string? DefaultBranch { get; set; }

    /// <summary>Gets or sets the stargazer count.</summary>
    [JsonPropertyName("stargazers")]
    public int? Stargazers { get; set; }

    /// <summary>Gets or sets the legacy master branch name.</summary>
    [JsonPropertyName("master_branch")]
    public string? MasterBranch { get; set; }
}

/// <summary>Represents the sender in a GitHub webhook payload.</summary>
public class Sender
{
    /// <summary>Gets or sets the sender login.</summary>
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    /// <summary>Gets or sets the sender identifier.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>Gets or sets the sender node identifier.</summary>
    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    /// <summary>Gets or sets the sender avatar URL.</summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>Gets or sets the sender Gravatar identifier.</summary>
    [JsonPropertyName("gravatar_id")]
    public string? GravatarId { get; set; }

    /// <summary>Gets or sets the sender API URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the sender HTML profile URL.</summary>
    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    /// <summary>Gets or sets the FollowersUrl value.</summary>
    [JsonPropertyName("followers_url")]
    public string? FollowersUrl { get; set; }

    /// <summary>Gets or sets the FollowingUrl value.</summary>
    [JsonPropertyName("following_url")]
    public string? FollowingUrl { get; set; }

    /// <summary>Gets or sets the GistsUrl value.</summary>
    [JsonPropertyName("gists_url")]
    public string? GistsUrl { get; set; }

    /// <summary>Gets or sets the StarredUrl value.</summary>
    [JsonPropertyName("starred_url")]
    public string? StarredUrl { get; set; }

    /// <summary>Gets or sets the SubscriptionsUrl value.</summary>
    [JsonPropertyName("subscriptions_url")]
    public string? SubscriptionsUrl { get; set; }

    /// <summary>Gets or sets the OrganizationsUrl value.</summary>
    [JsonPropertyName("organizations_url")]
    public string? OrganizationsUrl { get; set; }

    /// <summary>Gets or sets the ReposUrl value.</summary>
    [JsonPropertyName("repos_url")]
    public string? ReposUrl { get; set; }

    /// <summary>Gets or sets the EventsUrl value.</summary>
    [JsonPropertyName("events_url")]
    public string? EventsUrl { get; set; }

    /// <summary>Gets or sets the ReceivedEventsUrl value.</summary>
    [JsonPropertyName("received_events_url")]
    public string? ReceivedEventsUrl { get; set; }

    /// <summary>Gets or sets the Type value.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the UserViewType value.</summary>
    [JsonPropertyName("user_view_type")]
    public string? UserViewType { get; set; }

    /// <summary>Gets or sets the SiteAdmin value.</summary>
    [JsonPropertyName("site_admin")]
    public bool? SiteAdmin { get; set; }
}


