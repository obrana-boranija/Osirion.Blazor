namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Represents the result of a subscription form submission.
/// </summary>
public enum SubscriptionSubmissionResult
{
    /// <summary>
    /// No submission result.
    /// </summary>
    None = 0,

    /// <summary>
    /// Subscription was successful.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Validation errors occurred.
    /// </summary>
    ValidationError = 2,

    /// <summary>
    /// Server error occurred during submission.
    /// </summary>
    ServerError = 3,

    /// <summary>
    /// Spam was detected and submission was blocked.
    /// </summary>
    SpamDetected = 4,

    /// <summary>
    /// Subscription is being processed.
    /// </summary>
    Processing = 5,

    /// <summary>
    /// Email already exists in subscription list.
    /// </summary>
    AlreadySubscribed = 6
}