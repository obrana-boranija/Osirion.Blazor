using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Footer component for displaying site footer with links, social media, and copyright information
/// </summary>
public partial class OsirionFooter
{
    /// <summary>
    /// Gets or sets the company name for the copyright notice
    /// </summary>
    [Parameter]
    public string CompanyName { get; set; } = "Company";

    /// <summary>
    /// Gets or sets custom copyright text. If provided, overrides the default copyright format
    /// </summary>
    [Parameter]
    public string? Copyright { get; set; }

    /// <summary>
    /// Gets or sets whether to show the copyright notice
    /// </summary>
    [Parameter]
    public bool ShowCopyright { get; set; } = true;

    /// <summary>
    /// Gets or sets the logo content
    /// </summary>
    [Parameter]
    public RenderFragment? Logo { get; set; }

    /// <summary>
    /// Gets or sets the footer link sections
    /// </summary>
    [Parameter]
    public IReadOnlyList<FooterSection>? Links { get; set; }

    /// <summary>
    /// Gets or sets the social media links
    /// </summary>
    [Parameter]
    public IReadOnlyList<FooterSocialLink>? SocialLinks { get; set; }

    /// <summary>
    /// Gets or sets the bottom navigation links (Privacy Policy, Terms, etc.)
    /// </summary>
    [Parameter]
    public IReadOnlyList<FooterLink>? BottomLinks { get; set; }

    /// <summary>
    /// Gets or sets whether to show social links
    /// </summary>
    [Parameter]
    public bool ShowSocialLinks { get; set; } = true;

    /// <summary>
    /// Gets or sets the title for the social links section
    /// </summary>
    [Parameter]
    public string SocialTitle { get; set; } = "Follow Us";

    /// <summary>
    /// Gets or sets whether to show the divider between sections
    /// </summary>
    [Parameter]
    public bool ShowDivider { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the top section
    /// </summary>
    [Parameter]
    public bool ShowTopSection { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the bottom section
    /// </summary>
    [Parameter]
    public bool ShowBottomSection { get; set; } = true;

    /// <summary>
    /// Gets or sets the footer variant: "default", "minimal", "centered"
    /// </summary>
    [Parameter]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Gets or sets the footer theme: "light", "dark", "auto"
    /// </summary>
    [Parameter]
    public string Theme { get; set; } = "auto";

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the CSS class for the footer
    /// </summary>
    private string GetFooterClass()
    {
        var classes = new List<string> { "osirion-footer" };

        classes.Add($"osirion-footer-{Variant}");
        classes.Add($"osirion-footer-theme-{Theme}");

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }
}

/// <summary>
/// Represents a footer section with links
/// </summary>
public class FooterSection
{
    /// <summary>
    /// Gets or sets the section title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the links in this section
    /// </summary>
    public IReadOnlyList<FooterLink> Links { get; set; } = Array.Empty<FooterLink>();
}

/// <summary>
/// Represents a footer link
/// </summary>
public class FooterLink
{
    /// <summary>
    /// Gets or sets the link text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the link URL
    /// </summary>
    public string Href { get; set; } = "#";

    /// <summary>
    /// Gets or sets whether to open in a new tab
    /// </summary>
    public bool OpenInNewTab { get; set; }
}

/// <summary>
/// Represents a social media link
/// </summary>
public class FooterSocialLink : FooterLink
{
    /// <summary>
    /// Gets or sets the icon content
    /// </summary>
    public RenderFragment? Icon { get; set; }

    /// <summary>
    /// Gets or sets the aria label for accessibility
    /// </summary>
    public string AriaLabel { get; set; } = string.Empty;
}