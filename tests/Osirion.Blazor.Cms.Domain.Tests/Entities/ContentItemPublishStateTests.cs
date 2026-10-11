using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

/// <summary>
/// IsPublished and Status used to be two independent values, so a new item had Status Published but IsPublished
/// false and was written to the repository as a draft.
/// </summary>
public class ContentItemPublishStateTests
{
    [Fact]
    public void ContentItem_Created_IsPublished()
    {
        var item = CreateItem();

        item.Status.ShouldBe(ContentStatus.Published);
        item.IsPublished.ShouldBeTrue();
    }

    [Fact]
    public void ContentItem_IsPublishedSetToFalse_BecomesDraft()
    {
        var item = CreateItem();

        item.IsPublished = false;

        item.Status.ShouldBe(ContentStatus.Draft);
    }

    [Fact]
    public void ContentItem_StatusSetToDraft_IsNotPublished()
    {
        var item = CreateItem();

        item.SetStatus(ContentStatus.Draft);

        item.IsPublished.ShouldBeFalse();
    }

    [Fact]
    public void ContentItem_ArchivedItemSetToUnpublished_StaysArchived()
    {
        var item = CreateItem();
        item.SetStatus(ContentStatus.Archived);

        item.IsPublished = false;

        item.Status.ShouldBe(ContentStatus.Archived);
    }

    [Fact]
    public void ContentItem_Clone_KeepsDraftState()
    {
        var item = CreateItem();
        item.IsPublished = false;

        var copy = item.Clone();

        copy.IsPublished.ShouldBeFalse();
        copy.Status.ShouldBe(ContentStatus.Draft);
    }

    private static ContentItem CreateItem() => ContentItem.Create("post-1", "Post", "<p>Post</p>", "blog/post.md", "test");
}
