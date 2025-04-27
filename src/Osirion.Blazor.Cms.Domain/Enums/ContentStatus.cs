namespace Osirion.Blazor.Cms.Domain.Enums;

/// <summary>
/// Defines the status of a content item
/// </summary>
public enum ContentStatus
{
    /// <summary>
    /// Content is a draft and not publicly visible
    /// </summary>
    Draft,

    /// <summary>
    /// Content is published and publicly visible
    /// </summary>
    Published,

    /// <summary>
    /// Content is scheduled to be published at a future date
    /// </summary>
    Scheduled,

    /// <summary>
    /// Content is archived and may have limited visibility
    /// </summary>
    Archived,

    /// <summary>
    /// Content is in review and not publicly visible
    /// </summary>
    InReview
}