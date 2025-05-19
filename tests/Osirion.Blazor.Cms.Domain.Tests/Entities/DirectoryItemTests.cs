using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

public class DirectoryItemTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsValidInstance()
    {
        // Arrange
        string id = "dir-1";
        string path = "blog";
        string name = "Blog";
        string providerId = "test-provider";

        // Act
        var directory = DirectoryItem.Create(id, path, name, providerId);

        // Assert
        Assert.NotNull(directory);
        Assert.Equal(id, directory.Id);
        Assert.Equal(path, directory.Path);
        Assert.Equal(name, directory.Name);
        Assert.Equal(providerId, directory.ProviderId);
        Assert.Equal(0, directory.Depth);
        Assert.Empty(directory.Children);
        Assert.Empty(directory.Items);
    }

    [Fact]
    public void Create_WithEmptyPath_ThrowsContentValidationException()
    {
        // Arrange
        string id = "dir-1";
        string path = "";
        string name = "Blog";
        string providerId = "test-provider";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            DirectoryItem.Create(id, path, name, providerId));
        Assert.Equal("Path", exception.Errors.Keys.First());
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsContentValidationException()
    {
        // Arrange
        string id = "dir-1";
        string path = "blog";
        string name = "";
        string providerId = "test-provider";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            DirectoryItem.Create(id, path, name, providerId));
        Assert.Equal("Name", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetName_WithValidName_UpdatesName()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string newName = "Updated Name";

        // Act
        directory.SetName(newName);

        // Assert
        Assert.Equal(newName, directory.Name);
    }

    [Fact]
    public void SetName_WithEmptyName_ThrowsContentValidationException()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string emptyName = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            directory.SetName(emptyName));
        Assert.Equal("Name", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetPath_WithValidPath_UpdatesPath()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string newPath = "new/path";

        // Act
        directory.SetPath(newPath);

        // Assert
        Assert.Equal(newPath, directory.Path);
    }

    [Fact]
    public void SetPath_WithEmptyPath_ThrowsContentValidationException()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string emptyPath = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            directory.SetPath(emptyPath));
        Assert.Equal("Path", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetParent_UpdatesParentReference()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");

        // Act
        child.SetParent(parent);

        // Assert
        Assert.Same(parent, child.Parent);
    }

    [Fact]
    public void SetParent_WithNull_ClearsParentReference()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");
        child.SetParent(parent);

        // Act
        child.SetParent(null);

        // Assert
        Assert.Null(child.Parent);
    }

    [Fact]
    public void SetParent_WithSelf_ThrowsContentValidationException()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            directory.SetParent(directory));
        Assert.Equal("Parent", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetParent_WithChildAsParent_ThrowsContentValidationException()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");
        parent.AddChild(child);

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            parent.SetParent(child));
        Assert.Equal("Parent", exception.Errors.Keys.First());
    }

    [Fact]
    public void AddChild_AddsToChildrenCollection()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");

        // Act
        parent.AddChild(child);

        // Assert
        Assert.Single(parent.Children);
        Assert.Same(child, parent.Children.First());
        Assert.Same(parent, child.Parent);
    }

    [Fact]
    public void AddChild_WithSelf_ThrowsContentValidationException()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            directory.AddChild(directory));
        Assert.Equal("Child", exception.Errors.Keys.First());
    }

    [Fact]
    public void AddChild_WithParentAsChild_ThrowsContentValidationException()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");
        parent.AddChild(child);

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            child.AddChild(parent));
        Assert.Equal("Child", exception.Errors.Keys.First());
    }

    [Fact]
    public void AddChild_WithExistingChild_DoesNotAddDuplicate()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");
        parent.AddChild(child);

        // Act
        parent.AddChild(child);

        // Assert
        Assert.Single(parent.Children);
    }

    [Fact]
    public void RemoveChild_RemovesFromChildrenCollection()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child = CreateTestDirectory("child-id", "parent/child", "Child");
        parent.AddChild(child);

        // Act
        parent.RemoveChild(child);

        // Assert
        Assert.Empty(parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void ClearChildren_RemovesAllChildren()
    {
        // Arrange
        var parent = CreateTestDirectory("parent-id", "parent", "Parent");
        var child1 = CreateTestDirectory("child-1", "parent/child1", "Child 1");
        var child2 = CreateTestDirectory("child-2", "parent/child2", "Child 2");
        parent.AddChild(child1);
        parent.AddChild(child2);

        // Act
        parent.ClearChildren();

        // Assert
        Assert.Empty(parent.Children);
        Assert.Null(child1.Parent);
        Assert.Null(child2.Parent);
    }

    [Fact]
    public void AddItem_AddsToItemsCollection()
    {
        // Arrange
        var directory = CreateTestDirectory();
        var contentItem = ContentItem.Create(
            "content-1",
            "Test Content",
            "Content",
            "path/to/content.md",
            "test-provider");

        // Act
        directory.AddItem(contentItem);

        // Assert
        Assert.Single(directory.Items);
        Assert.Same(contentItem, directory.Items.First());
        Assert.Same(directory, contentItem.Directory);
    }

    [Fact]
    public void AddItem_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => directory.AddItem(null));
    }

    [Fact]
    public void AddItem_WithExistingItem_DoesNotAddDuplicate()
    {
        // Arrange
        var directory = CreateTestDirectory();
        var contentItem = ContentItem.Create(
            "content-1",
            "Test Content",
            "Content",
            "path/to/content.md",
            "test-provider");
        directory.AddItem(contentItem);

        // Act
        directory.AddItem(contentItem);

        // Assert
        Assert.Single(directory.Items);
    }

    [Fact]
    public void RemoveItem_RemovesFromItemsCollection()
    {
        // Arrange
        var directory = CreateTestDirectory();
        var contentItem = ContentItem.Create(
            "content-1",
            "Test Content",
            "Content",
            "path/to/content.md",
            "test-provider");
        directory.AddItem(contentItem);

        // Act
        directory.RemoveItem(contentItem);

        // Assert
        Assert.Empty(directory.Items);
        Assert.Null(contentItem.Directory);
    }

    [Fact]
    public void ClearItems_RemovesAllItems()
    {
        // Arrange
        var directory = CreateTestDirectory();
        var item1 = ContentItem.Create("id1", "Item 1", "Content 1", "path1.md", "provider");
        var item2 = ContentItem.Create("id2", "Item 2", "Content 2", "path2.md", "provider");
        directory.AddItem(item1);
        directory.AddItem(item2);

        // Act
        directory.ClearItems();

        // Assert
        Assert.Empty(directory.Items);
        Assert.Null(item1.Directory);
        Assert.Null(item2.Directory);
    }

    [Fact]
    public void GetMetadata_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string key = "test-key";
        string value = "test-value";
        directory.SetMetadata(key, value);

        // Act
        var result = directory.GetMetadata<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetMetadata_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        var directory = CreateTestDirectory();
        string key = null;
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => directory.SetMetadata(key, value));
    }

    [Fact]
    public void Depth_CalculatesCorrectly_ForNestedDirectories()
    {
        // Arrange
        var root = CreateTestDirectory("root", "root", "Root");
        var level1 = CreateTestDirectory("level1", "root/level1", "Level 1");
        var level2 = CreateTestDirectory("level2", "root/level1/level2", "Level 2");
        var level3 = CreateTestDirectory("level3", "root/level1/level2/level3", "Level 3");

        root.AddChild(level1);
        level1.AddChild(level2);
        level2.AddChild(level3);

        // Assert
        Assert.Equal(0, root.Depth);
        Assert.Equal(1, level1.Depth);
        Assert.Equal(2, level2.Depth);
        Assert.Equal(3, level3.Depth);
    }

    [Fact]
    public void IsAncestorOf_WithDescendant_ReturnsTrue()
    {
        // Arrange
        var root = CreateTestDirectory("root", "root", "Root");
        var level1 = CreateTestDirectory("level1", "root/level1", "Level 1");
        var level2 = CreateTestDirectory("level2", "root/level1/level2", "Level 2");

        root.AddChild(level1);
        level1.AddChild(level2);

        // Act & Assert
        Assert.True(root.IsAncestorOf(level1));
        Assert.True(root.IsAncestorOf(level2));
        Assert.True(level1.IsAncestorOf(level2));
    }

    [Fact]
    public void IsAncestorOf_WithSelf_ReturnsFalse()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act & Assert
        Assert.False(directory.IsAncestorOf(directory));
    }

    [Fact]
    public void IsAncestorOf_WithNonDescendant_ReturnsFalse()
    {
        // Arrange
        var dir1 = CreateTestDirectory("dir1", "dir1", "Dir 1");
        var dir2 = CreateTestDirectory("dir2", "dir2", "Dir 2");

        // Act & Assert
        Assert.False(dir1.IsAncestorOf(dir2));
    }

    [Fact]
    public void IsAncestorOf_WithNull_ReturnsFalse()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act & Assert
        Assert.False(directory.IsAncestorOf(null));
    }

    [Fact]
    public void GetBreadcrumbPath_ReturnsPathFromRootToSelf()
    {
        // Arrange
        var root = CreateTestDirectory("root", "root", "Root");
        var level1 = CreateTestDirectory("level1", "root/level1", "Level 1");
        var level2 = CreateTestDirectory("level2", "root/level1/level2", "Level 2");

        root.AddChild(level1);
        level1.AddChild(level2);

        // Act
        var breadcrumb = level2.GetBreadcrumbPath();

        // Assert
        Assert.Equal(3, breadcrumb.Count);
        Assert.Same(root, breadcrumb[0]);
        Assert.Same(level1, breadcrumb[1]);
        Assert.Same(level2, breadcrumb[2]);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = CreateTestDirectory();
        original.SetName("Original Name");
        original.SetDescription("Original Description");
        original.SetUrl("original-url");
        original.SetMetadata("key1", "value1");
        original.SetMetadata("key2", 42);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Path, clone.Path);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Description, clone.Description);
        Assert.Equal(original.Url, clone.Url);
        Assert.Equal(original.ProviderId, clone.ProviderId);

        Assert.Equal(original.Metadata.Count, clone.Metadata.Count);
        foreach (var key in original.Metadata.Keys)
        {
            Assert.Equal(original.Metadata[key], clone.Metadata[key]);
        }

        // Clone doesn't include children or items
        Assert.Empty(clone.Children);
        Assert.Empty(clone.Items);
    }

    [Fact]
    public void CloneWithChildren_CreatesDeepCopyWithChildren()
    {
        // Arrange
        var parent = CreateTestDirectory("parent", "parent", "Parent");
        var child1 = CreateTestDirectory("child1", "parent/child1", "Child 1");
        var child2 = CreateTestDirectory("child2", "parent/child2", "Child 2");

        parent.AddChild(child1);
        parent.AddChild(child2);

        // Act
        var clone = parent.CloneWithChildren();

        // Assert
        Assert.NotSame(parent, clone);
        Assert.Equal(parent.Id, clone.Id);
        Assert.Equal(parent.Path, clone.Path);
        Assert.Equal(parent.Name, clone.Name);

        Assert.Equal(2, clone.Children.Count);
        Assert.Contains(clone.Children, c => c.Id == "child1");
        Assert.Contains(clone.Children, c => c.Id == "child2");

        // Verify child parent references are set correctly
        foreach (var clonedChild in clone.Children)
        {
            Assert.Same(clone, clonedChild.Parent);
        }

        // Verify original children are not modified
        foreach (var originalChild in parent.Children)
        {
            Assert.Same(parent, originalChild.Parent);
        }
    }

    [Fact]
    public void RaiseCreatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act
        directory.RaiseCreatedEvent();

        // Assert
        var createdEvent = directory.DomainEvents
            .OfType<DirectoryCreatedEvent>()
            .FirstOrDefault();

        Assert.NotNull(createdEvent);
        Assert.Equal(directory.Id, createdEvent.DirectoryId);
        Assert.Equal(directory.Name, createdEvent.Name);
        Assert.Equal(directory.Path, createdEvent.Path);
        Assert.Equal(directory.ProviderId, createdEvent.ProviderId);
    }

    [Fact]
    public void RaiseUpdatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var directory = CreateTestDirectory();

        // Act
        directory.RaiseUpdatedEvent();

        // Assert
        var updatedEvent = directory.DomainEvents
            .OfType<DirectoryUpdatedEvent>()
            .FirstOrDefault();

        Assert.NotNull(updatedEvent);
        Assert.Equal(directory.Id, updatedEvent.DirectoryId);
        Assert.Equal(directory.Name, updatedEvent.Name);
        Assert.Equal(directory.Path, updatedEvent.Path);
        Assert.Equal(directory.ProviderId, updatedEvent.ProviderId);
    }

    [Fact]
    public void RaiseDeletedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var directory = CreateTestDirectory();
        bool recursive = true;

        // Act
        directory.RaiseDeletedEvent(recursive);

        // Assert
        var deletedEvent = directory.DomainEvents
            .OfType<DirectoryDeletedEvent>()
            .FirstOrDefault();

        Assert.NotNull(deletedEvent);
        Assert.Equal(directory.Id, deletedEvent.DirectoryId);
        Assert.Equal(directory.Path, deletedEvent.Path);
        Assert.Equal(directory.ProviderId, deletedEvent.ProviderId);
        Assert.True(deletedEvent.Recursive);
    }

    // Helper method to create a test directory
    private DirectoryItem CreateTestDirectory(string id = "test-id", string path = "test-path", string name = "Test Directory")
    {
        return DirectoryItem.Create(id, path, name, "test-provider");
    }
}