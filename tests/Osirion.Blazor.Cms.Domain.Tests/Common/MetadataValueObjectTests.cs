using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class MetadataValueObjectTests
{
    // Create a concrete implementation of MetadataValueObject for testing
    private class TestMetadataValueObject : MetadataValueObject
    {
        public string Name { get; }

        public TestMetadataValueObject(string name, Dictionary<string, object>? initialMetadata = null)
            : base(initialMetadata)
        {
            Name = name;
        }

        public TestMetadataValueObject WithMetadata(string key, object value)
        {
            var metadataWithNewValue = CloneMetadataWith(key, value);
            return new TestMetadataValueObject(Name, metadataWithNewValue);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;

            // Include metadata in equality check
            foreach (var component in GetMetadataEqualityComponents())
            {
                yield return component;
            }
        }
    }

    [Fact]
    public void Constructor_WithNoMetadata_ShouldCreateEmptyMetadata()
    {
        // Arrange & Act
        var obj = new TestMetadataValueObject("Test");

        // Assert
        Assert.Empty(obj.Metadata);
    }

    [Fact]
    public void Constructor_WithInitialMetadata_ShouldStoreMetadata()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "Key1", "Value1" },
            { "Key2", 42 }
        };

        // Act
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Assert
        Assert.Equal(2, obj.Metadata.Count);
        Assert.Equal("Value1", obj.Metadata["Key1"]);
        Assert.Equal(42, obj.Metadata["Key2"]);
    }

    [Fact]
    public void GetMetadata_ExistingKey_ShouldReturnValue()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "Key1", "Value1" }
        };
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Act
        var value = obj.GetMetadata<string>("Key1");

        // Assert
        Assert.Equal("Value1", value);
    }

    [Fact]
    public void GetMetadata_MissingKey_ShouldReturnDefault()
    {
        // Arrange
        var obj = new TestMetadataValueObject("Test");

        // Act
        var value = obj.GetMetadata<string>("MissingKey");

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public void GetMetadata_MissingKeyWithExplicitDefault_ShouldReturnDefault()
    {
        // Arrange
        var obj = new TestMetadataValueObject("Test");

        // Act
        var value = obj.GetMetadata<string>("MissingKey", "DefaultValue");

        // Assert
        Assert.Equal("DefaultValue", value);
    }

    [Fact]
    public void GetMetadata_TypeMismatch_ShouldTryConvert()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "IntKey", 123 }
        };
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Act
        var value = obj.GetMetadata<string>("IntKey");

        // Assert
        Assert.Equal("123", value);
    }

    [Fact]
    public void GetMetadata_TypeMismatchWithNoConversion_ShouldReturnDefault()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "StringKey", "NotAnInt" }
        };
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Act
        var value = obj.GetMetadata<int>("StringKey", 42);

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void CloneMetadataWith_ShouldCreateNewDictionaryWithNewValue()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "Key1", "Value1" }
        };
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Act
        var objWithNewMetadata = obj.WithMetadata("Key2", "Value2");

        // Assert
        Assert.Equal(1, obj.Metadata.Count); // Original should be unchanged
        Assert.Equal(2, objWithNewMetadata.Metadata.Count);
        Assert.Equal("Value1", objWithNewMetadata.Metadata["Key1"]);
        Assert.Equal("Value2", objWithNewMetadata.Metadata["Key2"]);
    }

    [Fact]
    public void CloneMetadataWith_ExistingKey_ShouldOverwriteValue()
    {
        // Arrange
        var initialMetadata = new Dictionary<string, object>
        {
            { "Key1", "Value1" }
        };
        var obj = new TestMetadataValueObject("Test", initialMetadata);

        // Act
        var objWithNewMetadata = obj.WithMetadata("Key1", "NewValue");

        // Assert
        Assert.Equal("Value1", obj.Metadata["Key1"]); // Original should be unchanged
        Assert.Equal("NewValue", objWithNewMetadata.Metadata["Key1"]);
    }

    [Fact]
    public void GetEqualityComponents_ShouldIncludeMetadata()
    {
        // Arrange
        var obj1 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "Value1");

        var obj2 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "Value1");

        var obj3 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "DifferentValue");

        // Act & Assert
        Assert.Equal(obj1, obj2);
        Assert.NotEqual(obj1, obj3);
    }

    [Fact]
    public void Equals_WithSameMetadata_ShouldBeEqual()
    {
        // Arrange
        var obj1 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "Value1")
            .WithMetadata("Key2", 42);

        var obj2 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "Value1")
            .WithMetadata("Key2", 42);

        // Act & Assert
        Assert.Equal(obj1, obj2);
        Assert.Equal(obj1.GetHashCode(), obj2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentMetadata_ShouldNotBeEqual()
    {
        // Arrange
        var obj1 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "Value1");

        var obj2 = new TestMetadataValueObject("Test")
            .WithMetadata("Key1", "DifferentValue");

        // Act & Assert
        Assert.NotEqual(obj1, obj2);
        Assert.NotEqual(obj1.GetHashCode(), obj2.GetHashCode());
    }

    [Fact]
    public void GetMetadataEqualityComponents_ShouldOrderByKey()
    {
        // Arrange
        var obj1 = new TestMetadataValueObject("Test")
            .WithMetadata("B", "Value2")
            .WithMetadata("A", "Value1");

        var obj2 = new TestMetadataValueObject("Test")
            .WithMetadata("A", "Value1")
            .WithMetadata("B", "Value2");

        // Act & Assert - Different order of adding metadata but still equal
        Assert.Equal(obj1, obj2);
        Assert.Equal(obj1.GetHashCode(), obj2.GetHashCode());
    }
}