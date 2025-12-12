using Microsoft.AspNetCore.Components;
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

    #region Helper Methods

    private string GetWebsiteDisplayText(string website)
    {
        if (string.IsNullOrWhiteSpace(website))
            return string.Empty;

        var baseUrl = website.Replace("http://", "").Replace("https://", "").Split("/", StringSplitOptions.RemoveEmptyEntries);
        return baseUrl is not null ? baseUrl[0] : "/";
    }

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