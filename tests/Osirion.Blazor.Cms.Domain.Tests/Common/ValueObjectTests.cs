using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class ValueObjectTests
{
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var obj1 = new TestValueObject("Value1", 42);
        var obj2 = new TestValueObject("Value1", 42);

        // Act & Assert
        Assert.True(obj1.Equals(obj2));
        Assert.True(obj1 == obj2);
        Assert.False(obj1 != obj2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var obj1 = new TestValueObject("Value1", 42);
        var obj2 = new TestValueObject("Value2", 42);

        // Act & Assert
        Assert.False(obj1.Equals(obj2));
        Assert.False(obj1 == obj2);
        Assert.True(obj1 != obj2);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var obj = new TestValueObject("Value", 42);

        // Act & Assert
        Assert.False(obj.Equals(null));
        Assert.False(obj is null);
        Assert.True(obj is not null);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        var obj = new TestValueObject("Value", 42);
        var otherObj = new object();

        // Act & Assert
        Assert.False(obj.Equals(otherObj));
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        // Arrange
        var obj1 = new TestValueObject("Value", 42);
        var obj2 = new TestValueObject("Value", 42);

        // Act & Assert
        Assert.Equal(obj1.GetHashCode(), obj2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
    {
        // Arrange
        var obj1 = new TestValueObject("Value1", 42);
        var obj2 = new TestValueObject("Value2", 42);

        // Act & Assert
        Assert.NotEqual(obj1.GetHashCode(), obj2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        // Arrange
        TestValueObject obj1 = null;
        TestValueObject obj2 = null;

        // Act & Assert
        Assert.True(obj1 == obj2);
        Assert.False(obj1 != obj2);
    }

    [Fact]
    public void EqualityOperator_OneNull_ReturnsFalse()
    {
        // Arrange
        TestValueObject obj1 = new TestValueObject("Value", 42);
        TestValueObject obj2 = null;

        // Act & Assert
        Assert.False(obj1 == obj2);
        Assert.False(obj2 == obj1);
        Assert.True(obj1 != obj2);
        Assert.True(obj2 != obj1);
    }

    private class TestValueObject : ValueObject
    {
        public string StringValue { get; }
        public int IntValue { get; }

        public TestValueObject(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
        }
    }
}