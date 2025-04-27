namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Command to move a directory to a new parent
/// </summary>
public class MoveDirectoryCommand : ICommand
{
    /// <summary>
    /// Gets or sets the ID of the directory to move
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the new parent directory (null for root)
    /// </summary>
    public string? NewParentId { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the commit message for versioned providers
    /// </summary>
    public string? CommitMessage { get; set; }
}