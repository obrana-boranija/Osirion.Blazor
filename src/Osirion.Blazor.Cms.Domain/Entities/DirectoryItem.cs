using System;
using System.Collections.Generic;
using System.Linq;
using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Exceptions;

namespace Osirion.Blazor.Cms.Domain.Entities;

/// <summary>
/// Represents a directory in the content structure
/// </summary>
public class DirectoryItem : Entity<string>
{
    private readonly MetadataContainer _metadata = new();
    private readonly List<DirectoryItem> _children = new();
    private readonly List<ContentItem> _items = new();

    /// <summary>
    /// Gets or sets the path of the directory
    /// </summary>
    public string Path { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the directory
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the directory
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for the directory
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale/language code
    /// </summary>
    public string Locale { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order for sorting
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Gets or sets the parent directory
    /// </summary>
    public DirectoryItem? Parent { get; private set; }

    /// <summary>
    /// Gets the child directories
    /// </summary>
    public IReadOnlyList<DirectoryItem> Children => _children.AsReadOnly();

    /// <summary>
    /// Gets the content items in this directory
    /// </summary>
    public IReadOnlyList<ContentItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the metadata dictionary
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata => _metadata.Values;

    /// <summary>
    /// Gets or sets the provider identifier that created this directory
    /// </summary>
    public string ProviderId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider-specific identifier
    /// </summary>
    public string? ProviderSpecificId { get; private set; }

    // Private constructor for initialization
    private DirectoryItem() { }

    // Factory method
    public static DirectoryItem Create(string id, string path, string name, string providerId)
    {
        if (string.IsNullOrEmpty(path))
            throw new ContentValidationException("Path", "Directory path cannot be empty");

        if (string.IsNullOrEmpty(name))
            throw new ContentValidationException("Name", "Directory name cannot be empty");

        return new DirectoryItem
        {
            Id = id,
            Path = path,
            Name = name,
            ProviderId = providerId
        };
    }

    // Domain methods
    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ContentValidationException("Name", "Directory name cannot be empty");

        Name = name;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetUrl(string url)
    {
        Url = url;
    }

    public void SetLocale(string locale)
    {
        Locale = locale;
    }

    public void SetOrder(int order)
    {
        Order = order;
    }

    public void SetParent(DirectoryItem? parent)
    {
        // Prevent circular references
        if (parent != null && IsAncestorOf(parent))
            throw new ContentValidationException("Parent", "Cannot set a child directory as parent");

        Parent = parent;
    }

    public void AddChild(DirectoryItem child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));

        if (IsAncestorOf(child))
            throw new ContentValidationException("Child", "Cannot add a directory as child of its descendant");

        if (!_children.Contains(child))
        {
            _children.Add(child);
            child.SetParent(this);
        }
    }

    public void RemoveChild(DirectoryItem child)
    {
        if (child != null && _children.Contains(child))
        {
            _children.Remove(child);
            child.SetParent(null);
        }
    }

    public void AddItem(ContentItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (!_items.Contains(item))
        {
            _items.Add(item);
            item.SetDirectory(this);
        }
    }

    public void RemoveItem(ContentItem item)
    {
        if (item != null && _items.Contains(item))
        {
            _items.Remove(item);
            item.SetDirectory(null);
        }
    }

    public void ClearItems()
    {
        foreach (var item in _items.ToList())
        {
            item.SetDirectory(null);
        }
        _items.Clear();
    }

    public T? GetMetadata<T>(string key, T? defaultValue = default)
    {
        return _metadata.GetValue(key, defaultValue);
    }

    public void SetMetadata<T>(string key, T value)
    {
        _metadata.SetValue(key, value);
    }

    public void SetProviderSpecificId(string? id)
    {
        ProviderSpecificId = id;
    }

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

    /// <summary>
    /// Checks if this directory is an ancestor of the specified directory
    /// </summary>
    public bool IsAncestorOf(DirectoryItem directory)
    {
        if (directory == null)
            return false;

        var current = directory.Parent;

        while (current != null)
        {
            if (current == this)
                return true;

            current = current.Parent;
        }

        return false;
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
        clone._metadata = _metadata.Clone();

        return clone;
    }

    /// <summary>
    /// Creates a deep clone of this directory item including children (but not items)
    /// </summary>
    public DirectoryItem CloneWithChildren()
    {
        var clone = Clone();

        foreach (var child in _children)
        {
            var childClone = child.CloneWithChildren();
            clone.AddChild(childClone);
        }

        return clone;
    }
}