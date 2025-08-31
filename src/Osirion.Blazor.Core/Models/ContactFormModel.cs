using System.ComponentModel.DataAnnotations;

namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Model for contact form data with validation attributes.
/// </summary>
public class ContactFormModel
{
    /// <summary>
    /// Gets or sets the sender's name.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message subject.
    /// </summary>
    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 5000 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the sender agrees to privacy policy.
    /// </summary>
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the privacy policy")]
    public bool AgreeToPrivacyPolicy { get; set; }

    /// <summary>
    /// Gets or sets whether the sender wants to receive updates (optional).
    /// </summary>
    public bool SubscribeToUpdates { get; set; }

    /// <summary>
    /// Validation property for honeypot anti-spam protection.
    /// Should remain empty for legitimate submissions.
    /// </summary>
    public string Website { get; set; } = string.Empty;
}