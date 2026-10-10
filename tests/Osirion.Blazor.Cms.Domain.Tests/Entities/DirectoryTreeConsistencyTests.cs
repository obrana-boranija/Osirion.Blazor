using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

public class DirectoryTreeConsistencyTests
{
    private static DirectoryItem Dir(string id) => DirectoryItem.Create(id, id, id, "provider");

    private static ContentItem Item(string id) => ContentItem.Create(id, "Title " + id, "Body", id + ".md", "provider");

    [Fact]
    public void AddChild_MovingChildToNewParent_RemovesItFromPreviousParent()
    {
        var first = Dir("first");
        var second = Dir("second");
        var child = Dir("child");
        first.AddChild(child);

        second.AddChild(child);

        Assert.Empty(first.Children);
        Assert.Same(child, Assert.Single(second.Children));
        Assert.Same(second, child.Parent);
    }

    [Fact]
    public void SetParent_ListsTheDirectoryUnderTheNewParent()
    {
        var parent = Dir("parent");
        var child = Dir("child");

        child.SetParent(parent);

        Assert.Same(child, Assert.Single(parent.Children));
    }

    [Fact]
    public void SetParent_ToAnotherParent_MovesTheChild()
    {
        var first = Dir("first");
        var second = Dir("second");
        var child = Dir("child");
        child.SetParent(first);

        child.SetParent(second);

        Assert.Empty(first.Children);
        Assert.Same(child, Assert.Single(second.Children));
    }

    [Fact]
    public void SetParent_WithNull_RemovesTheDirectoryFromTheOldParentChildren()
    {
        var parent = Dir("parent");
        var child = Dir("child");
        parent.AddChild(child);

        child.SetParent(null);

        Assert.Empty(parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void SetParent_WithSameParentTwice_DoesNotDuplicateTheChild()
    {
        var parent = Dir("parent");
        var child = Dir("child");

        child.SetParent(parent);
        child.SetParent(parent);

        Assert.Single(parent.Children);
    }

    [Fact]
    public void RemoveChild_ClearsTheChildParent()
    {
        var parent = Dir("parent");
        var child = Dir("child");
        parent.AddChild(child);

        parent.RemoveChild(child);

        Assert.Empty(parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void Clone_ReparentedClone_DoesNotRemoveTheOriginalFromItsParent()
    {
        var oldParent = Dir("old");
        var newParent = Dir("new");
        var original = Dir("child");
        oldParent.AddChild(original);

        var clone = original.Clone();
        clone.SetParent(newParent);

        Assert.Same(original, Assert.Single(oldParent.Children));
        Assert.Same(clone, Assert.Single(newParent.Children));
    }

    [Fact]
    public void AddItem_MovingItemToAnotherDirectory_RemovesItFromThePreviousDirectory()
    {
        var first = Dir("first");
        var second = Dir("second");
        var item = Item("a");
        first.AddItem(item);

        second.AddItem(item);

        Assert.Empty(first.Items);
        Assert.Same(item, Assert.Single(second.Items));
        Assert.Same(second, item.Directory);
    }

    [Fact]
    public void ContentItemDirectory_Assignment_UpdatesBothDirectories()
    {
        var first = Dir("first");
        var second = Dir("second");
        var item = Item("a");
        item.Directory = first;

        item.Directory = second;

        Assert.Empty(first.Items);
        Assert.Same(item, Assert.Single(second.Items));
    }

    [Fact]
    public void SetDirectory_WithNull_RemovesTheItemFromTheDirectory()
    {
        var directory = Dir("dir");
        var item = Item("a");
        directory.AddItem(item);

        item.SetDirectory(null);

        Assert.Empty(directory.Items);
        Assert.Null(item.Directory);
    }

    [Fact]
    public void RemoveItem_ClearsTheItemDirectory()
    {
        var directory = Dir("dir");
        var item = Item("a");
        directory.AddItem(item);

        directory.RemoveItem(item);

        Assert.Empty(directory.Items);
        Assert.Null(item.Directory);
    }

    [Fact]
    public void ContentItemClone_KeepsTheDirectoryWithoutListingTheCloneTwice()
    {
        var directory = Dir("dir");
        var item = Item("a");
        directory.AddItem(item);

        var clone = item.Clone();

        Assert.Same(directory, clone.Directory);
        Assert.Same(item, Assert.Single(directory.Items));
    }

    [Fact]
    public void SetParent_ToAHigherAncestor_MovesTheDirectoryUp()
    {
        var root = Dir("root");
        var middle = Dir("middle");
        var leaf = Dir("leaf");
        root.AddChild(middle);
        middle.AddChild(leaf);

        leaf.SetParent(root);

        Assert.Empty(middle.Children);
        Assert.Contains(leaf, root.Children);
    }

    [Fact]
    public void DomainEntityBase_IsMarkedObsolete()
    {
#pragma warning disable CS0618 // The test inspects the obsolete marker itself
        Assert.NotNull(Attribute.GetCustomAttribute(typeof(DomainEntityBase<>), typeof(ObsoleteAttribute)));
#pragma warning restore CS0618
    }
}
