using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Tests.Domain.Common;

public class EntityBaseTests
{
    [Fact]
    public void GetMetadata_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "test-key";
        string expectedValue = "test-value";
        entity.SetMetadata(key, expectedValue);

        // Act
        var result = entity.GetMetadata<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetMetadata_WithMissingKey_ReturnsDefault()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "non-existent-key";

        // Act
        var result = entity.GetMetadata<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMetadata_WithInvalidType_ReturnsDefault()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "test-key";
        entity.SetMetadata(key, "test-value");

        // Act
        var result = entity.GetMetadata<int>(key, 42);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetMetadata_WithDefaultValue_ReturnsDefaultWhenKeyNotFound()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "non-existent-key";
        int defaultValue = 42;

        // Act
        var result = entity.GetMetadata(key, defaultValue);

        // Assert
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void SetMetadata_AddsNewKeyValue_MetadataContainsKey()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "test-key";
        string value = "test-value";

        // Act
        entity.SetMetadata(key, value);

        // Assert
        Assert.True(entity.Metadata.ContainsKey(key));
        Assert.Equal(value, entity.Metadata[key]);
    }

    [Fact]
    public void SetMetadata_UpdatesExistingKey_MetadataContainsUpdatedValue()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "test-key";
        string originalValue = "original-value";
        string updatedValue = "updated-value";
        entity.SetMetadata(key, originalValue);

        // Act
        entity.SetMetadata(key, updatedValue);

        // Assert
        Assert.True(entity.Metadata.ContainsKey(key));
        Assert.Equal(updatedValue, entity.Metadata[key]);
    }

    [Fact]
    public void SetMetadata_WithNullValue_RemovesKey()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "test-key";
        string value = "test-value";
        entity.SetMetadata(key, value);

        // Act
        entity.SetMetadata<string>(key, null);

        // Assert
        Assert.False(entity.Metadata.ContainsKey(key));
    }

    [Fact]
    public void SetMetadata_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var entity = new TestEntity();
        string key = "";
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => entity.SetMetadata(key, value));
    }

    [Fact]
    public void Metadata_IsReadOnly_CannotModifyDirectly()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Assert.IsAssignableFrom<IReadOnlyDictionary<string, object>>(entity.Metadata);
    }

    private class TestEntity : EntityBase<string>
    {
        public TestEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}