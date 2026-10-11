using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class ContentCategoryCopyTests
{
    [Fact]
    public void WithMethods_ReturnUpdatedCopiesAndLeaveTheOriginalUnchanged()
    {
        var original = ContentCategory.Create("News");

        var updated = original
            .WithCount(7)
            .WithDescription("Latest")
            .WithUrl("/news")
            .WithColor("#fff")
            .WithIcon("star")
            .WithOrder(3)
            .WithFeatured(true);

        Assert.Equal(0, original.Count);
        Assert.Null(original.Description);
        Assert.Equal(7, updated.Count);
        Assert.Equal("Latest", updated.Description);
        Assert.Equal("/news", updated.Url);
        Assert.Equal("#fff", updated.Color);
        Assert.Equal("star", updated.Icon);
        Assert.Equal(3, updated.Order);
        Assert.True(updated.IsFeatured);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void WithName_BlankName_ThrowsArgumentException(string name)
    {
        var category = ContentCategory.Create("News");

        var exception = Assert.Throws<ArgumentException>(() => category.WithName(name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void WithSlug_BlankSlug_ThrowsArgumentException()
    {
        var category = ContentCategory.Create("News");

        var exception = Assert.Throws<ArgumentException>(() => category.WithSlug(" "));

        Assert.Contains("cannot be empty", exception.Message);
    }

    [Fact]
    public void WithMetadata_BlankKey_ThrowsArgumentException()
    {
        var category = ContentCategory.Create("News");

        var exception = Assert.Throws<ArgumentException>(() => category.WithMetadata(" ", 1));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void WithMetadata_ExposesValueThroughCaseInsensitiveMetadata()
    {
        var category = ContentCategory.Create("News").WithMetadata("Weight", 5);

        Assert.Equal(5, category.Metadata["weight"]);
        Assert.Empty(ContentCategory.Create("News").Metadata);
    }

    [Fact]
    public void GetMetadata_ConvertibleValue_ReturnsConvertedValue()
    {
        var category = ContentCategory.Create("News").WithMetadata("weight", "12");

        Assert.Equal(12, category.GetMetadata<int>("weight"));
    }

    [Fact]
    public void GetMetadata_InconvertibleValue_ReturnsDefault()
    {
        var category = ContentCategory.Create("News").WithMetadata("weight", "heavy");

        Assert.Equal(-1, category.GetMetadata("weight", -1));
    }

    [Fact]
    public void GetMetadata_MissingKey_ReturnsDefault()
    {
        var category = ContentCategory.Create("News");

        Assert.Equal("none", category.GetMetadata("missing", "none"));
    }

    [Fact]
    public void WithChildren_SetsChildrenAndNullClearsThem()
    {
        var child = ContentCategory.Create("Child");
        var parent = ContentCategory.Create("Parent").WithChildren([child]);

        var cleared = parent.WithChildren(null!);

        Assert.Same(child, Assert.Single(parent.Children));
        Assert.Empty(cleared.Children);
    }

    [Fact]
    public void IsAncestorOf_Null_ReturnsFalse()
    {
        var category = ContentCategory.Create("News");

        Assert.False(category.IsAncestorOf(null!));
    }

    [Fact]
    public void IsAncestorOf_Unrelated_ReturnsFalse()
    {
        var category = ContentCategory.Create("News");
        var other = ContentCategory.Create("Other").WithParent(ContentCategory.Create("Root"));

        Assert.False(category.IsAncestorOf(other));
    }

    [Fact]
    public void WithParent_Null_ClearsParent()
    {
        var category = ContentCategory.Create("News").WithParent(ContentCategory.Create("Root"));

        var orphan = category.WithParent(null);

        Assert.Null(orphan.Parent);
    }

    [Fact]
    public void CloneWithChildren_CopiesChildrenAsNewInstances()
    {
        var child = ContentCategory.Create("Child").WithMetadata("k", "v");
        var parent = ContentCategory.Create("Parent").WithChildren([child]);

        var clone = parent.CloneWithChildren();

        var clonedChild = Assert.Single(clone.Children);
        Assert.NotSame(child, clonedChild);
        Assert.Equal(child, clonedChild);
        Assert.Equal("v", clonedChild.GetMetadata<string>("k"));
        Assert.NotSame(parent.Children, clone.Children);
    }

    [Fact]
    public void CloneWithChildren_NoChildren_ReturnsEmptyChildren()
    {
        var category = ContentCategory.Create("Leaf");

        var clone = category.CloneWithChildren();

        Assert.Empty(clone.Children);
        Assert.Equal(category, clone);
    }

    [Fact]
    public void Equals_IgnoresCount()
    {
        var first = ContentCategory.Create("News", count: 1);
        var second = ContentCategory.Create("News", count: 2);

        Assert.Equal(first, second);
    }
}
