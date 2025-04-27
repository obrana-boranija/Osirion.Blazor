namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Command to delete a directory
/// </summary>
public class DeleteDirectoryCommand : ICommand
{
    /// <summary>
    /// Gets or sets the ID of the directory to delete
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to recursively delete subdirectories and files
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the commit message for versioned providers
    /// </summary>
    public string? CommitMessage { get; set; }
}