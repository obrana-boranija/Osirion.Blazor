namespace Osirion.Blazor.Models.Cms;

/// <summary>
/// Represents a content item from the CMS
/// </summary>
public class ContentItem
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of publication
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the keywords
    /// </summary>
    public List<string> Keywords { get; set; } = new();

    /// <summary>
    /// Gets or sets the categories
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets the slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this content is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the featured image URL
    /// </summary>
    public string FeaturedImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GitHub file path
    /// </summary>
    public string GitHubFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the directory
    /// </summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the last updated date
    /// </summary>
    public DateTime LastUpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the read time in minutes
    /// </summary>
    public int ReadTimeMinutes { get; set; }
}