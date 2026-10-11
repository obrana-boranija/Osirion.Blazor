using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// A dedicated contact information section component that displays contact details
/// such as address, phone, email, and website with consistent styling and accessibility.
/// </summary>
public partial class OsirionContactInfoSection(NavigationManager navigationManager) : OsirionComponentBase
{
    /// <summary>
    /// Gets or sets the title for the contact information section.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Contact information";

    /// <summary>
    /// Gets or sets the contact information to display.
    /// </summary>
    [Parameter, EditorRequired]
    public ContactInformation? ContactInfo { get; set; }

    /// <summary>
    /// Gets or sets the label text for the address field.
    /// </summary>
    [Parameter]
    public string AddressLabel { get; set; } = "Address:";

    /// <summary>
    /// Gets or sets the label text for the phone field.
    /// </summary>
    [Parameter]
    public string PhoneLabel { get; set; } = "Phone:";

    /// <summary>
    /// Gets or sets the label text for the email field.
    /// </summary>
    [Parameter]
    public string EmailLabel { get; set; } = "Email:";

    /// <summary>
    /// Gets or sets the label text for the website field.
    /// </summary>
    [Parameter]
    public string WebsiteLabel { get; set; } = "Website:";

    private IReadOnlyList<WebsiteLink> _websiteLinks = [];

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _websiteLinks = Websites()
            .Select(CreateWebsiteLink)
            .OfType<WebsiteLink>()
            .ToList();
    }

    #region Helper Methods

    /// <summary>
    /// A website entry that passed validation: only http and https addresses with a host become links.
    /// </summary>
    private sealed record WebsiteLink(string Href, string Target, string DisplayText);

    /// <summary>
    /// Writes text that may carry intentional markup (a paragraph or line break) through the registered sanitizer.
    /// </summary>
    private MarkupString Sanitized(string? html) => string.IsNullOrWhiteSpace(html)
        ? default
        : new MarkupString(ServiceProvider.GetHtmlContentSanitizer().Sanitize(html));

    private WebsiteLink? CreateWebsiteLink(string entry)
    {
        var href = entry;
        if (!Uri.TryCreate(entry, UriKind.Absolute, out var uri) || !IsWebAddress(uri))
        {
            // A value such as "example.org" or "example.org:8080" has no scheme. A value that already names another
            // scheme (javascript:, data:) does not parse as a host and is dropped.
            if (entry.Contains("//", StringComparison.Ordinal)
                || !Uri.TryCreate("https://" + entry, UriKind.Absolute, out uri)
                || !IsWebAddress(uri))
            {
                return null;
            }

            href = uri.AbsoluteUri;
        }

        if (IsCurrentSite(uri))
        {
            return new WebsiteLink(uri.PathAndQuery + uri.Fragment, "_self", uri.Authority);
        }

        return new WebsiteLink(href, "_blank", uri.Authority);
    }

    private static bool IsWebAddress(Uri uri) =>
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && !string.IsNullOrEmpty(uri.Host);

    /// <summary>
    /// Compares the host, not a string prefix: "https://site.com.evil.net" starts with "https://site.com" but is another site.
    /// </summary>
    private bool IsCurrentSite(Uri uri) =>
        Uri.TryCreate(navigationManager.BaseUri, UriKind.Absolute, out var baseUri)
        && string.Equals(uri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase);

    private string GetContactInfoSectionClass()
    {
        var classes = new List<string> { "osirion-contact-info-section" };

        return CombineCssClasses(string.Join(" ", classes));
    }

    private string[] Phones()
    {
        if (string.IsNullOrWhiteSpace(ContactInfo?.Phone))
        {
            return Array.Empty<string>();
        }

        return ContactInfo.Phone.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private string[] Emails()
    {
        if (string.IsNullOrWhiteSpace(ContactInfo?.Email))
        {
            return Array.Empty<string>();
        }
        return ContactInfo.Email.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private string[] Websites()
    {
        if (string.IsNullOrWhiteSpace(ContactInfo?.Website))
        {
            return Array.Empty<string>();
        }
        return ContactInfo.Website.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    #endregion
}
