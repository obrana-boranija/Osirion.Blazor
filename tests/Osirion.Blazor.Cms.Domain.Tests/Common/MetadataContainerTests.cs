using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class MetadataContainerTests
{
    [Fact]
    public void GetValue_ExistingValue_ShouldReturnValue()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("TestKey", "TestValue");

        // Act
        var value = container.GetValue<string>("TestKey");

        // Assert
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void GetValue_NonExistingValue_ShouldReturnDefault()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        var value = container.GetValue<string>("MissingKey");

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public void GetValue_WithExplicitDefault_ShouldReturnDefault()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        var value = container.GetValue<string>("MissingKey", "DefaultValue");

        // Assert
        Assert.Equal("DefaultValue", value);
    }

    [Fact]
    public void GetValue_TypeMismatch_ShouldTryConvert()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("IntKey", 123);

        // Act
        var value = container.GetValue<string>("IntKey");

        // Assert
        Assert.Equal("123", value);
    }

    [Fact]
    public void GetValue_TypeMismatchWithNoConversion_ShouldReturnDefault()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("StringKey", "NotAnInt");

        // Act
        var value = container.GetValue<int>("StringKey", 42);

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void SetValue_NullKey_ShouldThrowArgumentException()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => container.SetValue<string>(null!, "Value"));
    }

    [Fact]
    public void SetValue_EmptyKey_ShouldThrowArgumentException()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => container.SetValue<string>("", "Value"));
    }

    [Fact]
    public void SetValue_NullValue_ShouldRemoveKey()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("TestKey", "TestValue");
        Assert.True(container.ContainsKey("TestKey"));

        // Act
        container.SetValue<string?>("TestKey", null);

        // Assert
        Assert.False(container.ContainsKey("TestKey"));
    }

    [Fact]
    public void Values_ShouldReturnReadOnlyDictionary()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("Key1", "Value1");
        container.SetValue("Key2", 42);

        // Act
        var values = container.Values;

        // Assert
        Assert.Equal(2, values.Count);
        Assert.Equal("Value1", values["Key1"]);
        Assert.Equal(42, values["Key2"]);
    }

    [Fact]
    public void AddRange_ValidDictionary_ShouldAddAllEntries()
    {
        // Arrange
        var container = new MetadataContainer();
        var dictionary = new Dictionary<string, object>
        {
            { "Key1", "Value1" },
            { "Key2", 42 }
        };

        // Act
        container.AddRange(dictionary);

        // Assert
        Assert.Equal(2, container.Values.Count);
        Assert.Equal("Value1", container.GetValue<string>("Key1"));
        Assert.Equal(42, container.GetValue<int>("Key2"));
    }

    [Fact]
    public void AddRange_NullDictionary_ShouldDoNothing()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        container.AddRange(null!);

        // Assert
        Assert.Empty(container.Values);
    }

    [Fact]
    public void Clear_ShouldRemoveAllEntries()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("Key1", "Value1");
        container.SetValue("Key2", 42);
        Assert.Equal(2, container.Values.Count);

        // Act
        container.Clear();

        // Assert
        Assert.Empty(container.Values);
    }

    [Fact]
    public void ContainsKey_ExistingKey_ShouldReturnTrue()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetValue("TestKey", "TestValue");

        // Act & Assert
        Assert.True(container.ContainsKey("TestKey"));
    }

    [Fact]
    public void ContainsKey_NonExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act & Assert
        Assert.False(container.ContainsKey("MissingKey"));
    }

    [Fact]
    public void Clone_ShouldCreateNewInstanceWithSameValues()
    {
        // Arrange
        var original = new MetadataContainer();
        original.SetValue("Key1", "Value1");
        original.SetValue("Key2", 42);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(2, clone.Values.Count);
        Assert.Equal("Value1", clone.GetValue<string>("Key1"));
        Assert.Equal(42, clone.GetValue<int>("Key2"));
    }

    [Fact]
    public void SetPropertyValue_ShouldSetValueForProperty()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act
        container.SetPropertyValue("PropertyName", "PropertyValue");

        // Assert
        Assert.Equal("PropertyValue", container.GetPropertyValue<string>("PropertyName"));
    }

    [Fact]
    public void HasProperty_ExistingProperty_ShouldReturnTrue()
    {
        // Arrange
        var container = new MetadataContainer();
        container.SetPropertyValue("PropertyName", "PropertyValue");

        // Act & Assert
        Assert.True(container.HasProperty("PropertyName"));
    }

    [Fact]
    public void HasProperty_NonExistingProperty_ShouldReturnFalse()
    {
        // Arrange
        var container = new MetadataContainer();

        // Act & Assert
        Assert.False(container.HasProperty("MissingProperty"));
    }
}