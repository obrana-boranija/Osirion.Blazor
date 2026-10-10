using System.Net;
using System.Text.Json;

namespace Osirion.Blazor.Analytics.Internal;

/// <summary>
/// Encodes configuration values for the contexts the tracker markup places them in.
/// </summary>
internal static class ScriptLiteral
{
    /// <summary>
    /// Returns <paramref name="value"/> as a quoted JavaScript string literal. The default
    /// <see cref="System.Text.Json"/> encoder escapes <c>&lt;</c>, <c>&gt;</c>, <c>&amp;</c> and <c>'</c>,
    /// so the result can neither close the surrounding script element nor break out of the literal.
    /// </summary>
    public static string Js(string? value) => JsonSerializer.Serialize(value ?? string.Empty);

    /// <summary>Encodes <paramref name="value"/> for use inside a double-quoted HTML attribute.</summary>
    public static string Attribute(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
