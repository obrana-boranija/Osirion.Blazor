using System.ComponentModel.DataAnnotations;

namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Model for subscription form data with validation attributes.
/// </summary>
public class SubscriptionModel
{
    /// <summary>
    /// Gets or sets the subscriber's name (optional).
    /// </summary>
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the subscriber's email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the subscriber agrees to privacy policy.
    /// </summary>
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the privacy policy")]
    public bool AgreeToPrivacyPolicy { get; set; }

    /// <summary>
    /// Gets or sets the subscription preferences (optional categories).
    /// </summary>
    public List<string> SubscriptionCategories { get; set; } = new();

    /// <summary>
    /// Validation property for honeypot anti-spam protection.
    /// Should remain empty for legitimate submissions.
    /// </summary>
    public string Website { get; set; } = string.Empty;
}