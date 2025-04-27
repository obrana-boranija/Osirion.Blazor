namespace Osirion.Blazor.Cms.Application.Commands.Content;

/// <summary>
/// Command to create a new content item
/// </summary>
public class CreateContentCommand : ICommand
{
    /// <summary>
    /// Gets or sets the title of the content
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content body
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path where the content will be stored
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
    /// Gets or sets the author of the content
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the description of the content
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the slug for the content
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the tags for the content
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the categories for the content
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the content is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the locale for the content
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets the content ID for translations
    /// </summary>
    public string? ContentId { get; set; }
}