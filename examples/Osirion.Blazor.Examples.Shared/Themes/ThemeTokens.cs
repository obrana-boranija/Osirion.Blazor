namespace Osirion.Blazor.Examples.Shared.Themes;

/// <summary>A color token of the Osirion themes, as the theme page lists it.</summary>
/// <param name="Name">The token without its <c>--osirion-</c> prefix, such as <c>text-primary</c>.</param>
/// <param name="Colors">What the token colors.</param>
/// <param name="ReadsOn">The tokens it must stand out from, with the ratio, or <see langword="null"/> when it has no pair.</param>
public sealed record ThemeToken(string Name, string Colors, string? ReadsOn = null)
{
    /// <summary>The custom property, such as <c>--osirion-text-primary</c>.</summary>
    public string Property => "--osirion-" + Name;
}

/// <summary>A group of tokens in the reference table.</summary>
/// <param name="Title">The group's heading.</param>
/// <param name="Tokens">The tokens, in table order.</param>
public sealed record ThemeTokenGroup(string Title, IReadOnlyList<ThemeToken> Tokens);

/// <summary>
/// The color tokens every theme and palette sets, with the pairs the library's contrast tests hold them to (text pairs
/// 4.5:1, focus indicators 3:1). A test checks that both theme files declare each one.
/// </summary>
public static class ThemeTokens
{
    private const string OnPage = "4.5:1 on background-primary and background-secondary";

    /// <summary>The groups, in page order.</summary>
    public static IReadOnlyList<ThemeTokenGroup> Groups { get; } =
    [
        new("Surfaces",
        [
            new("background-primary", "The page"),
            new("background-secondary", "Raised surfaces: cards, sidebars, the table of contents"),
            new("background-tertiary", "Subtle fills on raised surfaces"),
            new("background-muted", "Pressed and disabled controls"),
            new("background-hover", "Hovered navigation items"),
            new("background-inverted", "Surfaces in the opposite theme"),
        ]),
        new("Text",
        [
            new("text-primary", "Body text and headings", "4.5:1 on background-primary, background-secondary and background-tertiary"),
            new("text-secondary", "Descriptions, metadata and other supporting text", OnPage),
            new("text-tertiary", "Quiet labels and placeholders", OnPage),
            new("text-disabled", "Disabled controls"),
            new("text-inverted", "Text on inverted surfaces"),
        ]),
        new("Actions and links",
        [
            new("action-primary", "Primary buttons"),
            new("action-primary-hover", "Primary buttons under the pointer"),
            new("action-primary-text", "Labels on primary buttons", "4.5:1 on action-primary and action-primary-hover"),
            new("action-secondary", "Secondary buttons"),
            new("action-secondary-text", "Labels on secondary buttons", "4.5:1 on action-secondary and action-secondary-hover"),
            new("link-color", "Links", OnPage),
            new("link-hover-color", "Links under the pointer"),
            new("link-visited-color", "Visited links"),
        ]),
        new("Borders and focus",
        [
            new("border-color", "Borders and dividers"),
            new("border-color-hover", "Borders under the pointer"),
            new("border-color-focus", "Borders of focused controls", "3:1 on background-primary"),
            new("focus-ring-color", "The focus ring", "3:1 on background-primary"),
        ]),
        new("Navigation",
        [
            new("nav-text", "Navigation links"),
            new("nav-active-text", "The current page in navigation", "4.5:1 on nav-active-background"),
            new("nav-active-background", "Behind the current page"),
            new("nav-hover-background", "Behind a link under the pointer"),
        ]),
        new("Status",
        [
            new("status-success-text", "Success messages", "4.5:1 on status-success-bg"),
            new("status-success-bg", "Behind success messages"),
            new("status-warning-text", "Warnings", "4.5:1 on status-warning-bg"),
            new("status-warning-bg", "Behind warnings"),
            new("status-danger-text", "Errors", "4.5:1 on status-danger-bg"),
            new("status-danger-bg", "Behind errors"),
            new("status-info-text", "Information", "4.5:1 on status-info-bg"),
            new("status-info-bg", "Behind information"),
        ]),
    ];
}
