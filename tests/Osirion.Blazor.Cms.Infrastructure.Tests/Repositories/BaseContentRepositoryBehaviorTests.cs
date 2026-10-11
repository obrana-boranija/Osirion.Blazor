using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Repositories;

public class BaseContentRepositoryBehaviorTests
{
    private static readonly DateTime Created = new(2024, 5, 6, 0, 0, 0, DateTimeKind.Utc);

    private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);

    private static ContentItem FullyPopulatedItem()
    {
        var item = new ContentItem
        {
            Path = "blog/post.md",
            ContentId = "c-1",
            OrderIndex = 3,
            Title = "Hello \"World\"",
            Description = "Desc",
            Author = "Ann",
            DateCreated = Created,
            FeaturedImageUrl = "/img.png",
            IsFeatured = true,
            Slug = "hello",
            Locale = "en",
            OriginalMarkdown = "# Body",
        };
        item.AddCategory("News");
        item.AddCategory("Tech");
        item.AddTag("a");
        item.IsPublished = false;
        return item;
    }

    private static Dictionary<string, ContentItem> Cache(params ContentItem[] items) => items.ToDictionary(item => item.Id);

    private static ContentItem Item(string id, string path, params string[] tags)
    {
        var item = ContentItem.Create(id, "Title " + id, "Body", path, "scripted");
        foreach (var tag in tags)
        {
            item.AddTag(tag);
        }

        return item;
    }

    [Fact]
    public async Task GenerateMarkdownWithFrontMatterAsync_NoMetadataAndEveryFieldSet_WritesEachKeyInOrder()
    {
        var repository = new ScriptedContentRepository();

        var markdown = await repository.GenerateAsync(FullyPopulatedItem());

        markdown.ShouldBe(Lines(
            "---",
            "id: \"c-1\"",
            "order: 3",
            "title: \"Hello \\\"World\\\"\"",
            "description: \"Desc\"",
            "author: \"Ann\"",
            "date: 2024-05-06",
            "featured_image: \"/img.png\"",
            "categories:",
            "  - \"News\"",
            "  - \"Tech\"",
            "tags:",
            "  - \"a\"",
            "is_featured: true",
            "published: false",
            "slug: \"hello\"",
            "lang: \"en\"",
            "---",
            "",
            "# Body"));
    }

    [Fact]
    public async Task GenerateMarkdownWithFrontMatterAsync_NoMetadataAndEmptyItem_WritesOnlyTheDate()
    {
        var repository = new ScriptedContentRepository();

        var markdown = await repository.GenerateAsync(new ContentItem());

        markdown.ShouldBe(Lines("---", "date: 0001-01-01", "---", "", ""));
    }

    [Fact]
    public async Task GenerateMarkdownWithFrontMatterAsync_NoOriginalMarkdown_ConvertsTheHtmlBody()
    {
        var processor = Substitute.For<IMarkdownProcessor>();
        processor.ConvertHtmlToMarkdownAsync("<p>x</p>", Arg.Any<CancellationToken>()).Returns("converted body");
        var repository = new ScriptedContentRepository(processor);
        var item = new ContentItem { Content = "<p>x</p>" };

        var markdown = await repository.GenerateAsync(item);

        markdown.ShouldEndWith(Environment.NewLine + "converted body");
    }

    [Fact]
    public async Task GenerateMarkdownWithFrontMatterAsync_WithMetadata_CopiesTheItemIntoTheMetadataAndKeepsOtherKeys()
    {
        var repository = new ScriptedContentRepository();
        var item = FullyPopulatedItem();
        item.Metadata = new FrontMatter { Title = "old", Layout = "post" };

        var markdown = await repository.GenerateAsync(item);

        var metadata = item.Metadata.ShouldNotBeNull();
        metadata.Id.ShouldBe("c-1");
        metadata.Order.ShouldBe(3);
        metadata.Title.ShouldBe("Hello \"World\"");
        metadata.Description.ShouldBe("Desc");
        metadata.Author.ShouldBe("Ann");
        metadata.Date.ShouldBe("2024-05-06");
        metadata.FeaturedImage.ShouldBe("/img.png");
        metadata.IsFeatured.ShouldBeTrue();
        metadata.Slug.ShouldBe("hello");
        metadata.Lang.ShouldBe("en");
        metadata.Published.ShouldBeFalse();
        metadata.Categories.ShouldBe(["News", "Tech"]);
        metadata.Tags.ShouldBe(["a"]);
        metadata.Layout.ShouldBe("post");
        markdown.ShouldBe(metadata.ToYaml() + "# Body");
    }

    [Fact]
    public async Task GenerateMarkdownWithFrontMatter_ObsoleteSynchronousWrapper_ReturnsTheSameMarkdown()
    {
        var repository = new ScriptedContentRepository();

        var synchronous = repository.Generate(FullyPopulatedItem());

        synchronous.ShouldBe(await repository.GenerateAsync(FullyPopulatedItem()));
    }

    [Theory]
    [InlineData("a\\b", "a\\\\b")]
    [InlineData("say \"hi\"", "say \\\"hi\\\"")]
    [InlineData("a\nb", "a\\nb")]
    [InlineData("a\rb", "a\\rb")]
    [InlineData("a\tb", "a\\tb")]
    [InlineData("plain", "plain")]
    public void EscapeYamlString_SpecialCharacter_IsEscaped(string value, string expected)
    {
        var repository = new ScriptedContentRepository();

        repository.Escape(value).ShouldBe(expected);
    }

    [Fact]
    public void EscapeYamlString_Null_Throws()
    {
        var repository = new ScriptedContentRepository();

        Should.Throw<ArgumentNullException>(() => repository.Escape(null!));
    }

    [Theory]
    [InlineData("title", true)]
    [InlineData("FEATURED_IMAGE", true)]
    [InlineData("tag", true)]
    [InlineData("layout", false)]
    [InlineData("custom", false)]
    public void IsStandardFrontMatterKey_Key_ReportsWhetherItIsStandard(string key, bool expected)
    {
        var repository = new ScriptedContentRepository();

        repository.IsStandardKey(key).ShouldBe(expected);
    }

    [Fact]
    public async Task ReadMethods_WhenTheCacheCheckThrows_ReturnEmptyResults()
    {
        var repository = new ScriptedContentRepository { EnsureCacheFailure = new InvalidOperationException("boom") };

        (await repository.GetAllAsync()).ShouldBeEmpty();
        (await repository.GetByIdAsync("x")).ShouldBeNull();
        (await repository.GetByPathAsync("x")).ShouldBeNull();
        (await repository.GetByUrlAsync("x")).ShouldBeNull();
        (await repository.FindByQueryAsync(new ContentQuery())).ShouldNotBeNull().ShouldBeEmpty();
        (await repository.GetByDirectoryAsync("d")).ShouldBeEmpty();
        (await repository.GetTranslationsAsync("c")).ShouldBeEmpty();
        (await repository.GetTagsAsync()).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task LookupMethods_BlankKey_ThrowArgumentException(string key)
    {
        var repository = new ScriptedContentRepository();

        await Should.ThrowAsync<ArgumentException>(() => repository.GetByIdAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByPathAsync(key));
        await Should.ThrowAsync<ArgumentException>(() => repository.GetByUrlAsync(key));
    }

    [Fact]
    public async Task GetTranslationsAsync_BlankContentId_ReturnsEmptyWithoutLoading()
    {
        var repository = new ScriptedContentRepository();

        var result = await repository.GetTranslationsAsync(" ");

        result.ShouldBeEmpty();
        repository.LoadCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetByIdAsync_KnownAndUnknownId_ReturnsTheCachedItemOrNull()
    {
        var item = Item("a", "blog/a.md");
        var repository = new ScriptedContentRepository { Loader = () => Cache(item) };

        (await repository.GetByIdAsync("a")).ShouldBeSameAs(item);
        (await repository.GetByIdAsync("missing")).ShouldBeNull();
        repository.LoadCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetByPathAsync_PathWithBackslashesAndSlashes_MatchesTheNormalizedPath()
    {
        var item = Item("a", "blog/post.md");
        var repository = new ScriptedContentRepository { Loader = () => Cache(item, Item("b", "other.md")) };

        var result = await repository.GetByPathAsync("\\blog\\post.md/");

        result.ShouldBeSameAs(item);
    }

    [Fact]
    public async Task GetByUrlAsync_KnownUrl_ReturnsTheItem()
    {
        var item = Item("a", "blog/a.md");
        item.Url = "blog/a";
        var repository = new ScriptedContentRepository { Loader = () => Cache(item, Item("b", "b.md")) };

        (await repository.GetByUrlAsync("blog/a")).ShouldBeSameAs(item);
        (await repository.GetByUrlAsync("blog/none")).ShouldBeNull();
    }

    [Fact]
    public async Task FindByQueryAsync_DelegatesFilteringToTheQueryFilter()
    {
        var kept = Item("a", "a.md");
        var dropped = Item("b", "b.md");
        var query = new ContentQuery();
        var filter = Substitute.For<IContentQueryFilter>();
        filter.ApplyFilters(Arg.Any<IQueryable<ContentItem>>(), query)
            .Returns(call => call.Arg<IQueryable<ContentItem>>().Where(item => item.Id == "a"));
        var repository = new ScriptedContentRepository(queryFilter: filter) { Loader = () => Cache(kept, dropped) };

        var result = await repository.FindByQueryAsync(query);

        result.ShouldNotBeNull().ShouldHaveSingleItem().ShouldBeSameAs(kept);
    }

    [Fact]
    public async Task GetByDirectoryAsync_ReturnsOnlyItemsOfThatDirectory()
    {
        var blog = DirectoryItem.Create("blog", "blog", "Blog", "scripted");
        var docs = DirectoryItem.Create("docs", "docs", "Docs", "scripted");
        var inBlog = Item("a", "blog/a.md");
        inBlog.SetDirectory(blog);
        var inDocs = Item("b", "docs/b.md");
        inDocs.SetDirectory(docs);
        var repository = new ScriptedContentRepository { Loader = () => Cache(inBlog, inDocs, Item("c", "c.md")) };

        var result = await repository.GetByDirectoryAsync("blog");

        result.ShouldHaveSingleItem().ShouldBeSameAs(inBlog);
    }

    [Fact]
    public async Task GetTranslationsAsync_ReturnsItemsSharingTheContentId()
    {
        var english = Item("en", "en/post.md");
        english.ContentId = "post";
        var german = Item("de", "de/post.md");
        german.ContentId = "post";
        var other = Item("x", "en/other.md");
        other.ContentId = "other";
        var repository = new ScriptedContentRepository { Loader = () => Cache(english, german, other) };

        var result = await repository.GetTranslationsAsync("post");

        result.Select(item => item.Id).OrderBy(id => id).ShouldBe(["de", "en"]);
    }

    [Fact]
    public async Task GetTagsAsync_GroupsTagsIgnoringCaseAndOrdersThemByName()
    {
        var repository = new ScriptedContentRepository
        {
            Loader = () => Cache(Item("a", "a.md", "Zeta", "alpha"), Item("b", "b.md", "zeta"))
        };

        var tags = await repository.GetTagsAsync();

        tags.Select(tag => (tag.Name, tag.Count)).ShouldBe([("alpha", 1), ("Zeta", 2)]);
        tags[1].Slug.ShouldBe("zeta");
    }

    [Fact]
    public async Task GetAllAsync_LoaderReturnsNull_TreatsItAsAnEmptySuccessfulLoad()
    {
        var repository = new ScriptedContentRepository { Loader = () => null };

        var result = await repository.GetAllAsync();

        result.ShouldBeEmpty();
        repository.LastRefreshSucceeded.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsync_AfterAColdLoadFailed_DoesNotRetryOnEveryRead()
    {
        var repository = new ScriptedContentRepository { Loader = () => throw new InvalidOperationException("offline") };

        (await repository.GetAllAsync()).ShouldBeEmpty();
        (await repository.GetAllAsync()).ShouldBeEmpty();

        repository.LoadCount.ShouldBe(1);
        repository.LastRefreshSucceeded.ShouldBeFalse();
    }

    [Fact]
    public async Task RefreshCacheAsync_AfterAColdLoadFailed_LoadsAgain()
    {
        var attempts = 0;
        var repository = new ScriptedContentRepository
        {
            Loader = () => ++attempts == 1 ? throw new InvalidOperationException("offline") : Cache(Item("a", "a.md"))
        };
        await repository.GetAllAsync();

        await repository.RefreshCacheAsync();

        (await repository.GetAllAsync()).ShouldHaveSingleItem().Id.ShouldBe("a");
        repository.LastRefreshSucceeded.ShouldBeTrue();
    }

    [Fact]
    public async Task SaveAsync_NewItem_UsesACreateCommitMessage()
    {
        var repository = new ScriptedContentRepository();
        var item = Item("a", "blog/post.md");

        var saved = await repository.SaveAsync(item);

        saved.ShouldBeSameAs(item);
        repository.Saved.ShouldHaveSingleItem().Message.ShouldBe("Create post.md");
    }

    [Fact]
    public async Task SaveAsync_ExistingItem_UsesAnUpdateCommitMessage()
    {
        var repository = new ScriptedContentRepository();
        var item = Item("a", "blog/post.md");
        item.SetProviderSpecificId("sha-1");

        await repository.SaveAsync(item);

        repository.Saved.ShouldHaveSingleItem().Message.ShouldBe("Update post.md");
    }

    [Fact]
    public async Task SaveAsync_NullItem_Throws()
    {
        var repository = new ScriptedContentRepository();

        await Should.ThrowAsync<ArgumentNullException>(() => repository.SaveAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_UsesADeleteCommitMessageNamingTheId()
    {
        var repository = new ScriptedContentRepository();

        await repository.DeleteAsync("abc");

        repository.Deleted.ShouldHaveSingleItem().ShouldBe(("abc", "Delete content abc"));
    }

    [Fact]
    public void AssignDefaultLocalizationId_PathWithoutAName_LeavesTheContentIdEmpty()
    {
        var repository = new ScriptedContentRepository();
        repository.ConfigureLocalization(string.Empty, "en");
        var item = new ContentItem { Path = ".md" };

        repository.AssignLocalizationId(item);

        item.ContentId.ShouldBeEmpty();
    }

    [Fact]
    public void AssignDefaultLocalizationId_LocalizedPath_SharesTheIdAcrossLocales()
    {
        var repository = new ScriptedContentRepository();
        repository.ConfigureLocalization(string.Empty, "en", "de");
        var english = new ContentItem { Path = "en/blog/post.md" };
        var german = new ContentItem { Path = "de/blog/post.md" };

        repository.AssignLocalizationId(english);
        repository.AssignLocalizationId(german);

        english.ContentId.ShouldNotBeNullOrWhiteSpace();
        german.ContentId.ShouldBe(english.ContentId);
    }

    [Fact]
    public async Task ProcessMarkdownAsync_ProcessorThrows_RethrowsTheError()
    {
        var processor = Substitute.For<IMarkdownProcessor>();
        processor.ExtractFrontMatterAndContent(Arg.Any<string>()).Throws(new FormatException("bad yaml"));
        var repository = new ScriptedContentRepository(processor);

        var exception = await Should.ThrowAsync<FormatException>(() => repository.ProcessAsync("---", new ContentItem { Path = "a.md" }));

        exception.Message.ShouldBe("bad yaml");
    }
}
