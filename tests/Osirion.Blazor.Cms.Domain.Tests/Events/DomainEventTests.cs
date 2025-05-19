using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Tests.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void DomainEvent_OnCreation_SetsOccurredOnToCurrentTime()
    {
        // Arrange
        var beforeTest = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var domainEvent = new TestDomainEvent();
        var afterTest = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeTest);
        Assert.True(domainEvent.OccurredOn <= afterTest);
    }

    [Fact]
    public void ContentCreatedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string contentId = "content-1";
        string title = "Test Content";
        string path = "blog/test-content.md";
        string providerId = "test-provider";

        // Act
        var @event = new ContentCreatedEvent(contentId, title, path, providerId);

        // Assert
        Assert.Equal(contentId, @event.ContentId);
        Assert.Equal(title, @event.Title);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void ContentCreatedEvent_WithNullValues_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => new ContentCreatedEvent(null, "title", "path", "provider"));
        Assert.Throws<ArgumentNullException>(() => new ContentCreatedEvent("id", null, "path", "provider"));
        Assert.Throws<ArgumentNullException>(() => new ContentCreatedEvent("id", "title", null, "provider"));
        Assert.Throws<ArgumentNullException>(() => new ContentCreatedEvent("id", "title", "path", null));
    }

    [Fact]
    public void ContentUpdatedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string contentId = "content-1";
        string title = "Test Content";
        string path = "blog/test-content.md";
        string providerId = "test-provider";

        // Act
        var @event = new ContentUpdatedEvent(contentId, title, path, providerId);

        // Assert
        Assert.Equal(contentId, @event.ContentId);
        Assert.Equal(title, @event.Title);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void ContentDeletedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string contentId = "content-1";
        string path = "blog/test-content.md";
        string providerId = "test-provider";

        // Act
        var @event = new ContentDeletedEvent(contentId, path, providerId);

        // Assert
        Assert.Equal(contentId, @event.ContentId);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void DirectoryCreatedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string directoryId = "dir-1";
        string name = "Test Directory";
        string path = "blog/test";
        string providerId = "test-provider";

        // Act
        var @event = new DirectoryCreatedEvent(directoryId, name, path, providerId);

        // Assert
        Assert.Equal(directoryId, @event.DirectoryId);
        Assert.Equal(name, @event.Name);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void DirectoryUpdatedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string directoryId = "dir-1";
        string name = "Test Directory";
        string path = "blog/test";
        string providerId = "test-provider";

        // Act
        var @event = new DirectoryUpdatedEvent(directoryId, name, path, providerId);

        // Assert
        Assert.Equal(directoryId, @event.DirectoryId);
        Assert.Equal(name, @event.Name);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void DirectoryDeletedEvent_Constructor_SetsProperties()
    {
        // Arrange
        string directoryId = "dir-1";
        string path = "blog/test";
        string providerId = "test-provider";
        bool recursive = true;

        // Act
        var @event = new DirectoryDeletedEvent(directoryId, path, providerId, recursive);

        // Assert
        Assert.Equal(directoryId, @event.DirectoryId);
        Assert.Equal(path, @event.Path);
        Assert.Equal(providerId, @event.ProviderId);
        Assert.Equal(recursive, @event.Recursive);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    private class TestDomainEvent : DomainEvent
    {
    }
}