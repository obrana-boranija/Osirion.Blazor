namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents a directory in the content structure
/// </summary>
public class DirectoryItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the directory
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the directory
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the directory
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the directory
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for the directory
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale/language code (e.g., "en", "es", "de")
    /// </summary>
    public string Locale { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order for sorting
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the parent directory
    /// </summary>
    public DirectoryItem? Parent { get; set; }

    /// <summary>
    /// Gets or sets the child directories
    /// </summary>
    public List<DirectoryItem> Children { get; set; } = new();

    /// <summary>
    /// Gets or sets the content items in this directory
    /// </summary>
    public List<ContentItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets additional metadata for the directory
    /// </summary>
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the provider identifier that created this directory
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-specific identifier
    /// </summary>
    public string? ProviderSpecificId { get; set; }

    /// <summary>
    /// Gets the depth level in the directory tree (0 = root)
    /// </summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            var current = Parent;
            while (current != null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }
    }

    /// <summary>
    /// Gets a strongly-typed metadata value
    /// </summary>
    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        if (Metadata.TryGetValue(key, out var value))
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
    /// Sets a metadata value with strong typing
    /// </summary>
    public void SetMetadata<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));
        }

        if (value == null)
        {
            Metadata.Remove(key);
        }
        else
        {
            Metadata[key] = value;
        }
    }

    /// <summary>
    /// Creates a deep clone of this directory item (without children and items)
    /// </summary>
    public DirectoryItem Clone()
    {
        var clone = new DirectoryItem
        {
            Id = Id,
            Path = Path,
            Name = Name,
            Description = Description,
            Url = Url,
            Locale = Locale,
            Order = Order,
            Parent = Parent,
            ProviderId = ProviderId,
            ProviderSpecificId = ProviderSpecificId
        };

        // Clone metadata
        foreach (var kvp in Metadata)
        {
            clone.Metadata[kvp.Key] = kvp.Value;
        }

        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this directory item including children (but not items)
    /// </summary>
    public DirectoryItem CloneWithChildren()
    {
        var clone = Clone();

        foreach (var child in Children)
        {
            var childClone = child.CloneWithChildren();
            childClone.Parent = clone;
            clone.Children.Add(childClone);
        }

        return clone;
    }

    /// <summary>
    /// Gets the full path from root to this directory
    /// </summary>
    public List<DirectoryItem> GetBreadcrumbPath()
    {
        var path = new List<DirectoryItem>();
        var current = this;

        while (current != null)
        {
            path.Insert(0, current);
            current = current.Parent;
        }

        return path;
    }
}