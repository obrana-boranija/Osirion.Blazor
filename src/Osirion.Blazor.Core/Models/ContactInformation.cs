namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Model representing contact information to display alongside the contact form.
/// </summary>
public class ContactInformation
{
    /// <summary>
    /// Gets or sets the physical address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the website URL.
    /// </summary>
    public string Website { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional message or description.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}