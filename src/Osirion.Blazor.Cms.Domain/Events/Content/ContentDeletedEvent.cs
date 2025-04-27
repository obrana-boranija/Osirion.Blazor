namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Event raised when a content item is deleted
/// </summary>
public class ContentDeletedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the content item
    /// </summary>
    public string ContentId { get; }

    /// <summary>
    /// Gets the path of the content item
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the provider ID that deleted the content
    /// </summary>
    public string ProviderId { get; }

    public ContentDeletedEvent(string contentId, string path, string providerId)
    {
        ContentId = contentId ?? throw new ArgumentNullException(nameof(contentId));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }
}