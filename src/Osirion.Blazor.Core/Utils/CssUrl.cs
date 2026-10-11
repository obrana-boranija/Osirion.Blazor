using System.Globalization;
using System.Text;

namespace Osirion.Blazor.Core.Utils;

/// <summary>
/// Writes untrusted URLs into inline CSS.
/// </summary>
internal static class CssUrl
{
    /// <summary>
    /// Writes <paramref name="url"/> as a quoted CSS <c>url()</c> value.
    /// </summary>
    /// <remarks>
    /// The URL can come from content front matter (<c>featured_image</c>) or a component parameter. Quotes,
    /// backslashes, parentheses and control characters are written as CSS escapes, so the value cannot close the
    /// string and add declarations of its own, such as a full-page overlay, while the browser still reads the
    /// original URL. Other URLs are written unchanged.
    /// </remarks>
    /// <param name="url">The URL to write.</param>
    /// <returns>A <c>url('...')</c> expression that is safe inside a style attribute.</returns>
    internal static string Format(string url)
    {
        var css = new StringBuilder("url('", url.Length + 7);

        foreach (var character in url)
        {
            if (character is '\'' or '"' or '\\' or '(' or ')' || char.IsControl(character))
            {
                // A CSS hex escape; the space ends it, so a hex digit that follows is not read as part of it
                css.Append('\\').Append(((int)character).ToString("X", CultureInfo.InvariantCulture)).Append(' ');
            }
            else
            {
                css.Append(character);
            }
        }

        return css.Append("')").ToString();
    }
}
