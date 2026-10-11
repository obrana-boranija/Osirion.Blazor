namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Thrown when GitHub answers 403 or 429 because the primary or a secondary rate limit was exceeded.
/// </summary>
public sealed class GitHubRateLimitException : HttpRequestException
{
    /// <summary>Initializes the exception.</summary>
    /// <param name="resetAt">When GitHub allows requests again, if it said so.</param>
    /// <param name="statusCode">The status code of the response.</param>
    public GitHubRateLimitException(DateTimeOffset? resetAt, System.Net.HttpStatusCode statusCode)
        : base(
            resetAt is null
                ? "GitHub rate limit exceeded."
                : $"GitHub rate limit exceeded; requests are allowed again at {resetAt:u}.",
            inner: null,
            statusCode)
    {
        ResetAt = resetAt;
    }

    /// <summary>Gets when GitHub allows requests again, or <see langword="null"/> when the response did not say.</summary>
    public DateTimeOffset? ResetAt { get; }
}
