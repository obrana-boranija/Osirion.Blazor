using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Represents a content category
/// </summary>
public class ContentCategory : ValueObject
{
    /// <summary>
    /// Gets the name of the category
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the URL-friendly slug
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the number of content items in this category
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets the description of the category
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the parent category if this is a subcategory
    /// </summary>
    public ContentCategory? Parent { get; private set; }

    /// <summary>
    /// Gets the child categories if this is a parent category
    /// </summary>
    public IReadOnlyList<ContentCategory> Children { get; private set; } = new List<ContentCategory>();

    /// <summary>
    /// Gets the metadata for the category
    /// </summary>
    private readonly MetadataContainer _metadata = new();
    public IReadOnlyDictionary<string, object> Metadata => _metadata.Values;

    /// <summary>
    /// Gets the URL for the category
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the color associated with this category
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Gets the icon associated with this category
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Gets the order for sorting
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Gets whether this category is featured
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Private constructor for object initialization
    /// </summary>
    private ContentCategory() { }

    /// <summary>
    /// Creates a new content category
    /// </summary>
    public static ContentCategory Create(
        string name,
        string slug,
        int count = 0,
        string? description = null,
        string? url = null,
        string? color = null,
        string? icon = null,
        int order = 0,
        bool isFeatured = false)
    {
        return new ContentCategory
        {
            Name = name,
            Slug = slug,
            Count = count,
            Description = description,
            Url = url ?? string.Empty,
            Color = color,
            Icon = icon,
            Order = order,
            IsFeatured = isFeatured
        };
    }

    /// <summary>
    /// Gets a metadata value
    /// </summary>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        return _metadata.GetValue(key, defaultValue);
    }

    /// <summary>
    /// Creates a new content category with the specified count
    /// </summary>
    public ContentCategory WithCount(int count)
    {
        var clone = Clone();
        clone.Count = count;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified description
    /// </summary>
    public ContentCategory WithDescription(string? description)
    {
        var clone = Clone();
        clone.Description = description;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified parent
    /// </summary>
    public ContentCategory WithParent(ContentCategory? parent)
    {
        var clone = Clone();
        clone.Parent = parent;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified children
    /// </summary>
    public ContentCategory WithChildren(IEnumerable<ContentCategory> children)
    {
        var clone = Clone();
        clone.Children = children != null
            ? new List<ContentCategory>(children)
            : new List<ContentCategory>();
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified URL
    /// </summary>
    public ContentCategory WithUrl(string url)
    {
        var clone = Clone();
        clone.Url = url;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified color
    /// </summary>
    public ContentCategory WithColor(string? color)
    {
        var clone = Clone();
        clone.Color = color;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified icon
    /// </summary>
    public ContentCategory WithIcon(string? icon)
    {
        var clone = Clone();
        clone.Icon = icon;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified order
    /// </summary>
    public ContentCategory WithOrder(int order)
    {
        var clone = Clone();
        clone.Order = order;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified featured status
    /// </summary>
    public ContentCategory WithFeatured(bool isFeatured)
    {
        var clone = Clone();
        clone.IsFeatured = isFeatured;
        return clone;
    }

    /// <summary>
    /// Creates a new content category with the specified metadata
    /// </summary>
    public ContentCategory WithMetadata(string key, object value)
    {
        var clone = Clone();
        clone._metadata.SetValue(key, value);
        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this category
    /// </summary>
    public ContentCategory Clone()
    {
        var clone = new ContentCategory
        {
            Name = Name,
            Slug = Slug,
            Count = Count,
            Description = Description,
            Url = Url,
            Color = Color,
            Icon = Icon,
            Order = Order,
            IsFeatured = IsFeatured,
            Parent = Parent
        };

        // Clone metadata
        clone._metadata = _metadata.Clone();

        // Don't clone children by default to avoid circular references

        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this category including children
    /// </summary>
    public ContentCategory CloneWithChildren()
    {
        var clone = Clone();

        if (Children.Count > 0)
        {
            var childrenList = new List<ContentCategory>();
            foreach (var child in Children)
            {
                var childClone = child.Clone(); // Don't recursively clone to avoid potential circular references
                childClone.Parent = clone;
                childrenList.Add(childClone);
            }
            clone.Children = childrenList;
        }

        return clone;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Slug;
        // Count is explicitly excluded from equality comparison
        yield return Description ?? string.Empty;
        yield return Url;
        yield return Color ?? string.Empty;
        yield return Icon ?? string.Empty;
        yield return Order;
        yield return IsFeatured;
        // Parent and Children are excluded to avoid circular references in equality checks
    }
}