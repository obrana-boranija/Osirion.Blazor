using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class ContentCategoryTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCorrectCategory()
    {
        // Arrange & Act
        var category = ContentCategory.Create(
            name: "Technology",
            slug: "technology",
            description: "Tech articles",
            url: "/categories/technology",
            color: "#0066cc",
            icon: "laptop",
            order: 1,
            isFeatured: true
        );

        // Assert
        Assert.Equal("Technology", category.Name);
        Assert.Equal("technology", category.Slug);
        Assert.Equal("Tech articles", category.Description);
        Assert.Equal("/categories/technology", category.Url);
        Assert.Equal("#0066cc", category.Color);
        Assert.Equal("laptop", category.Icon);
        Assert.Equal(1, category.Order);
        Assert.True(category.IsFeatured);
    }

    [Fact]
    public void Create_WithOnlyName_ShouldGenerateSlugAndDefaultValues()
    {
        // Arrange & Act
        var category = ContentCategory.Create(name: "Test Category");

        // Assert
        Assert.Equal("Test Category", category.Name);
        Assert.Equal("test-category", category.Slug);
        Assert.Equal(0, category.Count);
        Assert.Null(category.Description);
        Assert.Equal(string.Empty, category.Url);
        Assert.Null(category.Color);
        Assert.Null(category.Icon);
        Assert.Equal(0, category.Order);
        Assert.False(category.IsFeatured);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ContentCategory.Create(name: ""));
        Assert.Contains("Category name cannot be empty", exception.Message);
    }

    [Fact]
    public void WithName_ValidName_ShouldReturnNewInstanceWithUpdatedName()
    {
        // Arrange
        var category = ContentCategory.Create(name: "Original");

        // Act
        var updated = category.WithName("Updated");

        // Assert
        Assert.Equal("Updated", updated.Name);
        Assert.Equal("Original", category.Name); // Original should be unchanged
        Assert.NotSame(category, updated); // Should be different instances
    }

    [Fact]
    public void WithSlug_ValidSlug_ShouldReturnNewInstanceWithUpdatedSlug()
    {
        // Arrange
        var category = ContentCategory.Create(name: "Test", slug: "test");

        // Act
        var updated = category.WithSlug("new-slug");

        // Assert
        Assert.Equal("new-slug", updated.Slug);
        Assert.Equal("test", category.Slug); // Original should be unchanged
    }

    [Fact]
    public void WithSlug_InvalidSlug_ShouldThrowArgumentException()
    {
        // Arrange
        var category = ContentCategory.Create(name: "Test");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => category.WithSlug("Invalid Slug With Spaces"));
        Assert.Contains("Category slug contains invalid characters", exception.Message);
    }

    [Fact]
    public void IsAncestorOf_DirectParentChild_ShouldReturnTrue()
    {
        // Arrange
        var parent = ContentCategory.Create(name: "Parent");
        var child = ContentCategory.Create(name: "Child").WithParent(parent);

        // Act & Assert
        Assert.True(parent.IsAncestorOf(child));
        Assert.False(child.IsAncestorOf(parent));
    }

    [Fact]
    public void IsAncestorOf_DeepHierarchy_ShouldTraverseCorrectly()
    {
        // Arrange
        var grandparent = ContentCategory.Create(name: "Grandparent");
        var parent = ContentCategory.Create(name: "Parent").WithParent(grandparent);
        var child = ContentCategory.Create(name: "Child").WithParent(parent);

        // Act & Assert
        Assert.True(grandparent.IsAncestorOf(child));
        Assert.True(parent.IsAncestorOf(child));
        Assert.False(child.IsAncestorOf(parent));
        Assert.False(child.IsAncestorOf(grandparent));
    }

    [Fact]
    public void WithParent_CircularReference_ShouldThrowArgumentException()
    {
        // Arrange
        var parent = ContentCategory.Create(name: "Parent");
        var child = ContentCategory.Create(name: "Child").WithParent(parent);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parent.WithParent(child));
        Assert.Contains("Cannot set a child category as parent", exception.Message);
    }

    [Fact]
    public void Clone_ShouldCreateNewInstanceWithSameValues()
    {
        // Arrange
        var original = ContentCategory.Create(
            name: "Original",
            slug: "original",
            description: "Description",
            color: "#ff0000"
        );

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Slug, clone.Slug);
        Assert.Equal(original.Description, clone.Description);
        Assert.Equal(original.Color, clone.Color);
    }

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        // Arrange
        var category1 = ContentCategory.Create(
            name: "Test",
            slug: "test",
            description: "Description"
        );

        var category2 = ContentCategory.Create(
            name: "Test",
            slug: "test",
            description: "Description"
        );

        // Act & Assert
        Assert.Equal(category1, category2);
        Assert.Equal(category1.GetHashCode(), category2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var category1 = ContentCategory.Create(name: "Test1", slug: "test1");
        var category2 = ContentCategory.Create(name: "Test2", slug: "test2");

        // Act & Assert
        Assert.NotEqual(category1, category2);
        Assert.NotEqual(category1.GetHashCode(), category2.GetHashCode());
    }

    [Fact]
    public void MetadataOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var category = ContentCategory.Create(name: "Test");

        // Act
        var withMetadata = category.WithMetadata("customKey", "customValue");

        // Assert
        Assert.Equal("customValue", withMetadata.GetMetadata<string>("customKey"));
        Assert.Null(category.GetMetadata<string>("customKey")); // Original shouldn't have the metadata
    }
}