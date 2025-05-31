using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Cookie consent banner component for GDPR compliance (SSR-compatible)
/// </summary>
public partial class OsirionCookieConsent
{
    private const string ConsentCookieName = "osirion_cookie_consent";
    private const string ConsentCookieVersion = "1.0";

    /// <summary>
    /// Gets or sets the consent title
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Cookie Consent";

    /// <summary>
    /// Gets or sets the consent message
    /// </summary>
    [Parameter]
    public string Message { get; set; } = "We use cookies to improve your experience on our website. By continuing to browse, you agree to our use of cookies.";

    /// <summary>
    /// Gets or sets the accept button text
    /// </summary>
    [Parameter]
    public string AcceptButtonText { get; set; } = "Accept All";

    /// <summary>
    /// Gets or sets the decline button text
    /// </summary>
    [Parameter]
    public string DeclineButtonText { get; set; } = "Decline";

    /// <summary>
    /// Gets or sets the customize button text
    /// </summary>
    [Parameter]
    public string CustomizeButtonText { get; set; } = "Customize";

    /// <summary>
    /// Gets or sets the save preferences button text
    /// </summary>
    [Parameter]
    public string SavePreferencesButtonText { get; set; } = "Save Preferences";

    /// <summary>
    /// Gets or sets whether to show the decline button
    /// </summary>
    [Parameter]
    public bool ShowDeclineButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the customize button
    /// </summary>
    [Parameter]
    public bool ShowCustomizeButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the customization panel
    /// </summary>
    [Parameter]
    public bool ShowCustomizationPanel { get; set; } = true;

    /// <summary>
    /// Gets or sets the customize panel title
    /// </summary>
    [Parameter]
    public string CustomizePanelTitle { get; set; } = "Cookie Preferences";

    /// <summary>
    /// Gets or sets the privacy policy link
    /// </summary>
    [Parameter]
    public string? PolicyLink { get; set; }

    /// <summary>
    /// Gets or sets the privacy policy link text
    /// </summary>
    [Parameter]
    public string PolicyLinkText { get; set; } = "Learn more";

    /// <summary>
    /// Gets or sets the icon content
    /// </summary>
    [Parameter]
    public RenderFragment? Icon { get; set; }

    /// <summary>
    /// Gets or sets the cookie categories
    /// </summary>
    [Parameter]
    public IReadOnlyList<CookieCategory> Categories { get; set; } = GetDefaultCategories();

    /// <summary>
    /// Gets or sets the consent endpoint for form submission
    /// </summary>
    [Parameter]
    public string ConsentEndpoint { get; set; } = "/api/cookie-consent";

    /// <summary>
    /// Gets or sets the position: "bottom", "top"
    /// </summary>
    [Parameter]
    public string Position { get; set; } = "bottom";

    /// <summary>
    /// Gets or sets the theme: "light", "dark", "auto"
    /// </summary>
    [Parameter]
    public string Theme { get; set; } = "auto";

    /// <summary>
    /// Gets or sets the consent expiry in days
    /// </summary>
    [Parameter]
    public int ConsentExpiryDays { get; set; } = 365;

    /// <summary>
    /// Gets or sets whether the banner should be shown
    /// </summary>
    private bool ShowBanner { get; set; }

    /// <summary>
    /// Gets or sets whether the customization panel is shown
    /// </summary>
    private bool IsCustomizing { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        CheckConsentStatus();
    }

    /// <summary>
    /// Checks if consent has already been given
    /// </summary>
    private void CheckConsentStatus()
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var consentCookie = httpContext.Request.Cookies[ConsentCookieName];
            ShowBanner = string.IsNullOrEmpty(consentCookie);

            // Check if we're in customize mode from query string
            if (httpContext.Request.Query.ContainsKey("customize-cookies"))
            {
                IsCustomizing = true;
            }
        }
    }

    /// <summary>
    /// Gets the current URL for the return URL field
    /// </summary>
    private string GetCurrentUrl()
    {
        return Navigation.Uri;
    }

    /// <summary>
    /// Gets the CSS class for the cookie consent
    /// </summary>
    private string GetCookieConsentClass()
    {
        var classes = new List<string> { "osirion-cookie-consent" };

        classes.Add($"osirion-cookie-consent-{Position}");
        classes.Add($"osirion-cookie-consent-theme-{Theme}");

        if (IsCustomizing)
        {
            classes.Add("osirion-cookie-consent-customizing");
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets default cookie categories
    /// </summary>
    private static IReadOnlyList<CookieCategory> GetDefaultCategories()
    {
        return new[]
        {
            new CookieCategory
            {
                Id = "necessary",
                Name = "Necessary",
                Description = "These cookies are essential for the website to function properly.",
                IsRequired = true,
                IsEnabled = true
            },
            new CookieCategory
            {
                Id = "analytics",
                Name = "Analytics",
                Description = "These cookies help us understand how visitors interact with our website.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "marketing",
                Name = "Marketing",
                Description = "These cookies are used to show you relevant ads based on your interests.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "preferences",
                Name = "Preferences",
                Description = "These cookies remember your settings and preferences.",
                IsRequired = false,
                IsEnabled = false
            }
        };
    }
}

/// <summary>
/// Represents a cookie category
/// </summary>
public class CookieCategory
{
    /// <summary>
    /// Gets or sets the category ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this category is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets whether this category is enabled
    /// </summary>
    public bool IsEnabled { get; set; }
}