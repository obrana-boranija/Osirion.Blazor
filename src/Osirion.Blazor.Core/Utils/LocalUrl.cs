namespace Osirion.Blazor.Core.Utils;

/// <summary>
/// Validates return URLs so that redirects never leave the current site.
/// </summary>
/// <remarks>
/// Follows the rules ASP.NET Core applies in <c>IUrlHelper.IsLocalUrl</c> for root-relative paths.
/// A value is local only when it starts with a single <c>/</c>, its second character is not
/// <c>/</c> or <c>\</c> (browsers treat both as a protocol-relative URL to another host), and it
/// contains no control characters (browsers strip tabs and newlines, which can turn
/// <c>/\t/evil.example</c> into <c>//evil.example</c>). Absolute URLs are never local, even when
/// they point at the current host, because the host header is attacker-controlled.
/// </remarks>
public static class LocalUrl
{
    /// <summary>
    /// The path used when a return URL is missing or not local.
    /// </summary>
    public const string DefaultPath = "/";

    /// <summary>
    /// Determines whether <paramref name="url"/> is a root-relative path on the current site.
    /// </summary>
    /// <param name="url">The candidate URL, typically taken from a query string or form field.</param>
    /// <returns><see langword="true"/> when redirecting to <paramref name="url"/> stays on the current site.</returns>
    public static bool IsLocalPath(string? url)
    {
        if (string.IsNullOrEmpty(url) || url[0] != '/')
        {
            return false;
        }

        if (url.Length == 1)
        {
            return true;
        }

        if (url[1] is '/' or '\\')
        {
            return false;
        }

        foreach (var character in url.AsSpan(1))
        {
            if (char.IsControl(character))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <paramref name="url"/> when it is a local path, otherwise <paramref name="fallback"/>.
    /// </summary>
    /// <param name="url">The candidate URL, typically taken from a query string or form field.</param>
    /// <param name="fallback">The local path to use when <paramref name="url"/> is rejected.</param>
    /// <returns>A path that is safe to redirect to.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fallback"/> is not itself a local path.</exception>
    public static string GetSafeLocalPath(string? url, string fallback = DefaultPath)
    {
        if (!IsLocalPath(fallback))
        {
            throw new ArgumentException("The fallback must be a local path.", nameof(fallback));
        }

        return IsLocalPath(url) ? url! : fallback;
    }
}
