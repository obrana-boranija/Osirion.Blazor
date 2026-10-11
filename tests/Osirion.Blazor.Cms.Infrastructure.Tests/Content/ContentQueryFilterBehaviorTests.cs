using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;
using SortDirection = Osirion.Blazor.Cms.Domain.Enums.SortDirection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content;

/// <summary>
/// Pins every filter, the sort order and the paging of <see cref="ContentQueryFilter.ApplyFilters"/>.
/// </summary>
public class ContentQueryFilterBehaviorTests
{
    private const string Provider = "p1";

    private readonly ContentQueryFilter _filter;
    private readonly List<ContentItem> _items;

    public ContentQueryFilterBehaviorTests()
    {
        var pathUtils = Substitute.For<IPathUtilities>();
        pathUtils.NormalizePath(Arg.Any<string>()).Returns(x => x.Arg<string>());
        _filter = new ContentQueryFilter(pathUtils);

        var blog = DirectoryItem.Create("d-blog", "blog", "Blog", Provider);
        var sub = DirectoryItem.Create("d-sub", "blog/sub", "Sub", Provider);
        var docs = DirectoryItem.Create("d-docs", "docs", "Docs", Provider);

        var alpha = CreateItem("a", "Alpha Guide", "blog/alpha.md", "one");
        alpha.Slug = "alpha";
        alpha.Url = "blog/alpha";
        alpha.AddCategory("Web Dev");
        alpha.AddTag("csharp");
        alpha.AddTag("blazor");
        alpha.Author = "Ann";
        alpha.IsFeatured = true;
        alpha.Description = "first";
        alpha.Locale = "en";
        alpha.ContentId = "loc-1";
        alpha.DateCreated = Utc(2024, 1, 10);
        alpha.LastModified = Utc(2024, 3, 1);
        alpha.OrderIndex = 3;
        alpha.SetDirectory(blog);

        var beta = CreateItem("b", "Beta Notes", "blog/sub/beta.md", string.Join(' ', Enumerable.Repeat("word", 450)));
        beta.Slug = "beta";
        beta.Url = "blog/sub/beta";
        beta.AddCategory("Web Dev");
        beta.AddCategory("Tools");
        beta.AddTag("csharp");
        beta.Author = "Bob";
        beta.Description = "Second entry about testing";
        beta.Locale = "fr";
        beta.ContentId = "loc-1";
        beta.DateCreated = Utc(2024, 2, 15);
        beta.LastModified = null;
        beta.OrderIndex = 1;
        beta.SetDirectory(sub);

        var gamma = CreateItem("c", "Gamma Draft", "docs/gamma.md", string.Join(' ', Enumerable.Repeat("word", 250)));
        gamma.Slug = "gamma";
        gamma.Url = "docs/gamma";
        gamma.AddCategory("Tools");
        gamma.AddTag("devops");
        gamma.Author = "Dan";
        gamma.Status = ContentStatus.Draft;
        gamma.Locale = "en";
        gamma.ContentId = "loc-2";
        gamma.DateCreated = Utc(2024, 3, 20);
        gamma.LastModified = Utc(2024, 1, 5);
        gamma.OrderIndex = 2;
        gamma.SetDirectory(docs);

        var delta = CreateItem("d", "Delta Root", "delta.md", "the root page");
        delta.Slug = "delta";
        delta.Url = "delta";
        delta.Author = "Cara";
        delta.Status = ContentStatus.Archived;
        delta.Locale = "EN";
        delta.ContentId = "loc-3";
        delta.DateCreated = Utc(2023, 12, 1);
        delta.LastModified = null;
        delta.OrderIndex = 4;

        _items = [alpha, beta, gamma, delta];
    }

    [Fact]
    public void ApplyFilters_WithEmptyQuery_ReturnsOnlyPublishedItems()
    {
        // Act
        var result = Apply(new ContentQuery());

        // Assert
        result.ShouldBe("a,b");
    }

    [Fact]
    public void ApplyFilters_WithWhitespaceTextFields_IgnoresThoseFilters()
    {
        // Arrange
        var query = AllStatuses();
        query.Path = " ";
        query.Directory = "";
        query.DirectoryId = " ";
        query.Slug = "";
        query.Url = " ";
        query.Category = " ";
        query.Tag = "";
        query.Author = " ";
        query.SearchQuery = "   ";
        query.Locale = "";
        query.LocalizationId = " ";
        query.ProviderId = "";

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,b,c,d");
    }

    [Fact]
    public void ApplyFilters_WithEmptyCollections_IgnoresThoseFilters()
    {
        // Arrange
        var query = AllStatuses();
        query.Categories = [];
        query.Tags = [];
        query.IncludeIds = [];
        query.ExcludeIds = [];

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,b,c,d");
    }

    // Known defect, pinned as current behavior: ContentQuery.Path is ignored, so a path filter returns every item. Update this expectation when it is fixed.
    [Fact]
    public void ApplyFilters_WithPath_IsIgnoredAndReturnsEverything()
    {
        // Arrange
        var query = AllStatuses();
        query.Path = "docs";

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,b,c,d");
    }

    [Theory]
    [InlineData("BLOG", "a")]
    [InlineData("sub", "b")]
    [InlineData("docs", "c")]
    [InlineData("missing", "")]
    public void ApplyFilters_WithDirectoryOnly_MatchesTheDirectoryNameIgnoringCase(string directory, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Directory = directory;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("blog", "a,b")]
    [InlineData("BLOG/SUB", "b")]
    [InlineData("docs", "c")]
    public void ApplyFilters_WithDirectoryAndSubdirectories_MatchesTheDirectoryPathPrefix(string directory, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Directory = directory;
        query.IncludeSubdirectories = true;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    // Before the fix a plain prefix match let "blog" take in "blog2/..." too.
    [Theory]
    [InlineData("blog", "a,b")]
    [InlineData("blog2", "z")]
    public void ApplyFilters_WithDirectoryAndSubdirectories_MatchesWholeFolderNamesOnly(string directory, string expected)
    {
        _items.Add(CreateItem("z", "Zeta", "blog2/zeta.md", "next to blog"));
        var query = AllStatuses();
        query.Directory = directory;
        query.IncludeSubdirectories = true;

        var result = Apply(query);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("d-sub", "b")]
    [InlineData("d-blog", "a")]
    [InlineData("unknown", "")]
    public void ApplyFilters_WithDirectoryId_ReturnsItemsInThatDirectory(string directoryId, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.DirectoryId = directoryId;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("beta", "b")]
    [InlineData("BETA", "")]
    public void ApplyFilters_WithSlug_MatchesExactly(string slug, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Slug = slug;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("/docs/gamma/", "c")]
    [InlineData("DOCS/GAMMA", "c")]
    [InlineData("docs", "")]
    public void ApplyFilters_WithUrl_TrimsSlashesAndIgnoresCase(string url, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Url = url;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("web-dev", "a,b")]
    [InlineData("dev", "a,b")]
    [InlineData(" TOOLS ", "b,c")]
    [InlineData("design", "")]
    public void ApplyFilters_WithCategory_MatchesPartOfACategoryWithHyphensAsSpaces(string category, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Category = category;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ApplyFilters_WithCategory_IgnoresTheLocationFilters()
    {
        // Arrange
        var query = AllStatuses();
        query.Category = "web-dev";
        query.DirectoryId = "d-docs";
        query.Directory = "docs";
        query.Slug = "gamma";
        query.Url = "docs/gamma";

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,b");
    }

    [Fact]
    public void ApplyFilters_WithTag_IgnoresTheLocationFilters()
    {
        // Arrange
        var query = AllStatuses();
        query.Tag = "csharp";
        query.Slug = "gamma";
        query.DirectoryId = "d-docs";

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,b");
    }

    [Theory]
    [InlineData("web dev| tools ", "b")]
    [InlineData("TOOLS", "b,c")]
    [InlineData("web", "")]
    public void ApplyFilters_WithCategories_RequiresEveryCategoryExactly(string categories, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Categories = categories.Split('|');

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("CSharp", "a,b")]
    [InlineData(" devops ", "c")]
    [InlineData("c", "")]
    public void ApplyFilters_WithTag_MatchesATagExactlyIgnoringCase(string tag, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Tag = tag;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("csharp|BLAZOR", "a")]
    [InlineData(" csharp ", "a,b")]
    [InlineData("csharp|devops", "")]
    public void ApplyFilters_WithTags_RequiresEveryTag(string tags, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Tags = tags.Split('|');

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(true, "a")]
    [InlineData(false, "b,c,d")]
    public void ApplyFilters_WithIsFeatured_MatchesTheFlag(bool isFeatured, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.IsFeatured = isFeatured;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("ANN", "a")]
    [InlineData("an", "")]
    public void ApplyFilters_WithAuthor_MatchesTheWholeNameIgnoringCase(string author, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Author = author;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(ContentStatus.Draft, false, "c")]
    [InlineData(ContentStatus.Archived, false, "d")]
    [InlineData(ContentStatus.Published, true, "a,b")]
    [InlineData(ContentStatus.InReview, true, "")]
    public void ApplyFilters_WithStatus_UsesThatStatusWhateverIncludeUnpublishedSays(
        ContentStatus status, bool includeUnpublished, string expected)
    {
        // Arrange
        var query = new ContentQuery { Status = status, IncludeUnpublished = includeUnpublished };

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("2024-02-15", null, "b,c")]
    [InlineData(null, "2024-01-10", "a,d")]
    [InlineData("2024-01-10", "2024-02-15", "a,b")]
    public void ApplyFilters_WithDateRange_IncludesBothEnds(string? from, string? to, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.DateFrom = from is null ? null : ParseUtc(from);
        query.DateTo = to is null ? null : ParseUtc(to);

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("GAMMA", "c")]
    [InlineData("testing", "b")]
    [InlineData("root", "d")]
    [InlineData("tools", "b,c")]
    [InlineData("devops", "c")]
    [InlineData("alpha  devops", "a,c")]
    [InlineData("nothing", "")]
    public void ApplyFilters_WithSearchQuery_MatchesAnyTermInAnySearchedField(string search, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.SearchQuery = search;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("en", "a,c,d")]
    [InlineData("FR", "b")]
    public void ApplyFilters_WithLocale_MatchesIgnoringCase(string locale, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.Locale = locale;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("LOC-1", "a,b")]
    [InlineData("loc-3", "d")]
    public void ApplyFilters_WithLocalizationId_MatchesTheContentIdIgnoringCase(string localizationId, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.LocalizationId = localizationId;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(Provider, "a,b,c,d")]
    [InlineData("other", "")]
    public void ApplyFilters_WithProviderId_ReturnsEverythingOrNothing(string providerId, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.ProviderId = providerId;

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ApplyFilters_WithProviderIdOnNoItems_ReturnsNothing()
    {
        // Arrange
        var query = AllStatuses();
        query.ProviderId = Provider;

        // Act
        var result = _filter.ApplyFilters(new List<ContentItem>().AsQueryable(), query).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ApplyFilters_WithIncludeIds_KeepsOnlyThoseItems()
    {
        // Arrange
        var query = AllStatuses();
        query.IncludeIds = ["a", "c", "zzz"];

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a,c");
    }

    [Fact]
    public void ApplyFilters_WithExcludeIds_DropsThoseItems()
    {
        // Arrange
        var query = AllStatuses();
        query.ExcludeIds = ["a", "zzz"];

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("b,c,d");
    }

    [Fact]
    public void ApplyFilters_WithSeveralFilters_ReturnsItemsMatchingAll()
    {
        // Arrange
        var query = AllStatuses();
        query.Author = "ann";
        query.IsFeatured = true;
        query.Locale = "EN";
        query.Tags = ["blazor"];
        query.ExcludeIds = ["b"];

        // Act
        var result = Apply(query);

        // Assert
        result.ShouldBe("a");
    }

    [Theory]
    [InlineData(SortField.Title, SortDirection.Ascending, "a,b,d,c")]
    [InlineData(SortField.Title, SortDirection.Descending, "c,d,b,a")]
    [InlineData(SortField.Author, SortDirection.Ascending, "a,b,d,c")]
    [InlineData(SortField.Author, SortDirection.Descending, "c,d,b,a")]
    [InlineData(SortField.LastModified, SortDirection.Ascending, "d,c,b,a")]
    [InlineData(SortField.LastModified, SortDirection.Descending, "a,b,c,d")]
    [InlineData(SortField.Created, SortDirection.Ascending, "d,a,b,c")]
    [InlineData(SortField.Created, SortDirection.Descending, "c,b,a,d")]
    [InlineData(SortField.Order, SortDirection.Ascending, "b,c,a,d")]
    [InlineData(SortField.Order, SortDirection.Descending, "d,a,c,b")]
    [InlineData(SortField.PublishDate, SortDirection.Ascending, "d,a,b,c")]
    [InlineData(SortField.PublishDate, SortDirection.Descending, "c,b,a,d")]
    [InlineData(SortField.Slug, SortDirection.Ascending, "a,b,d,c")]
    [InlineData(SortField.Slug, SortDirection.Descending, "c,d,b,a")]
    [InlineData(SortField.ReadTime, SortDirection.Ascending, "a,d,c,b")]
    [InlineData(SortField.ReadTime, SortDirection.Descending, "b,c,a,d")]
    [InlineData(SortField.Date, SortDirection.Ascending, "d,a,b,c")]
    [InlineData(SortField.Date, SortDirection.Descending, "c,b,a,d")]
    [InlineData((SortField)99, SortDirection.Ascending, "d,a,b,c")]
    [InlineData((SortField)99, SortDirection.Descending, "c,b,a,d")]
    public void ApplyFilters_WithSort_OrdersByTheField(SortField sortBy, SortDirection direction, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.SortBy = sortBy;
        query.SortDirection = direction;

        // Act
        var result = ApplyInOrder(query);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void ApplyFilters_WithDefaultSort_OrdersNewestFirst()
    {
        // Act
        var result = ApplyInOrder(AllStatuses());

        // Assert
        result.ShouldBe("c,b,a,d");
    }

    [Theory]
    [InlineData(1, 2, "c,a")]
    [InlineData(3, null, "d")]
    [InlineData(null, 1, "b")]
    [InlineData(0, 0, "b,c,a,d")]
    [InlineData(-1, -5, "b,c,a,d")]
    [InlineData(10, null, "")]
    public void ApplyFilters_WithSkipAndTake_PagesAfterSorting(int? skip, int? take, string expected)
    {
        // Arrange
        var query = AllStatuses();
        query.SortBy = SortField.Order;
        query.SortDirection = SortDirection.Ascending;
        query.Skip = skip;
        query.Take = take;

        // Act
        var result = ApplyInOrder(query);

        // Assert
        result.ShouldBe(expected);
    }

    private static ContentQuery AllStatuses() => new() { IncludeUnpublished = true };

    private string Apply(ContentQuery query)
    {
        return string.Join(',', _filter.ApplyFilters(_items.AsQueryable(), query).Select(i => i.Id).OrderBy(id => id, StringComparer.Ordinal));
    }

    private string ApplyInOrder(ContentQuery query)
    {
        return string.Join(',', _filter.ApplyFilters(_items.AsQueryable(), query).Select(i => i.Id));
    }

    private static ContentItem CreateItem(string id, string title, string path, string content)
    {
        return ContentItem.Create(id, title, content, path, Provider);
    }

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    private static DateTime ParseUtc(string value)
    {
        var date = DateOnly.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        return Utc(date.Year, date.Month, date.Day);
    }
}
