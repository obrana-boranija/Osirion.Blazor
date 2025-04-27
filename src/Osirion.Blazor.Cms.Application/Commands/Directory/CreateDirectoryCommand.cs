namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Command to create a new directory
/// </summary>
public class CreateDirectoryCommand : ICommand
{
    /// <summary>
    /// Gets or sets the name of the directory
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path where the directory will be created
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent directory ID (if any)
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the description of the directory
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the locale for the directory
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the order index for the directory
    /// </summary>
    public int Order { get; set; }
}