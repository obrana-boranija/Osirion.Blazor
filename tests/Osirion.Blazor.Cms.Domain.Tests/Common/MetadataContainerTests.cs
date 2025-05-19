using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class MetadataContainerTests
{
    [Fact]
    public void Constructor_CreatesEmptyContainer()
    {
        // Act
        var container = new MetadataContainer();

        // Assert
        Assert.Empty(container.Values);
    }

    [Fact]
    public void GetValue_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "test-key";
        string value = "test-value";
        container.SetValue(key, value);

        // Act
        var result = container.GetValue<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void GetValue_WithMissingKey_ReturnsDefault()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "non-existent-key";

        // Act
        var result = container.GetValue<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_WithTypeConversion_ConvertsValueWhenPossible()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "test-key";
        container.SetValue(key, "42");

        // Act
        var result = container.GetValue<int>(key);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetValue_WithDefaultValue_ReturnsDefaultWhenKeyNotFound()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "non-existent-key";
        int defaultValue = 42;

        // Act
        var result = container.GetValue(key, defaultValue);

        // Assert
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void SetValue_AddsNewKeyValue_ValuesContainsKey()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "test-key";
        string value = "test-value";

        // Act
        container.SetValue(key, value);

        // Assert
        Assert.True(container.Values.ContainsKey(key));
        Assert.Equal(value, container.Values[key]);
    }

    [Fact]
    public void SetValue_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = null;
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => container.SetValue(key, value));
    }

    [Fact]
    public void SetValue_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "";
        string value = "test-value";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => container.SetValue(key, value));
    }

    [Fact]
    public void SetValue_WithNullValue_RemovesKey()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "test-key";
        string value = "test-value";
        container.SetValue(key, value);

        // Act
        container.SetValue<string>(key, null);

        // Assert
        Assert.False(container.Values.ContainsKey(key));
    }

    [Fact]
    public void AddRange_WithValidValues_AddsAllValues()
    {
        // Arrange
        var container = new MetadataContainer();
        var values = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", 42 },
            { "key3", true }
        };

        // Act
        container.AddRange(values);

        // Assert
        Assert.Equal(3, container.Values.Count);
        Assert.Equal("value1", container.GetValue<string>("key1"));
        Assert.Equal(42, container.GetValue<int>("key2"));
        Assert.True(container.GetValue<bool>("key3"));
    }

    [Fact]
    public void AddRange_WithNullDictionary_DoesNothing()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("existing", "value");

        // Act
        container.AddRange(null);

        // Assert
        Assert.Single(container.Values);
        Assert.Equal("value", container.GetValue<string>("existing"));
    }

    [Fact]
    public void Clear_RemovesAllValues()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("key1", "value1");
        container.SetValue("key2", "value2");

        // Act
        container.Clear();

        // Assert
        Assert.Empty(container.Values);
    }

    [Fact]
    public void ContainsKey_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        var container = new MetadataContainer();
        string key = "test-key";
        container.SetValue(key, "value");

        // Act
        var result = container.ContainsKey(key);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        var result = container.ContainsKey("non-existent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = new MetadataContainer();
        original.SetValue("key1", "value1");
        original.SetValue("key2", 42);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(2, clone.Values.Count);
        Assert.Equal("value1", clone.GetValue<string>("key1"));
        Assert.Equal(42, clone.GetValue<int>("key2"));

        // Modifying clone should not affect original
        clone.SetValue("key3", "value3");
        Assert.Equal(3, clone.Values.Count);
        Assert.Equal(2, original.Values.Count);
        Assert.False(original.ContainsKey("key3"));
    }

    [Fact]
    public void SetPropertyValue_SetsValueWithGivenPropertyName()
    {
        // Arrange
        var container = new MetadataContainer();
        string propertyName = "PropertyName";
        string value = "PropertyValue";

        // Act
        container.SetPropertyValue(propertyName, value);

        // Assert
        Assert.Equal(value, container.GetValue<string>(propertyName));
    }

    [Fact]
    public void SetPropertyValue_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var container = new MetadataContainer();
        string propertyName = "";
        string value = "PropertyValue";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => container.SetPropertyValue(propertyName, value));
    }

    [Fact]
    public void GetPropertyValue_RetrievesValueWithGivenPropertyName()
    {
        // Arrange
        var container = new MetadataContainer();
        string propertyName = "PropertyName";
        string value = "PropertyValue";
        container.SetPropertyValue(propertyName, value);

        // Act
        var result = container.GetPropertyValue<string>(propertyName);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void HasProperty_WithExistingProperty_ReturnsTrue()
    {
        // Arrange
        var container = new MetadataContainer();
        string propertyName = "PropertyName";
        container.SetPropertyValue(propertyName, "value");

        // Act
        var result = container.HasProperty(propertyName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasProperty_WithNonExistentProperty_ReturnsFalse()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        var result = container.HasProperty("NonExistentProperty");

        // Assert
        Assert.False(result);
    }
}