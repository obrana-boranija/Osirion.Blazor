using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Tests.Entities;

public class ContentItemMutationTests
{
    private static ContentItem NewItem() => ContentItem.Create("id-1", "Title", "body", "/a.md", "provider");

    [Fact]
    public void SettersThatTrackChanges_UpdateValueAndLastModified()
    {
        var item = NewItem();

        item.SetDescription("Desc");
        item.SetFeaturedImage("/f.png");
        item.SetFeatured(true);
        item.SetAuthor("Ana");
        item.SetLocale("sr");

        Assert.Equal("Desc", item.Description);
        Assert.Equal("/f.png", item.FeaturedImageUrl);
        Assert.True(item.IsFeatured);
        Assert.Equal("Ana", item.Author);
        Assert.Equal("sr", item.Locale);
        Assert.NotNull(item.LastModified);
    }

    [Fact]
    public void SettersThatDoNotTrackChanges_LeaveLastModifiedEmpty()
    {
        var item = NewItem();
        var created = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc);

        item.SetUrl("/a");
        item.SetOriginalMarkdown("# md");
        item.SetContentId("cid");
        item.SetCreatedDate(created);
        item.SetOrderIndex(5);

        Assert.Equal("/a", item.Url);
        Assert.Equal("# md", item.OriginalMarkdown);
        Assert.Equal("cid", item.ContentId);
        Assert.Equal(created, item.DateCreated);
        Assert.Equal(5, item.OrderIndex);
        Assert.Null(item.LastModified);
    }

    [Fact]
    public void SetLastModifiedDate_StoresTheDate()
    {
        var item = NewItem();
        var date = new DateTime(2021, 6, 7, 0, 0, 0, DateTimeKind.Utc);

        item.SetLastModifiedDate(date);

        Assert.Equal(date, item.LastModified);
    }

    [Theory]
    [InlineData(ContentStatus.Published, ContentStatus.Draft)]
    [InlineData(ContentStatus.Draft, ContentStatus.Draft)]
    [InlineData(ContentStatus.Archived, ContentStatus.Archived)]
    public void IsPublished_SetFalse_OnlyTurnsAPublishedItemIntoADraft(ContentStatus initial, ContentStatus expected)
    {
        var item = NewItem();
        item.Status = initial;

        item.IsPublished = false;

        Assert.Equal(expected, item.Status);
        Assert.False(item.IsPublished);
    }

    [Fact]
    public void IsPublished_SetTrue_PublishesTheItem()
    {
        var item = NewItem();
        item.Status = ContentStatus.Draft;

        item.IsPublished = true;

        Assert.Equal(ContentStatus.Published, item.Status);
    }

    [Fact]
    public void ReadTimeMinutes_UsesContentWordCount()
    {
        var item = NewItem();
        item.Content = string.Join(' ', Enumerable.Repeat("word", 401));

        Assert.Equal(3, item.ReadTimeMinutes);
    }

    [Fact]
    public void ReadTimeMinutes_EmptyContent_FallsBackToMarkdown()
    {
        var item = NewItem();
        item.Content = " ";
        item.OriginalMarkdown = string.Join('\n', Enumerable.Repeat("word", 250));

        Assert.Equal(2, item.ReadTimeMinutes);
    }

    [Fact]
    public void ReadTimeMinutes_NoText_IsOneMinute()
    {
        var item = NewItem();
        item.Content = string.Empty;

        Assert.Equal(1, item.ReadTimeMinutes);
    }

    [Fact]
    public void PublishDate_WithoutMetadata_IsTheCreationDate()
    {
        var item = NewItem();
        var created = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        item.SetCreatedDate(created);

        Assert.Equal(created, item.PublishDate);
    }

    [Fact]
    public void PublishDate_WithMetadata_UsesThePublishDateValue()
    {
        var item = NewItem();
        var published = new DateTime(2022, 2, 2, 0, 0, 0, DateTimeKind.Utc);
        item.SetMetadata("publish_date", published);

        Assert.Equal(published, item.PublishDate);
    }

    [Fact]
    public void GetMetadata_ConvertibleValue_ReturnsConvertedValue()
    {
        var item = NewItem();
        item.SetMetadata("weight", "7");

        Assert.Equal(7, item.GetMetadata<int>("weight"));
    }

    [Fact]
    public void GetMetadata_InconvertibleValue_ReturnsDefault()
    {
        var item = NewItem();
        item.SetMetadata("weight", "heavy");

        Assert.Equal(-1, item.GetMetadata("WEIGHT", -1));
    }

    [Fact]
    public void SetMetadata_NullValue_RemovesTheKey()
    {
        var item = NewItem();
        item.SetMetadata("weight", 1);

        item.SetMetadata<object?>("weight", null);

        Assert.Equal(0, item.GetMetadata<int>("weight"));
    }

    [Fact]
    public void SetMetadata_BlankKey_ThrowsArgumentException()
    {
        var item = NewItem();

        var exception = Assert.Throws<ArgumentException>(() => item.SetMetadata("", 1));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void ToMarkdown_WithoutMetadata_ReturnsTheContentLine()
    {
        var item = NewItem();

        Assert.Equal("body" + Environment.NewLine, item.ToMarkdown());
    }

    [Fact]
    public void ToMarkdown_WithMetadata_PrependsTheFrontMatter()
    {
        var item = NewItem();
        item.Metadata = FrontMatter.Create("Front");

        var markdown = item.ToMarkdown();

        Assert.StartsWith("---", markdown);
        Assert.Contains("title: Front", markdown);
        Assert.EndsWith("body" + Environment.NewLine, markdown);
    }

    [Fact]
    public void SetSeoMetadata_WithoutMetadata_CreatesFrontMatter()
    {
        var item = NewItem();
        var seo = SeoMetadata.Create("Seo", "Desc");

        item.SetSeoMetadata(seo);

        Assert.NotNull(item.Metadata);
        Assert.Same(seo, item.Metadata!.SeoProperties);
    }

    [Fact]
    public void SetSeoMetadata_Null_ThrowsArgumentNullException()
    {
        var item = NewItem();

        Assert.Throws<ArgumentNullException>(() => item.SetSeoMetadata(null!));
    }

    [Fact]
    public void SetDirectory_NullWhenAlreadyNull_KeepsNoDirectory()
    {
        var item = NewItem();

        item.SetDirectory(null);

        Assert.Null(item.Directory);
    }

    [Fact]
    public void TagsAndCategories_IgnoreDuplicatesAndMissingValues()
    {
        var item = NewItem();

        item.AddTag("Blazor");
        item.AddTag("blazor");
        item.RemoveTag("missing");
        item.RemoveTag(" ");
        item.AddCategory("News");
        item.AddCategory("NEWS");
        item.RemoveCategory("missing");
        item.RemoveCategory("");

        Assert.Equal(["Blazor"], item.Tags);
        Assert.Equal(["News"], item.Categories);
    }

    [Fact]
    public void ClearTagsAndCategories_WhenEmpty_DoNotTouchLastModified()
    {
        var item = NewItem();

        item.ClearTags();
        item.ClearCategories();

        Assert.Null(item.LastModified);
    }

    [Fact]
    public void SetStatus_SameStatus_RaisesNoEvent()
    {
        var item = NewItem();

        item.SetStatus(ContentStatus.Published);

        Assert.Empty(item.DomainEvents);
        Assert.NotNull(item.LastModified);
    }
}
