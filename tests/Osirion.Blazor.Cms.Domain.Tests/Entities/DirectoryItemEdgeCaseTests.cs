using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

public class DirectoryItemEdgeCaseTests
{
    private static DirectoryItem Directory(string id) => DirectoryItem.Create(id, $"/{id}", id, "provider");

    private static ContentItem Content(string id) => ContentItem.Create(id, $"Title {id}", "body", $"/{id}.md", "provider");

    [Fact]
    public void Setters_UpdateTheirProperties()
    {
        var directory = Directory("docs");

        directory.SetLocale("sr");
        directory.SetOrder(9);
        directory.SetFeaturedImage("/cover.png");

        Assert.Equal("sr", directory.Locale);
        Assert.Equal(9, directory.Order);
        Assert.Equal("/cover.png", directory.FeaturedImageUrl);
    }

    [Fact]
    public void SetMetadata_FrontMatter_StoresAndClearsIt()
    {
        var directory = Directory("docs");
        var frontMatter = FrontMatter.Create("Docs");

        directory.SetMetadata(frontMatter);
        var stored = directory.Metadata;
        directory.SetMetadata((FrontMatter?)null);

        Assert.Same(frontMatter, stored);
        Assert.Null(directory.Metadata);
    }

    [Fact]
    public void SetMetadata_Value_IsVisibleInCaseInsensitiveCustomMetadata()
    {
        var directory = Directory("docs");

        directory.SetMetadata("Icon", "book");

        Assert.Equal("book", directory.CustomMetadata["icon"]);
    }

    [Fact]
    public void SetMetadata_NullValue_RemovesTheKey()
    {
        var directory = Directory("docs");
        directory.SetMetadata("icon", "book");

        directory.SetMetadata<string?>("icon", null);

        Assert.Empty(directory.CustomMetadata);
    }

    [Fact]
    public void SetMetadata_BlankKey_ThrowsArgumentException()
    {
        var directory = Directory("docs");

        var exception = Assert.Throws<ArgumentException>(() => directory.SetMetadata(" ", 1));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void GetMetadata_TypedValue_ReturnsIt()
    {
        var directory = Directory("docs");
        directory.SetMetadata("weight", 4);

        Assert.Equal(4, directory.GetMetadata<int>("weight"));
    }

    [Fact]
    public void GetMetadata_ConvertibleValue_ReturnsConvertedValue()
    {
        var directory = Directory("docs");
        directory.SetMetadata("weight", "4");

        Assert.Equal(4, directory.GetMetadata<int>("weight"));
    }

    [Fact]
    public void GetMetadata_InconvertibleValue_ReturnsDefault()
    {
        var directory = Directory("docs");
        directory.SetMetadata("weight", "heavy");

        Assert.Equal(-1, directory.GetMetadata("weight", -1));
    }

    [Fact]
    public void GetMetadata_MissingKey_ReturnsDefault()
    {
        var directory = Directory("docs");

        Assert.Equal("none", directory.GetMetadata("missing", "none"));
    }

    [Fact]
    public void AddChild_Null_ThrowsArgumentNullException()
    {
        var directory = Directory("docs");

        Assert.Throws<ArgumentNullException>(() => directory.AddChild(null!));
    }

    [Fact]
    public void AddChild_OtherInstanceWithListedId_KeepsTheListedChild()
    {
        var parent = Directory("root");
        var child = Directory("child");
        var duplicate = Directory("child");
        parent.AddChild(child);

        parent.AddChild(duplicate);

        Assert.Same(child, Assert.Single(parent.Children));
        Assert.Null(duplicate.Parent);
    }

    [Fact]
    public void RemoveChild_Null_LeavesChildrenUnchanged()
    {
        var parent = Directory("root");
        parent.AddChild(Directory("child"));

        parent.RemoveChild(null!);

        Assert.Single(parent.Children);
    }

    [Fact]
    public void RemoveChild_NotListed_LeavesChildrenUnchanged()
    {
        var parent = Directory("root");
        var child = Directory("child");
        parent.AddChild(child);
        var stranger = Directory("stranger");
        var otherParent = Directory("other");
        otherParent.AddChild(stranger);

        parent.RemoveChild(stranger);

        Assert.Single(parent.Children);
        Assert.Same(otherParent, stranger.Parent);
    }

    [Fact]
    public void RemoveChild_OtherInstanceWithListedId_RemovesTheListedChild()
    {
        var parent = Directory("root");
        var child = Directory("child");
        parent.AddChild(child);
        var sameId = Directory("child");

        parent.RemoveChild(sameId);

        Assert.Empty(parent.Children);
        Assert.Null(child.Parent);
        Assert.Null(sameId.Parent);
    }

    [Fact]
    public void AddItem_Null_ThrowsArgumentNullException()
    {
        var directory = Directory("docs");

        Assert.Throws<ArgumentNullException>(() => directory.AddItem(null!));
    }

    [Fact]
    public void AddItem_OtherInstanceWithListedId_KeepsTheListedItem()
    {
        var directory = Directory("docs");
        var item = Content("a");
        var duplicate = Content("a");
        directory.AddItem(item);

        directory.AddItem(duplicate);

        Assert.Same(item, Assert.Single(directory.Items));
        Assert.Null(duplicate.Directory);
    }

    [Fact]
    public void RemoveItem_Null_LeavesItemsUnchanged()
    {
        var directory = Directory("docs");
        directory.AddItem(Content("a"));

        directory.RemoveItem(null!);

        Assert.Single(directory.Items);
    }

    [Fact]
    public void RemoveItem_NotListed_LeavesItemsUnchanged()
    {
        var directory = Directory("docs");
        directory.AddItem(Content("a"));

        directory.RemoveItem(Content("b"));

        Assert.Single(directory.Items);
    }

    [Fact]
    public void RemoveItem_OtherInstanceWithListedId_RemovesTheListedItem()
    {
        var directory = Directory("docs");
        var item = Content("a");
        directory.AddItem(item);

        directory.RemoveItem(Content("a"));

        Assert.Empty(directory.Items);
        Assert.Null(item.Directory);
    }

    [Fact]
    public void IsAncestorOf_Null_ReturnsFalse()
    {
        var directory = Directory("docs");

        Assert.False(directory.IsAncestorOf(null!));
    }

    [Fact]
    public void IsAncestorOf_CircularParentChain_ReturnsFalse()
    {
        var first = Directory("first");
        var second = Directory("second");
        first.SetParent(second, skipCircularCheck: true);
        second.SetParent(first, skipCircularCheck: true);
        var outsider = Directory("outsider");

        Assert.False(outsider.IsAncestorOf(first));
    }

    [Fact]
    public void Clone_CopiesScalarValuesAndCustomMetadata()
    {
        var parent = Directory("root");
        var directory = Directory("docs");
        directory.SetParent(parent);
        directory.SetDescription("Guides");
        directory.SetUrl("/docs");
        directory.SetLocale("sr");
        directory.SetOrder(2);
        directory.SetProviderSpecificId("sha");
        directory.SetMetadata("icon", "book");

        var clone = directory.Clone();

        Assert.Equal(directory.Id, clone.Id);
        Assert.Equal("Guides", clone.Description);
        Assert.Equal("/docs", clone.Url);
        Assert.Equal("sr", clone.Locale);
        Assert.Equal(2, clone.Order);
        Assert.Equal("sha", clone.ProviderSpecificId);
        Assert.Same(parent, clone.Parent);
        Assert.Equal("book", clone.CustomMetadata["icon"]);
    }
}
