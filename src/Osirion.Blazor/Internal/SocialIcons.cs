using System.Collections.Frozen;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Internal;

/// <summary>
/// Icons for the social profiles <see cref="Components.OsirionSiteLayout"/> links in its footer, drawn on a 24 by 24
/// grid in the text color.
/// </summary>
internal static class SocialIcons
{
    private static readonly FrozenDictionary<string, string> Paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["github"] = "M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z",
        ["x"] = "M18.901 1.153h3.68l-8.04 9.19L24 22.846h-7.406l-5.8-7.584-6.638 7.584H.474l8.6-9.83L0 1.154h7.594l5.243 6.932ZM17.61 20.644h2.039L6.486 3.24H4.298Z",
        ["linkedin"] = "M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433c-1.144 0-2.063-.926-2.063-2.065 0-1.138.92-2.063 2.063-2.063 1.14 0 2.064.925 2.064 2.063 0 1.139-.925 2.065-2.064 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z",
        ["nuget"] = "M17.67 21.29a3.28 3.28 0 01-3.33 3.21H3.09a3.08 3.08 0 01-3.09-3V8.26A3.29 3.29 0 013.33 5h11.25a3.09 3.09 0 013.09 3.09v13.2zm2.33-10.6H8.75a3.09 3.09 0 00-3.09 3.09v11.25a3.29 3.29 0 003.33 3.26h11.25a3.08 3.08 0 003.09-3V13.79a3.28 3.28 0 00-3.33-3.1z",
        ["rss"] = "M19.199 24C19.199 13.467 10.533 4.8 0 4.8V0c13.165 0 24 10.835 24 24h-4.801zM3.291 17.415c1.814 0 3.293 1.479 3.293 3.295 0 1.813-1.485 3.29-3.301 3.29C1.47 24 0 22.526 0 20.71s1.475-3.294 3.291-3.295zM15.909 24h-4.665c0-6.169-5.075-11.245-11.244-11.245V8.09c8.727 0 15.909 7.184 15.909 15.91z",
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["github"] = "GitHub",
        ["x"] = "X",
        ["linkedin"] = "LinkedIn",
        ["nuget"] = "NuGet",
        ["rss"] = "RSS feed",
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the display name of <paramref name="network"/>, or the value itself for an unknown network.</summary>
    public static string NameOf(string network) => Names.TryGetValue(network, out var name) ? name : network;

    /// <summary>
    /// Gets the icon of <paramref name="network"/>, or <see langword="null"/> for a network without one, which the
    /// footer then shows as text.
    /// </summary>
    public static RenderFragment? For(string? network)
    {
        if (network is null || !Paths.TryGetValue(network, out var path))
        {
            return null;
        }

        var svg = $"<svg class=\"osirion-site-social-icon\" viewBox=\"0 0 24 24\" width=\"20\" height=\"20\" fill=\"currentColor\" aria-hidden=\"true\" focusable=\"false\"><path d=\"{path}\" /></svg>";
        return builder => builder.AddMarkupContent(0, svg);
    }
}
