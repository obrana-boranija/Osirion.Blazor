namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Event raised when a directory is deleted
/// </summary>
public class DirectoryDeletedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the directory
    /// </summary>
    public string DirectoryId { get; }

    /// <summary>
    /// Gets the path of the directory
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the provider ID that deleted the directory
    /// </summary>
    public string ProviderId { get; }

    /// <summary>
    /// Gets whether the directory was deleted recursively
    /// </summary>
    public bool Recursive { get; }

    public DirectoryDeletedEvent(string directoryId, string path, string providerId, bool recursive)
    {
        DirectoryId = directoryId ?? throw new ArgumentNullException(nameof(directoryId));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
        Recursive = recursive;
    }
}