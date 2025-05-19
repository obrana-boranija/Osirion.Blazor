using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class ContentTagTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCorrectTag()
    {
        // Arrange & Act
        var tag = ContentTag.Create(
            name: "JavaScript",
            slug: "javascript",
            count: 5,
            url: "/tags/javascript",
            color: "#f7df1e",
            isFeatured: true,
            group: "Programming"
        );

        // Assert
        Assert.Equal("JavaScript", tag.Name);
        Assert.Equal("javascript", tag.Slug);
        Assert.Equal(5, tag.Count);
        Assert.Equal("/tags/javascript", tag.Url);
        Assert.Equal("#f7df1e", tag.Color);
        Assert.True(tag.IsFeatured);
        Assert.Equal("Programming", tag.Group);
    }

    [Fact]
    public void WithCount_ShouldReturnNewInstanceWithUpdatedCount()
    {
        // Arrange
        var tag = ContentTag.Create(name: "Test", slug: "test", count: 0);

        // Act
        var updated = tag.WithCount(10);

        // Assert
        Assert.Equal(10, updated.Count);
        Assert.Equal(0, tag.Count); // Original should be unchanged
        Assert.NotSame(tag, updated); // Should be different instances
    }

    [Fact]
    public void WithUrl_ShouldReturnNewInstanceWithUpdatedUrl()
    {
        // Arrange
        var tag = ContentTag.Create(name: "Test", slug: "test");

        // Act
        var updated = tag.WithUrl("/new/url");

        // Assert
        Assert.Equal("/new/url", updated.Url);
        Assert.Equal(string.Empty, tag.Url); // Original should be unchanged
    }

    [Fact]
    public void WithColor_ShouldReturnNewInstanceWithUpdatedColor()
    {
        // Arrange
        var tag = ContentTag.Create(name: "Test", slug: "test");

        // Act
        var updated = tag.WithColor("#ff0000");

        // Assert
        Assert.Equal("#ff0000", updated.Color);
        Assert.Null(tag.Color); // Original should be unchanged
    }

    [Fact]
    public void WithFeatured_ShouldReturnNewInstanceWithUpdatedFeaturedStatus()
    {
        // Arrange
        var tag = ContentTag.Create(name: "Test", slug: "test", isFeatured: false);

        // Act
        var updated = tag.WithFeatured(true);

        // Assert
        Assert.True(updated.IsFeatured);
        Assert.False(tag.IsFeatured); // Original should be unchanged
    }

    [Fact]
    public void WithGroup_ShouldReturnNewInstanceWithUpdatedGroup()
    {
        // Arrange
        var tag = ContentTag.Create(name: "Test", slug: "test");

        // Act
        var updated = tag.WithGroup("NewGroup");

        // Assert
        Assert.Equal("NewGroup", updated.Group);
        Assert.Null(tag.Group); // Original should be unchanged
    }

    [Fact]
    public void Clone_ShouldCreateNewInstanceWithSameValues()
    {
        // Arrange
        var original = ContentTag.Create(
            name: "Original",
            slug: "original",
            count: 42,
            url: "/tags/original",
            color: "#0066cc",
            isFeatured: true,
            group: "TestGroup"
        );

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Slug, clone.Slug);
        Assert.Equal(original.Count, clone.Count);
        Assert.Equal(original.Url, clone.Url);
        Assert.Equal(original.Color, clone.Color);
        Assert.Equal(original.IsFeatured, clone.IsFeatured);
        Assert.Equal(original.Group, clone.Group);
    }

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        // Arrange
        var tag1 = ContentTag.Create(
            name: "Test",
            slug: "test",
            url: "/tags/test",
            color: "#0066cc",
            isFeatured: true,
            group: "TestGroup"
        );

        var tag2 = ContentTag.Create(
            name: "Test",
            slug: "test",
            url: "/tags/test",
            color: "#0066cc",
            isFeatured: true,
            group: "TestGroup"
        );

        // Count is excluded from equality comparison, so we set different count values
        tag2 = tag2.WithCount(100);

        // Act & Assert
        Assert.Equal(tag1, tag2);
        Assert.Equal(tag1.GetHashCode(), tag2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var tag1 = ContentTag.Create(name: "Test1", slug: "test1");
        var tag2 = ContentTag.Create(name: "Test2", slug: "test2");

        // Act & Assert
        Assert.NotEqual(tag1, tag2);
        Assert.NotEqual(tag1.GetHashCode(), tag2.GetHashCode());
    }

    [Fact]
    public void GetEqualityComponents_ShouldExcludeCount()
    {
        // Arrange
        var tag1 = ContentTag.Create(name: "Test", slug: "test", count: 5);
        var tag2 = ContentTag.Create(name: "Test", slug: "test", count: 10);

        // Act & Assert
        Assert.Equal(tag1, tag2); // Should be equal despite different counts
        Assert.Equal(tag1.GetHashCode(), tag2.GetHashCode());
    }
}