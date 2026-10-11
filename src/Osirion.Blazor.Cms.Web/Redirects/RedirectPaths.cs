using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Web.Redirects;

/// <summary>
/// Normalizes the paths the redirect map is keyed by and checks redirect targets.
/// </summary>
internal static class RedirectPaths
{
    /// <summary>
    /// Returns <paramref name="path"/> as <c>/segment/segment</c> (leading slash, no trailing slash, no query string or
    /// fragment), or <see langword="null"/> when it is not a path on the site or is the site root, which never moves.
    /// </summary>
    public static string? Normalize(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var trimmed = path.Trim();
        var cut = trimmed.IndexOfAny(['?', '#']);
        if (cut >= 0)
        {
            trimmed = trimmed[..cut];
        }

        if (!trimmed.StartsWith('/'))
        {
            // "docs/old" is a site path written without its slash; "https://..." or "mailto:..." is not a path at all.
            if (Uri.TryCreate(trimmed, UriKind.Absolute, out var absolute) && !string.IsNullOrEmpty(absolute.Scheme)
                && trimmed.StartsWith(absolute.Scheme + ":", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            trimmed = "/" + trimmed;
        }

        trimmed = trimmed.TrimEnd('/');
        return trimmed.Length > 1 && LocalUrl.IsLocalPath(trimmed) ? trimmed : null;
    }

    /// <summary>Gets whether <paramref name="target"/> is a path on the site or an absolute http or https URL.</summary>
    public static bool IsValidTarget(string? target) =>
        LocalUrl.IsLocalPath(target)
        || (Uri.TryCreate(target, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp));
}
