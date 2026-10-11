namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Encodes the caller supplied parts of GitHub REST API URLs, so that a path, branch, owner or repository
/// name can never change which endpoint, repository or ref a request targets.
/// </summary>
/// <remarks>
/// Every segment is percent-encoded with <see cref="Uri.EscapeDataString(string)"/>, which leaves ASCII
/// letters, digits and the characters <c>-</c>, <c>_</c>, <c>.</c> and <c>~</c> unchanged, so URLs built
/// from such values are byte-identical to the unencoded form. Dot segments are rejected instead of
/// encoded: escaping leaves <c>..</c> unchanged and <see cref="Uri"/> would collapse it, retargeting the
/// request.
/// </remarks>
internal static class GitHubApiPath
{
    private const char Separator = '/';
    private const char WindowsSeparator = '\\';

    /// <summary>
    /// Encodes a repository path for the <c>contents</c> endpoint. Leading and trailing slashes are trimmed,
    /// and an empty path stays empty so the repository root can still be listed.
    /// </summary>
    /// <remarks>
    /// A backslash is read as a separator, as <see cref="Uri"/> did when earlier versions built these URLs, so a
    /// path combined with <c>System.IO.Path</c> on Windows still addresses the same folder. The dot segment
    /// checks run on the normalized segments.
    /// </remarks>
    /// <param name="path">The repository path, using <c>/</c> or <c>\</c> between segments.</param>
    /// <param name="parameterName">The parameter name reported when the path is rejected.</param>
    /// <returns>The encoded path, with <c>/</c> kept between segments.</returns>
    /// <exception cref="ArgumentException">The path contains an empty, <c>.</c> or <c>..</c> segment.</exception>
    public static string EncodePath(string? path, string parameterName)
    {
        var trimmed = path?.Replace(WindowsSeparator, Separator).Trim(Separator);
        return string.IsNullOrEmpty(trimmed)
            ? string.Empty
            : EncodeSegments(trimmed, parameterName);
    }

    /// <summary>
    /// Encodes a branch name used inside a URL path, such as <c>branches/{branch}</c>. Slashes in the branch
    /// name are kept, so <c>feature/x</c> still addresses the branch of that name.
    /// </summary>
    /// <param name="branch">The branch name.</param>
    /// <param name="parameterName">The parameter name reported when the branch name is rejected.</param>
    /// <returns>The encoded branch name.</returns>
    /// <exception cref="ArgumentException">The branch name is empty or contains an empty, <c>.</c> or <c>..</c> segment.</exception>
    public static string EncodeBranchPath(string? branch, string parameterName)
    {
        if (string.IsNullOrEmpty(branch))
        {
            throw new ArgumentException("A branch name is required.", parameterName);
        }

        return EncodeSegments(branch, parameterName);
    }

    /// <summary>
    /// Encodes a value that must stay a single URL path segment, such as a repository owner or name.
    /// </summary>
    /// <param name="segment">The segment value.</param>
    /// <param name="parameterName">The parameter name reported when the value is rejected.</param>
    /// <returns>The encoded segment.</returns>
    /// <exception cref="ArgumentException">The value is empty, <c>.</c> or <c>..</c>.</exception>
    public static string EncodeSegment(string? segment, string parameterName)
    {
        EnsureValidSegment(segment, parameterName);
        return Uri.EscapeDataString(segment!);
    }

    /// <summary>
    /// Encodes a query string value, such as the <c>ref</c> parameter. Slashes are legal in a query and are
    /// kept, so a branch such as <c>feature/x</c> produces the same URL as before.
    /// </summary>
    /// <param name="value">The query value.</param>
    /// <returns>The encoded value.</returns>
    public static string EncodeQueryValue(string value)
    {
        return string.Join(Separator, value.Split(Separator).Select(Uri.EscapeDataString));
    }

    private static string EncodeSegments(string value, string parameterName)
    {
        var segments = value.Split(Separator);
        for (var index = 0; index < segments.Length; index++)
        {
            EnsureValidSegment(segments[index], parameterName);
            segments[index] = Uri.EscapeDataString(segments[index]);
        }

        return string.Join(Separator, segments);
    }

    private static void EnsureValidSegment(string? segment, string parameterName)
    {
        if (string.IsNullOrEmpty(segment) || segment is "." or "..")
        {
            throw new ArgumentException(
                "GitHub API path segments must not be empty, '.' or '..'.",
                parameterName);
        }
    }
}
