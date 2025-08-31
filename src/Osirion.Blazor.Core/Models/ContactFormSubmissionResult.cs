namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Represents the result of a contact form submission.
/// </summary>
public enum ContactFormSubmissionResult
{
    /// <summary>
    /// The form has not been submitted yet.
    /// </summary>
    None,

    /// <summary>
    /// The form submission was successful.
    /// </summary>
    Success,

    /// <summary>
    /// The form submission failed due to validation errors.
    /// </summary>
    ValidationError,

    /// <summary>
    /// The form submission failed due to a server error.
    /// </summary>
    ServerError,

    /// <summary>
    /// The form submission was rejected due to spam detection.
    /// </summary>
    SpamDetected,

    /// <summary>
    /// The form submission is currently being processed.
    /// </summary>
    Processing
}