namespace Osirion.Blazor.Cms.Application.Commands.Directory;

/// <summary>
/// Command to update an existing directory
/// </summary>
public class UpdateDirectoryCommand : ICommand
{
    /// <summary>
    /// Gets or sets the ID of the directory to update
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the directory
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the directory
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the order index for the directory
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the provider-specific ID
    /// </summary>
    public string? ProviderSpecificId { get; set; }
}