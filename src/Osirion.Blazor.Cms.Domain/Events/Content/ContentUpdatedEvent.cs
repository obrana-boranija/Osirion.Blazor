namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Event raised when a content item is updated
/// </summary>
public class ContentUpdatedEvent : DomainEvent
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
    /// Gets the path of the content item
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the provider ID that updated the content
    /// </summary>
    public string ProviderId { get; }

    public ContentUpdatedEvent(string contentId, string title, string path, string providerId)
    {
        ContentId = contentId ?? throw new ArgumentNullException(nameof(contentId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }
}