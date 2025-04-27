using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Represents a content tag
/// </summary>
public class ContentTag : ValueObject
{
    /// <summary>
    /// Gets the name of the tag
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the URL-friendly slug
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the number of content items with this tag
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets the URL for the tag
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the color associated with this tag
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Gets whether this tag is featured
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Gets the group this tag belongs to (for tag categorization)
    /// </summary>
    public string? Group { get; private set; }

    /// <summary>
    /// Creates a new content tag
    /// </summary>
    public static ContentTag Create(
        string name,
        string slug,
        int count = 0,
        string? url = null,
        string? color = null,
        bool isFeatured = false,
        string? group = null)
    {
        return new ContentTag
        {
            Name = name,
            Slug = slug,
            Count = count,
            Url = url ?? string.Empty,
            Color = color,
            IsFeatured = isFeatured,
            Group = group
        };
    }

    /// <summary>
    /// Creates a new content tag with an updated count
    /// </summary>
    public ContentTag WithCount(int count)
    {
        var clone = Clone();
        clone.Count = count;
        return clone;
    }

    /// <summary>
    /// Creates a new content tag with an updated URL
    /// </summary>
    public ContentTag WithUrl(string url)
    {
        var clone = Clone();
        clone.Url = url;
        return clone;
    }

    /// <summary>
    /// Creates a new content tag with an updated color
    /// </summary>
    public ContentTag WithColor(string? color)
    {
        var clone = Clone();
        clone.Color = color;
        return clone;
    }

    /// <summary>
    /// Creates a new content tag with an updated featured status
    /// </summary>
    public ContentTag WithFeatured(bool isFeatured)
    {
        var clone = Clone();
        clone.IsFeatured = isFeatured;
        return clone;
    }

    /// <summary>
    /// Creates a new content tag with an updated group
    /// </summary>
    public ContentTag WithGroup(string? group)
    {
        var clone = Clone();
        clone.Group = group;
        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this tag
    /// </summary>
    public ContentTag Clone()
    {
        return new ContentTag
        {
            Name = Name,
            Slug = Slug,
            Count = Count,
            Url = Url,
            Color = Color,
            IsFeatured = IsFeatured,
            Group = Group
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Slug;
        // Count is explicitly excluded from equality comparison
        yield return Url;
        yield return Color ?? string.Empty;
        yield return IsFeatured;
        yield return Group ?? string.Empty;
    }
}