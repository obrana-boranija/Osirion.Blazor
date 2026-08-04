using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.ValueObjects;
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
    /// <summary>Gets the directory path.</summary>
    public string Path { get; private set; } = string.Empty;
    /// <summary>Gets the directory name.</summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>Gets the directory description.</summary>
    public string Description { get; private set; } = string.Empty;
    /// <summary>Gets the directory URL.</summary>
    public string Url { get; private set; } = string.Empty;
    /// <summary>Gets the featured image URL.</summary>
    public string? FeaturedImageUrl { get; private set; }
    /// <summary>Gets the directory locale.</summary>
    public string Locale { get; private set; } = string.Empty;
    /// <summary>Gets the display order.</summary>
    public int Order { get; private set; }
    /// <summary>Gets the parent directory.</summary>
    public DirectoryItem? Parent { get; private set; }

    // Collections and readonly properties
    /// <summary>Gets the child directories.</summary>
    public IReadOnlyList<DirectoryItem> Children => _children.AsReadOnly();
    /// <summary>Gets the content items in this directory.</summary>
    public IReadOnlyList<ContentItem> Items => _items.AsReadOnly();
    /// <summary>Gets the directory metadata.</summary>
    public FrontMatter? Metadata { get; private set; }

    // Computed properties
    /// <summary>Gets the directory depth in the hierarchy.</summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            var current = Parent;
            while (current is not null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }
    }

    // Private constructor for initialization
    private DirectoryItem() { }

    /// <summary>Creates a directory item.</summary>
    /// <param name="id">The directory identifier.</param>
    /// <param name="path">The directory path.</param>
    /// <param name="name">The directory name.</param>
    /// <param name="providerId">The content provider identifier.</param>
    public static DirectoryItem Create(string id, string path, string name, string providerId)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ContentValidationException("Path", "Directory path cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            throw new ContentValidationException("Name", "Directory name cannot be empty");

        return new DirectoryItem
        {
            Id = id,
            Path = path,
            Name = name,
            ProviderId = providerId
        };
    }

    /// <summary>Changes the directory name.</summary>
    /// <param name="name">The new name.</param>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ContentValidationException("Name", "Directory name cannot be empty");

        Name = name;
    }

    /// <summary>Changes the directory description.</summary>
    /// <param name="description">The new description.</param>
    public void SetDescription(string description)
    {
        Description = description;
    }

    /// <summary>Changes the directory URL.</summary>
    /// <param name="url">The new URL.</param>
    public void SetUrl(string url)
    {
        Url = url;
    }

    /// <summary>Changes the featured image URL.</summary>
    /// <param name="url">The image URL.</param>
    public void SetFeaturedImage(string? url)
    {
        FeaturedImageUrl = url;
    }

    /// <summary>Changes the directory path.</summary>
    /// <param name="path">The new path.</param>
    public void SetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ContentValidationException("Path", "Directory path cannot be empty");

        Path = path;
    }

    /// <summary>Changes the directory locale.</summary>
    /// <param name="locale">The new locale.</param>
    public void SetLocale(string locale)
    {
        Locale = locale;
    }

    /// <summary>Changes the display order.</summary>
    /// <param name="order">The new order.</param>
    public void SetOrder(int order)
    {
        Order = order;
    }

    /// <summary>
    /// Sets the parent of this directory with additional safeguards
    /// </summary>
    public void SetParent(DirectoryItem? parent, bool skipCircularCheck = false)
    {
        // If parent is null, just clear the parent reference
        if (parent is null)
        {
            Parent = null;
            return;
        }

        // Don't set self as parent
        if (parent.Id == Id)
            throw new ContentValidationException("Parent", "Cannot set a directory as its own parent");

        // Skip circular checks if we already did them (from AddChild)
        if (!skipCircularCheck)
        {
            // Check if this directory is already an ancestor of the new parent
            if (IsAncestorOf(parent))
                throw new ContentValidationException("Parent", "Cannot set a child directory as parent");

            // Check if parent is already in this directory's ancestry
            if (parent.IsAncestorOf(this))
                throw new ContentValidationException("Parent", "Cannot set a parent that would create a circular reference");
        }

        Parent = parent;
    }

    /// <summary>
    /// Adds a directory as a child of this directory with additional safeguards
    /// </summary>
    public void AddChild(DirectoryItem child)
    {
        if (child is null)
            throw new ArgumentNullException(nameof(child));

        // Don't add self as child
        if (child.Id == Id)
            throw new ContentValidationException("Child", "Cannot add a directory as its own child");

        // Check if this directory is already an ancestor of the child (would create circular reference)
        if (IsAncestorOf(child))
            throw new ContentValidationException("Child", "Cannot add a directory as child of its descendant");

        // Check if child is already in this directory's ancestry (would create circular reference)
        if (child.IsAncestorOf(this))
            throw new ContentValidationException("Child", "Cannot add an ancestor directory as a child");

        // Avoid duplicates in the children collection
        if (!_children.Contains(child))
        {
            // Set parent first to ensure proper relationship
            child.SetParent(this, skipCircularCheck: true); // Skip check since we already did it
            _children.Add(child);
        }
    }

    /// <summary>Removes a child directory.</summary>
    /// <param name="child">The child directory to remove.</param>
    public void RemoveChild(DirectoryItem child)
    {
        if (child is not null && _children.Contains(child))
        {
            _children.Remove(child);
            child.SetParent(null);
        }
    }

    /// <summary>Removes all child directories.</summary>
    public void ClearChildren()
    {
        foreach (var child in _children.ToList())
        {
            RemoveChild(child);
        }
    }

    /// <summary>Adds a content item to this directory.</summary>
    /// <param name="item">The content item to add.</param>
    public void AddItem(ContentItem item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (!_items.Contains(item))
        {
            _items.Add(item);
            item.SetDirectory(this);
        }
    }

    /// <summary>Removes a content item from this directory.</summary>
    /// <param name="item">The content item to remove.</param>
    public void RemoveItem(ContentItem item)
    {
        if (item is not null && _items.Contains(item))
        {
            _items.Remove(item);
            item.SetDirectory(null);
        }
    }

    /// <summary>Removes all content items.</summary>
    public void ClearItems()
    {
        foreach (var item in _items.ToList())
        {
            RemoveItem(item);
        }
    }

    /// <summary>Gets a typed metadata value.</summary>
    /// <typeparam name="T">The metadata value type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <param name="defaultValue">The fallback value.</param>
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

    /// <summary>Sets the directory front matter.</summary>
    /// <param name="metadata">The metadata to assign.</param>
    public void SetMetadata(FrontMatter? metadata)
    {
        Metadata = metadata;
    }

    /// <summary>Sets or removes a metadata value.</summary>
    /// <typeparam name="T">The metadata value type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The value to store.</param>
    public void SetMetadata<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));

        if (value is null)
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
        if (directory is null)
            return false;

        // More robust equality check
        if (Id == directory.Id)
            return false; // A directory isn't its own ancestor

        // Maximum depth to prevent infinite loops in case of circular references
        int maxDepth = 100;
        int depth = 0;
        var visited = new HashSet<string>(); // Track visited IDs to detect loops
        var current = directory.Parent;

        while (current is not null && depth < maxDepth)
        {
            depth++;

            // If we've seen this ID before, we have a loop
            if (!visited.Add(current.Id))
            {
                // Log this circular reference
                // Logger.LogWarning("Circular reference detected in directory structure: {DirectoryId}", current.Id);
                return false;
            }

            // Found match - this directory is an ancestor
            if (current.Id == Id)
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

        while (current is not null)
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

    /// <summary>Performs the RaiseCreatedEvent operation.</summary>
    public void RaiseCreatedEvent()
    {
        AddDomainEvent(new DirectoryCreatedEvent(
            Id,
            Name,
            Path,
            ProviderId));
    }

    /// <summary>Performs the RaiseUpdatedEvent operation.</summary>
    public void RaiseUpdatedEvent()
    {
        AddDomainEvent(new DirectoryUpdatedEvent(
            Id,
            Name,
            Path,
            ProviderId));
    }

    /// <summary>Gets or sets the RaiseDeletedEvent value.</summary>
    public void RaiseDeletedEvent(bool recursive)
    {
        AddDomainEvent(new DirectoryDeletedEvent(
            Id,
            Path,
            ProviderId,
            recursive));
    }
}
