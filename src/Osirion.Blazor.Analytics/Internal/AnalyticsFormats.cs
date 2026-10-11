using System.Text.RegularExpressions;

namespace Osirion.Blazor.Analytics.Internal;

/// <summary>
/// Format checks for analytics configuration values whose shape is defined by the vendor.
/// </summary>
internal static partial class AnalyticsFormats
{
    /// <summary>Google tag IDs: <c>G-</c> (GA4 measurement ID) or <c>GT-</c> (Google tag) followed by alphanumerics.</summary>
    [GeneratedRegex("^GT?-[A-Za-z0-9]+$", RegexOptions.CultureInvariant)]
    private static partial Regex GoogleTagIdPattern();

    /// <summary>Yandex Metrica counter IDs are plain integers.</summary>
    [GeneratedRegex("^[0-9]{1,18}$", RegexOptions.CultureInvariant)]
    private static partial Regex YandexCounterIdPattern();

    public static bool IsGoogleTagId(string? value) =>
        !string.IsNullOrEmpty(value) && GoogleTagIdPattern().IsMatch(value);

    public static bool IsYandexCounterId(string? value) =>
        !string.IsNullOrEmpty(value) && YandexCounterIdPattern().IsMatch(value);

    /// <summary>An absolute http or https URL.</summary>
    public static bool IsHttpUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// An http(s) or protocol-relative URL, or a path on the site itself such as <c>/js/script.js</c>, the shape a
    /// script or endpoint takes when a same-site proxy serves it.
    /// </summary>
    public static bool IsHttpOrSiteRelativeUrl(string? value) =>
        IsHttpOrProtocolRelativeUrl(value)
        || (value is ['/', not '/' and not '\\', ..] && Uri.TryCreate(value, UriKind.Relative, out _));

    /// <summary>An absolute http(s) URL, or a protocol-relative URL such as <c>//analytics.example.com/</c>.</summary>
    public static bool IsHttpOrProtocolRelativeUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.StartsWith("//", StringComparison.Ordinal) && !value.StartsWith("///", StringComparison.Ordinal)
            ? Uri.TryCreate("https:" + value, UriKind.Absolute, out _)
            : IsHttpUrl(value);
    }
}
