using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Extensions;

namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Represents a content category
/// </summary>
public class ContentCategory : ValueObject
{
    /// <summary>Gets the category name.</summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>Gets the URL-friendly category slug.</summary>
    public string Slug { get; private set; } = string.Empty;
    /// <summary>Gets the number of content items in the category.</summary>
    public int Count { get; private set; }
    /// <summary>Gets the category description.</summary>
    public string? Description { get; private set; }
    /// <summary>Gets the parent category.</summary>
    public ContentCategory? Parent { get; private set; }
    /// <summary>Gets the child categories.</summary>
    public IReadOnlyList<ContentCategory> Children { get; private set; } = new List<ContentCategory>();
    /// <summary>Gets the category URL.</summary>
    public string Url { get; private set; } = string.Empty;
    /// <summary>Gets the category color.</summary>
    public string? Color { get; private set; }
    /// <summary>Gets the category icon.</summary>
    public string? Icon { get; private set; }
    /// <summary>Gets the display order.</summary>
    public int Order { get; private set; }
    /// <summary>Gets whether the category is featured.</summary>
    public bool IsFeatured { get; private set; }

    // Metadata
    private readonly Dictionary<string, object> _metadataValues = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>Gets the category metadata.</summary>
    public IReadOnlyDictionary<string, object> Metadata => _metadataValues;

    // Private constructor for object initialization
    private ContentCategory() { }

    /// <summary>
    /// Creates a new content category
    /// </summary>
    public static ContentCategory Create(
        string name,
        string? slug = null,
        int count = 0,
        string? description = null,
        string? url = null,
        string? color = null,
        string? icon = null,
        int order = 0,
        bool isFeatured = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        return new ContentCategory
        {
            Name = name,
            Slug = slug ?? name.GenerateSlug(),
            Count = count,
            Description = description,
            Url = url ?? string.Empty,
            Color = color,
            Icon = icon,
            Order = order,
            IsFeatured = isFeatured
        };
    }

    /// <summary>Returns a copy with a different name.</summary>
    /// <param name="name">The new category name.</param>
    public ContentCategory WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        var clone = Clone();
        clone.Name = name;
        return clone;
    }

    /// <summary>Returns a copy with a different slug.</summary>
    /// <param name="slug">The new category slug.</param>
    public ContentCategory WithSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Category slug cannot be empty", nameof(slug));

        if (!slug.IsValidSlug())
            throw new ArgumentException("Category slug contains invalid characters", nameof(slug));

        var clone = Clone();
        clone.Slug = slug;
        return clone;
    }

    /// <summary>Returns a copy with a different item count.</summary>
    /// <param name="count">The new item count.</param>
    public ContentCategory WithCount(int count)
    {
        var clone = Clone();
        clone.Count = count;
        return clone;
    }

    /// <summary>Returns a copy with a different description.</summary>
    /// <param name="description">The new description.</param>
    public ContentCategory WithDescription(string? description)
    {
        var clone = Clone();
        clone.Description = description;
        return clone;
    }

    /// <summary>Returns a copy with a different parent category.</summary>
    /// <param name="parent">The new parent category.</param>
    public ContentCategory WithParent(ContentCategory? parent)
    {
        // Prevent circular references
        if (parent is not null && IsAncestorOf(parent))
            throw new ArgumentException("Cannot set a child category as parent", nameof(parent));

        var clone = Clone();
        clone.Parent = parent;
        return clone;
    }

    /// <summary>Returns a copy with different child categories.</summary>
    /// <param name="children">The child categories.</param>
    public ContentCategory WithChildren(IEnumerable<ContentCategory> children)
    {
        var clone = Clone();
        clone.Children = children is not null
            ? children.ToList()
            : new List<ContentCategory>();
        return clone;
    }

    /// <summary>Returns a copy with a different URL.</summary>
    /// <param name="url">The new category URL.</param>
    public ContentCategory WithUrl(string url)
    {
        var clone = Clone();
        clone.Url = url;
        return clone;
    }

    /// <summary>Returns a copy with a different color.</summary>
    /// <param name="color">The new category color.</param>
    public ContentCategory WithColor(string? color)
    {
        var clone = Clone();
        clone.Color = color;
        return clone;
    }

    /// <summary>Gets or sets the WithIcon value.</summary>
    public ContentCategory WithIcon(string? icon)
    {
        var clone = Clone();
        clone.Icon = icon;
        return clone;
    }

    /// <summary>Gets or sets the WithOrder value.</summary>
    public ContentCategory WithOrder(int order)
    {
        var clone = Clone();
        clone.Order = order;
        return clone;
    }

    /// <summary>Gets or sets the WithFeatured value.</summary>
    public ContentCategory WithFeatured(bool isFeatured)
    {
        var clone = Clone();
        clone.IsFeatured = isFeatured;
        return clone;
    }

    /// <summary>Gets or sets the WithMetadata value.</summary>
    public ContentCategory WithMetadata(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));

        var clone = Clone();
        clone._metadataValues[key] = value;
        return clone;
    }

    /// <summary>Gets a typed metadata value by key.</summary>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        if (_metadataValues.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }

            // Try to convert if possible
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Checks if this category is an ancestor of the specified category
    /// </summary>
    public bool IsAncestorOf(ContentCategory category)
    {
        if (category is null)
            return false;

        var current = category.Parent;

        while (current is not null)
        {
            if (current.Equals(this))
                return true;

            current = current.Parent;
        }

        return false;
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
            Parent = Parent,
            Children = Children
        };

        // Clone metadata
        foreach (var kvp in _metadataValues)
        {
            clone._metadataValues[kvp.Key] = kvp.Value;
        }

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
                childrenList.Add(childClone);
            }
            clone.Children = childrenList;
        }

        return clone;
    }

    /// <summary>Gets or sets the GetEqualityComponents value.</summary>
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
