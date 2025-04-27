namespace Osirion.Blazor.Cms.Domain.Events;

/// <summary>
/// Event raised when a directory is created
/// </summary>
public class DirectoryCreatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the directory
    /// </summary>
    public string DirectoryId { get; }

    /// <summary>
    /// Gets the name of the directory
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path of the directory
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the provider ID that created the directory
    /// </summary>
    public string ProviderId { get; }

    public DirectoryCreatedEvent(string directoryId, string name, string path, string providerId)
    {
        DirectoryId = directoryId ?? throw new ArgumentNullException(nameof(directoryId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }
}