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
    /// <summary>
    /// Gets or sets the author of the content
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the description of the content
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the slug for the content, used in URLs
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with the content
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the categories associated with the content
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the content is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the locale for the content, if localization is enabled
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the content ID for the content, used in some providers
    /// </summary>
    public string? ContentId { get; set; }
}