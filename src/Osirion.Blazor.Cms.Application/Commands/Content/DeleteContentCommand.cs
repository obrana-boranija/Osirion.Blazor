namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Command to delete a content item
/// </summary>
public class DeleteContentCommand : ICommand
{
    /// <summary>
    /// Gets or sets the ID of the content to delete
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the commit message for versioned providers
    /// </summary>
    public string? CommitMessage { get; set; }
}