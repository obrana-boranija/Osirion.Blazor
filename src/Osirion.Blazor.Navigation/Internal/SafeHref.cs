namespace Osirion.Blazor.Navigation.Internal;

/// <summary>
/// Decides whether a link destination is safe to render in an <c>href</c> attribute.
/// </summary>
/// <remarks>
/// Menu data frequently comes from a CMS or configuration, so a value such as
/// <c>javascript:alert(1)</c> must never reach the page. Relative references and the schemes
/// <c>http</c>, <c>https</c>, <c>mailto</c> and <c>tel</c> are allowed; everything else is rejected.
/// Browsers ignore tabs, newlines and other control characters inside a URL, so the check removes them
/// before it looks at the scheme (<c>java\tscript:</c> is a script URL for a browser).
/// </remarks>
internal static class SafeHref
{
    private static readonly string[] AllowedSchemes = ["http", "https", "mailto", "tel"];

    /// <summary>
    /// Returns <paramref name="href"/> when it is safe to render, otherwise <see langword="null"/>.
    /// </summary>
    internal static string? Sanitize(string? href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        var normalized = Normalize(href);

        var delimiterIndex = normalized.IndexOfAny(['/', '?', '#']);
        var schemeEnd = normalized.IndexOf(':');
        var hasScheme = schemeEnd > 0 && (delimiterIndex < 0 || schemeEnd < delimiterIndex);
        if (!hasScheme)
        {
            // A reference without a scheme is relative (or protocol-relative), which cannot run script.
            return href;
        }

        var scheme = normalized[..schemeEnd];
        foreach (var allowed in AllowedSchemes)
        {
            if (string.Equals(scheme, allowed, StringComparison.OrdinalIgnoreCase))
            {
                return href;
            }
        }

        return null;
    }

    private static string Normalize(string value)
    {
        var buffer = new char[value.Length];
        var length = 0;
        foreach (var character in value)
        {
            if (!char.IsControl(character) && !char.IsWhiteSpace(character))
            {
                buffer[length++] = character;
            }
        }

        return new string(buffer, 0, length);
    }
}
