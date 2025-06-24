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
    /// Gets or sets the company name for the copyright notice
    /// </summary>
    [Parameter]
    public string CompanyUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the company/site description shown below the logo
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

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
    /// Gets or sets additional content for the top section (e.g., newsletter form)
    /// </summary>
    [Parameter]
    public RenderFragment? TopContent { get; set; }

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
    /// Gets or sets the grid layout for desktop: "auto", "4-column", "3-column", "2-column"
    /// </summary>
    [Parameter]
    public string GridLayout { get; set; } = "auto";

    /// <summary>
    /// Gets or sets additional child content (shown at the very bottom)
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the footer should be docked to the bottom of the viewport
    /// </summary>
    [Parameter]
    public bool Docked { get; set; } = false;

    /// <summary>
    /// Gets or sets the docking mode: "fixed" or "sticky"
    /// Fixed: Always visible at bottom. Sticky: Sticks to bottom when page is short
    /// </summary>
    [Parameter]
    public string DockingMode { get; set; } = "sticky";

    /// <summary>
    /// Gets the CSS class for the footer
    /// </summary>
    private string GetFooterClass()
    {
        var classes = new List<string> { "osirion-footer" };

        classes.Add($"osirion-footer-{Variant}");
        //classes.Add($"osirion-footer-theme-{Theme}");
        classes.Add($"osirion-footer-grid-{GridLayout}");

        if (Docked)
        {
            classes.Add($"osirion-footer-docked-{DockingMode}");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
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