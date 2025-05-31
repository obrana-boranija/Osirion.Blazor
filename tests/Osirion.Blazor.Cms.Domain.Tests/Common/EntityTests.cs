using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Events;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class EntityTests
{
    [Fact]
    public void Constructor_WithId_SetsId()
    {
        // Arrange
        string id = "test-id";

        // Act
        var entity = new TestEntity(id);

        // Assert
        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void DomainEvents_InitiallyEmpty()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_AddsEventToDomainEvents()
    {
        // Arrange
        var entity = new TestEntity();
        var @event = new TestDomainEvent();

        // Act
        entity.AddDomainEvent(@event);

        // Assert
        Assert.Single(entity.DomainEvents);
        Assert.Contains(@event, entity.DomainEvents);
    }

    [Fact]
    public void RemoveDomainEvent_RemovesEventFromDomainEvents()
    {
        // Arrange
        var entity = new TestEntity();
        var @event = new TestDomainEvent();
        entity.AddDomainEvent(@event);

        // Act
        entity.RemoveDomainEvent(@event);

        // Assert
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var entity = new TestEntity();
        entity.AddDomainEvent(new TestDomainEvent());
        entity.AddDomainEvent(new TestDomainEvent());

        // Act
        entity.ClearDomainEvents();

        // Assert
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        // Arrange
        string id = "test-id";
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity("id1");
        var entity2 = new TestEntity("id2");

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.False(entity1 == entity2);
        Assert.True(entity1 != entity2);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity("id");

        // Act & Assert
        Assert.False(entity.Equals(null));
        Assert.False(entity is null);
        Assert.True(entity is not null);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity("id");
        var obj = new object();

        // Act & Assert
        Assert.False(entity.Equals(obj));
    }

    [Fact]
    public void GetHashCode_SameId_ReturnsSameHashCode()
    {
        // Arrange
        string id = "test-id";
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentId_ReturnsDifferentHashCode()
    {
        // Arrange
        var entity1 = new TestEntity("id1");
        var entity2 = new TestEntity("id2");

        // Act & Assert
        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }

    private class TestEntity : Entity<string>
    {
        public TestEntity() { }

        public TestEntity(string id) : base(id) { }

        public new void AddDomainEvent(IDomainEvent domainEvent) => base.AddDomainEvent(domainEvent);
        public new void RemoveDomainEvent(IDomainEvent domainEvent) => base.RemoveDomainEvent(domainEvent);
        public new void ClearDomainEvents() => base.ClearDomainEvents();
    }

    private class TestDomainEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}