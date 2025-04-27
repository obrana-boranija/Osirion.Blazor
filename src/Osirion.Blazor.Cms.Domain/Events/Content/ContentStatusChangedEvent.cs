using Osirion.Blazor.Cms.Domain.Enums;

namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Event raised when a content item's status is changed
/// </summary>
public class ContentStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the content item
    /// </summary>
    public string ContentId { get; }

    /// <summary>
    /// Gets the title of the content item
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the previous status
    /// </summary>
    public ContentStatus PreviousStatus { get; }

    /// <summary>
    /// Gets the new status
    /// </summary>
    public ContentStatus NewStatus { get; }

    /// <summary>
    /// Gets the provider ID of the content
    /// </summary>
    public string ProviderId { get; }

    public ContentStatusChangedEvent(
        string contentId,
        string title,
        ContentStatus previousStatus,
        ContentStatus newStatus,
        string providerId)
    {
        ContentId = contentId ?? throw new ArgumentNullException(nameof(contentId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }
}