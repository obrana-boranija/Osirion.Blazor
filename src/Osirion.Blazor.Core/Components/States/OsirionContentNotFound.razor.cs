using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Component for displaying "content not found" states with customizable messaging and actions
/// </summary>
public partial class OsirionContentNotFound
{
    /// <summary>
    /// Gets or sets the error code (e.g., "404")
    /// </summary>
    [Parameter]
    public string? ErrorCode { get; set; } = "404";

    /// <summary>
    /// Gets or sets the main title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Page Not Found";

    /// <summary>
    /// Gets or sets the descriptive message
    /// </summary>
    [Parameter]
    public string Message { get; set; } = "The page you're looking for doesn't exist or has been moved.";

    /// <summary>
    /// Gets or sets whether to show an icon
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to use the default icon when no custom icon is provided
    /// </summary>
    [Parameter]
    public bool UseDefaultIcon { get; set; } = true;

    /// <summary>
    /// Gets or sets custom icon content
    /// </summary>
    [Parameter]
    public RenderFragment? Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon image URL
    /// </summary>
    [Parameter]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Gets or sets the icon image alt text
    /// </summary>
    [Parameter]
    public string IconAlt { get; set; } = "Not Found";

    /// <summary>
    /// Gets or sets the primary button text
    /// </summary>
    [Parameter]
    public string? PrimaryButtonText { get; set; } = "Go to Homepage";

    /// <summary>
    /// Gets or sets the primary button URL
    /// </summary>
    [Parameter]
    public string PrimaryButtonUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the secondary button text
    /// </summary>
    [Parameter]
    public string? SecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the secondary button URL
    /// </summary>
    [Parameter]
    public string SecondaryButtonUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets custom action buttons content
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Gets or sets whether to show suggestions
    /// </summary>
    [Parameter]
    public bool ShowSuggestions { get; set; } = false;

    /// <summary>
    /// Gets or sets the suggestions title
    /// </summary>
    [Parameter]
    public string SuggestionsTitle { get; set; } = "Here are some helpful links:";

    /// <summary>
    /// Gets or sets the list of suggested links
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentNotFoundSuggestion>? Suggestions { get; set; }

    /// <summary>
    /// Gets or sets whether to show a search box
    /// </summary>
    [Parameter]
    public bool ShowSearchBox { get; set; } = false;

    /// <summary>
    /// Gets or sets the search box content
    /// </summary>
    [Parameter]
    public RenderFragment? SearchContent { get; set; }

    /// <summary>
    /// Gets or sets whether to show contact information
    /// </summary>
    [Parameter]
    public bool ShowContactInfo { get; set; } = false;

    /// <summary>
    /// Gets or sets the contact message
    /// </summary>
    [Parameter]
    public string ContactMessage { get; set; } = "Need help? Contact us at";

    /// <summary>
    /// Gets or sets the contact email
    /// </summary>
    [Parameter]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the component variant: "default", "minimal", "centered", "hero"
    /// </summary>
    [Parameter]
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Gets or sets the component size: "small", "medium", "large", "full"
    /// </summary>
    [Parameter]
    public string Size { get; set; } = "medium";

    /// <summary>
    /// Gets or sets the background pattern
    /// </summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the CSS class for the component
    /// </summary>
    private string GetContentNotFoundClass()
    {
        var classes = new List<string> { "osirion-content-not-found" };

        classes.Add($"osirion-content-not-found-{Variant}");
        classes.Add($"osirion-content-not-found-{Size}");

        if (!string.IsNullOrWhiteSpace(ErrorCode))
        {
            classes.Add("osirion-content-not-found-with-code");
        }

        if (BackgroundPattern is not null)
        {
            classes.Add("osirion-content-not-found-with-background");
        }

        if (!string.IsNullOrWhiteSpace(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Creates a default 404 configuration
    /// </summary>
    public static OsirionContentNotFound Create404()
    {
        return new OsirionContentNotFound
        {
            ErrorCode = "404",
            Title = "Page Not Found",
            Message = "The page you're looking for doesn't exist or has been moved.",
            PrimaryButtonText = "Go to Homepage",
            PrimaryButtonUrl = "/"
        };
    }

    /// <summary>
    /// Creates a default 403 configuration
    /// </summary>
    public static OsirionContentNotFound Create403()
    {
        return new OsirionContentNotFound
        {
            ErrorCode = "403",
            Title = "Access Denied",
            Message = "You don't have permission to access this resource.",
            PrimaryButtonText = "Go Back",
            PrimaryButtonUrl = "javascript:history.back()"
        };
    }

    /// <summary>
    /// Creates a default empty search results configuration
    /// </summary>
    public static OsirionContentNotFound CreateEmptySearch(string searchTerm)
    {
        return new OsirionContentNotFound
        {
            Title = "No Results Found",
            Message = $"We couldn't find any results for \"{searchTerm}\". Try adjusting your search or browse our categories.",
            ShowSearchBox = true,
            UseDefaultIcon = true
        };
    }

    /// <summary>
    /// Creates a default empty content configuration
    /// </summary>
    public static OsirionContentNotFound CreateEmptyContent()
    {
        return new OsirionContentNotFound
        {
            Title = "No Content Available",
            Message = "There's no content to display at the moment. Please check back later.",
            ShowIcon = true,
            UseDefaultIcon = true
        };
    }
}

/// <summary>
/// Represents a content suggestion link
/// </summary>
public class ContentNotFoundSuggestion
{
    /// <summary>
    /// Gets or sets the suggestion text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the suggestion URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the suggestion description (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets target. Default "_self".
    /// </summary>
    public string? Target { get; set; } = "_self";
}