namespace Osirion.Blazor.Core.Services;

/// <summary>
/// Removes markup that can run script or take over the page from HTML that comes from content,
/// such as rendered markdown, before it is written to the page as raw markup.
/// </summary>
/// <remarks>
/// Implementations must be thread-safe: one instance is shared by every render and every content refresh.
/// </remarks>
public interface IHtmlContentSanitizer
{
    /// <summary>
    /// Sanitizes an HTML body fragment against the configured allow-list.
    /// </summary>
    /// <param name="html">The HTML fragment to sanitize. May be <see langword="null"/>.</param>
    /// <returns>
    /// The sanitized fragment, or <see cref="string.Empty"/> when <paramref name="html"/> is
    /// <see langword="null"/>, empty or white space.
    /// </returns>
    string Sanitize(string? html);
}
