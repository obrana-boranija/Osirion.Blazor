using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

public class ContentItemTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsValidInstance()
    {
        // Arrange
        string id = "content-1";
        string title = "Test Content";
        string content = "This is test content.";
        string path = "blog/test-content.md";
        string providerId = "test-provider";

        // Act
        var contentItem = ContentItem.Create(id, title, content, path, providerId);

        // Assert
        Assert.NotNull(contentItem);
        Assert.Equal(id, contentItem.Id);
        Assert.Equal(title, contentItem.Title);
        Assert.Equal(content, contentItem.Content);
        Assert.Equal(path, contentItem.Path);
        Assert.Equal(providerId, contentItem.ProviderId);
        Assert.Equal("test-content", contentItem.Slug);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsContentValidationException()
    {
        // Arrange
        string id = "content-1";
        string title = "";
        string content = "This is test content.";
        string path = "blog/test-content.md";
        string providerId = "test-provider";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            ContentItem.Create(id, title, content, path, providerId));
        Assert.Equal("Title", exception.Errors.Keys.First());
    }

    [Fact]
    public void Create_WithEmptyPath_ThrowsContentValidationException()
    {
        // Arrange
        string id = "content-1";
        string title = "Test Content";
        string content = "This is test content.";
        string path = "";
        string providerId = "test-provider";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            ContentItem.Create(id, title, content, path, providerId));
        Assert.Equal("Path", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetTitle_WithValidTitle_UpdatesTitle()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string newTitle = "Updated Title";

        // Act
        contentItem.SetTitle(newTitle);

        // Assert
        Assert.Equal(newTitle, contentItem.Title);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void SetTitle_WithEmptyTitle_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string emptyTitle = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.SetTitle(emptyTitle));
        Assert.Equal("Title", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetContent_UpdatesContentAndOriginalMarkdown()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string newContent = "Updated content.";
        string newMarkdown = "# Updated content.";

        // Act
        contentItem.SetContent(newContent, newMarkdown);

        // Assert
        Assert.Equal(newContent, contentItem.Content);
        Assert.Equal(newMarkdown, contentItem.OriginalMarkdown);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void SetContent_WithNullContent_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.SetContent(null));
        Assert.Equal("Content", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetSlug_WithValidSlug_UpdatesSlug()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string newSlug = "new-slug";

        // Act
        contentItem.SetSlug(newSlug);

        // Assert
        Assert.Equal(newSlug, contentItem.Slug);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void SetSlug_WithEmptySlug_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string emptySlug = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.SetSlug(emptySlug));
        Assert.Equal("Slug", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetSlug_WithInvalidSlug_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string invalidSlug = "Invalid Slug With Spaces!";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.SetSlug(invalidSlug));
        Assert.Equal("Slug", exception.Errors.Keys.First());
    }

    [Fact]
    public void SetPath_WithValidPath_UpdatesPath()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string newPath = "new/path/to/content.md";

        // Act
        contentItem.SetPath(newPath);

        // Assert
        Assert.Equal(newPath, contentItem.Path);
    }

    [Fact]
    public void SetPath_WithEmptyPath_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string emptyPath = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.SetPath(emptyPath));
        Assert.Equal("Path", exception.Errors.Keys.First());
    }

    [Fact]
    public void AddTag_WithValidTag_AddsToTags()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string tag = "new-tag";

        // Act
        contentItem.AddTag(tag);

        // Assert
        Assert.Contains(tag, contentItem.Tags);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void AddTag_WithDuplicateTag_DoesNotAddDuplicate()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string tag = "existing-tag";
        contentItem.AddTag(tag);
        var initialModifiedDate = contentItem.LastModified;

        // Wait to ensure timestamp would be different
        System.Threading.Thread.Sleep(1);

        // Act
        contentItem.AddTag(tag);

        // Assert
        Assert.Single(contentItem.Tags.Where(t => t == tag));
        Assert.Equal(initialModifiedDate, contentItem.LastModified);
    }

    [Fact]
    public void AddTag_WithEmptyTag_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string emptyTag = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.AddTag(emptyTag));
        Assert.Equal("Tag", exception.Errors.Keys.First());
    }

    [Fact]
    public void RemoveTag_WithExistingTag_RemovesTag()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string tag = "test-tag";
        contentItem.AddTag(tag);

        // Act
        contentItem.RemoveTag(tag);

        // Assert
        Assert.DoesNotContain(tag, contentItem.Tags);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void RemoveTag_WithNonExistentTag_DoesNothing()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        var initialModifiedDate = contentItem.LastModified;

        // Wait to ensure timestamp would be different
        System.Threading.Thread.Sleep(1);

        // Act
        contentItem.RemoveTag("non-existent-tag");

        // Assert
        Assert.Equal(initialModifiedDate, contentItem.LastModified);
    }

    [Fact]
    public void ClearTags_RemovesAllTags()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        contentItem.AddTag("tag1");
        contentItem.AddTag("tag2");

        // Act
        contentItem.ClearTags();

        // Assert
        Assert.Empty(contentItem.Tags);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void AddCategory_WithValidCategory_AddsToCategories()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string category = "new-category";

        // Act
        contentItem.AddCategory(category);

        // Assert
        Assert.Contains(category, contentItem.Categories);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void AddCategory_WithDuplicateCategory_DoesNotAddDuplicate()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string category = "existing-category";
        contentItem.AddCategory(category);
        var initialModifiedDate = contentItem.LastModified;

        // Wait to ensure timestamp would be different
        System.Threading.Thread.Sleep(1);

        // Act
        contentItem.AddCategory(category);

        // Assert
        Assert.Single(contentItem.Categories.Where(c => c == category));
        Assert.Equal(initialModifiedDate, contentItem.LastModified);
    }

    [Fact]
    public void AddCategory_WithEmptyCategory_ThrowsContentValidationException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string emptyCategory = "";

        // Act & Assert
        var exception = Assert.Throws<ContentValidationException>(() =>
            contentItem.AddCategory(emptyCategory));
        Assert.Equal("Category", exception.Errors.Keys.First());
    }

    [Fact]
    public void RemoveCategory_WithExistingCategory_RemovesCategory()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string category = "test-category";
        contentItem.AddCategory(category);

        // Act
        contentItem.RemoveCategory(category);

        // Assert
        Assert.DoesNotContain(category, contentItem.Categories);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void RemoveCategory_WithNonExistentCategory_DoesNothing()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        var initialModifiedDate = contentItem.LastModified;

        // Wait to ensure timestamp would be different
        System.Threading.Thread.Sleep(1);

        // Act
        contentItem.RemoveCategory("non-existent-category");

        // Assert
        Assert.Equal(initialModifiedDate, contentItem.LastModified);
    }

    [Fact]
    public void ClearCategories_RemovesAllCategories()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        contentItem.AddCategory("category1");
        contentItem.AddCategory("category2");

        // Act
        contentItem.ClearCategories();

        // Assert
        Assert.Empty(contentItem.Categories);
        Assert.NotNull(contentItem.LastModified);
    }

    [Fact]
    public void GetMetadata_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string key = "test-key";
        string value = "test-value";
        contentItem.SetMetadata(key, value);

        // Act
        var result = contentItem.GetMetadata<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void SetMetadata_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string key = null;
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => contentItem.SetMetadata(key, value));
    }

    [Fact]
    public void SetMetadata_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        string key = "";
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => contentItem.SetMetadata(key, value));
    }

    [Fact]
    public void SetStatus_UpdatesStatusAndRaisesEvent()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        var previousStatus = contentItem.Status;
        var newStatus = ContentStatus.Archived;

        // Act
        contentItem.SetStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, contentItem.Status);
        Assert.NotNull(contentItem.LastModified);

        // Check for status changed event
        var statusChangedEvent = contentItem.DomainEvents
            .OfType<ContentStatusChangedEvent>()
            .FirstOrDefault();

        Assert.NotNull(statusChangedEvent);
        Assert.Equal(contentItem.Id, statusChangedEvent.ContentId);
        Assert.Equal(contentItem.Title, statusChangedEvent.Title);
        Assert.Equal(previousStatus, statusChangedEvent.PreviousStatus);
        Assert.Equal(newStatus, statusChangedEvent.NewStatus);
        Assert.Equal(contentItem.ProviderId, statusChangedEvent.ProviderId);
    }

    [Fact]
    public void SetStatus_WithSameStatus_DoesNotRaiseEvent()
    {
        // Arrange
        var contentItem = CreateTestContentItem();
        var status = contentItem.Status;

        // Act
        contentItem.SetStatus(status);

        // Assert
        Assert.Equal(status, contentItem.Status);
        Assert.NotNull(contentItem.LastModified);

        // Verify that no status changed event was raised
        var statusChangedEvent = contentItem.DomainEvents
            .OfType<ContentStatusChangedEvent>()
            .FirstOrDefault();

        Assert.Null(statusChangedEvent);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = CreateTestContentItem();
        original.SetTitle("Original Title");
        original.SetContent("Original content", "# Original content");
        original.AddTag("tag1");
        original.AddTag("tag2");
        original.AddCategory("category1");
        original.SetMetadata("key1", "value1");
        original.SetMetadata("key2", 42);
        original.SetSeoMetadata(SeoMetadata.Create("SEO Title", "SEO Description"));

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Title, clone.Title);
        Assert.Equal(original.Content, clone.Content);
        Assert.Equal(original.OriginalMarkdown, clone.OriginalMarkdown);

        Assert.Equal(original.Tags.Count, clone.Tags.Count);
        foreach (var tag in original.Tags)
        {
            Assert.Contains(tag, clone.Tags);
        }

        Assert.Equal(original.Categories.Count, clone.Categories.Count);
        foreach (var category in original.Categories)
        {
            Assert.Contains(category, clone.Categories);
        }
    }

    [Fact]
    public void RaiseCreatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var contentItem = CreateTestContentItem();

        // Act
        contentItem.RaiseCreatedEvent();

        // Assert
        var createdEvent = contentItem.DomainEvents
            .OfType<ContentCreatedEvent>()
            .FirstOrDefault();

        Assert.NotNull(createdEvent);
        Assert.Equal(contentItem.Id, createdEvent.ContentId);
        Assert.Equal(contentItem.Title, createdEvent.Title);
        Assert.Equal(contentItem.Path, createdEvent.Path);
        Assert.Equal(contentItem.ProviderId, createdEvent.ProviderId);
    }

    [Fact]
    public void RaiseUpdatedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var contentItem = CreateTestContentItem();

        // Act
        contentItem.RaiseUpdatedEvent();

        // Assert
        var updatedEvent = contentItem.DomainEvents
            .OfType<ContentUpdatedEvent>()
            .FirstOrDefault();

        Assert.NotNull(updatedEvent);
        Assert.Equal(contentItem.Id, updatedEvent.ContentId);
        Assert.Equal(contentItem.Title, updatedEvent.Title);
        Assert.Equal(contentItem.Path, updatedEvent.Path);
        Assert.Equal(contentItem.ProviderId, updatedEvent.ProviderId);
    }

    [Fact]
    public void RaiseDeletedEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var contentItem = CreateTestContentItem();

        // Act
        contentItem.RaiseDeletedEvent();

        // Assert
        var deletedEvent = contentItem.DomainEvents
            .OfType<ContentDeletedEvent>()
            .FirstOrDefault();

        Assert.NotNull(deletedEvent);
        Assert.Equal(contentItem.Id, deletedEvent.ContentId);
        Assert.Equal(contentItem.Path, deletedEvent.Path);
        Assert.Equal(contentItem.ProviderId, deletedEvent.ProviderId);
    }

    // Helper method to create a test content item
    private ContentItem CreateTestContentItem()
    {
        return ContentItem.Create(
            "test-id",
            "Test Content",
            "This is test content.",
            "blog/test-content.md",
            "test-provider");
    }
}