using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osirion.Blazor.Cms.Domain.Entities;

/// <summary>
/// Represents a directory in the content structure
/// </summary>
public class DirectoryItem : DomainEntity<string>
{
    private readonly Dictionary<string, object> _metadataValues = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<DirectoryItem> _children = new();
    private readonly List<ContentItem> _items = new();

    // Core properties
    public string Path { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string Locale { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public DirectoryItem? Parent { get; private set; }

    // Collections and readonly properties
    public IReadOnlyList<DirectoryItem> Children => _children.AsReadOnly();
    public IReadOnlyList<ContentItem> Items => _items.AsReadOnly();
    public IReadOnlyDictionary<string, object> Metadata => _metadataValues;

    // Computed properties
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

    // Modifier methods
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

    public void SetPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ContentValidationException("Path", "Directory path cannot be empty");

        Path = path;
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

    // Child directory operations
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

    public void ClearChildren()
    {
        foreach (var child in _children.ToList())
        {
            RemoveChild(child);
        }
    }

    // Content item operations
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
            RemoveItem(item);
        }
    }

    // Metadata operations
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

    public void SetMetadata<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));

        if (value == null)
        {
            _metadataValues.Remove(key);
        }
        else
        {
            _metadataValues[key] = value;
        }
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
        foreach (var kvp in _metadataValues)
        {
            clone._metadataValues[kvp.Key] = kvp.Value;
        }

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

    public void RaiseCreatedEvent()
    {
        AddDomainEvent(new DirectoryCreatedEvent(
            Id,
            Name,
            Path,
            ProviderId));
    }

    public void RaiseUpdatedEvent()
    {
        AddDomainEvent(new DirectoryUpdatedEvent(
            Id,
            Name,
            Path,
            ProviderId));
    }

    public void RaiseDeletedEvent(bool recursive)
    {
        AddDomainEvent(new DirectoryDeletedEvent(
            Id,
            Path,
            ProviderId,
            recursive));
    }
}