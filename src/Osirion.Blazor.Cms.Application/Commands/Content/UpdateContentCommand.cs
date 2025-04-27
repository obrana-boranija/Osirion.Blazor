namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Command to update an existing content item
/// </summary>
public class UpdateContentCommand : ICommand
{
    /// <summary>
    /// Gets or sets the ID of the content to update
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the content
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content body
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path where the content is stored
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider ID to use
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the commit message for versioned providers
    /// </summary>
    public string? CommitMessage { get; set; }

    /// <summary>
    /// Gets or sets the provider-specific ID (e.g., SHA in GitHub)
    /// </summary>
    public string? ProviderSpecificId { get; set; }

    // Additional properties (similar to CreateContentCommand)
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public bool IsFeatured { get; set; }
    public string? Locale { get; set; }
    public string? ContentId { get; set; }
}