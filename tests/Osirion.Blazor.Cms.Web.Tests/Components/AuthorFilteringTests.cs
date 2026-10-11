using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class AuthorFilteringTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();
    private ContentQuery? _lastQuery;

    public AuthorFilteringTests()
    {
        _provider.GetItemsByQueryAsync(Arg.Do<ContentQuery>(q => _lastQuery = q))
            .Returns(Task.FromResult<IReadOnlyList<ContentItem>?>(new List<ContentItem>()));
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
    }

    [Fact]
    public void ContentList_Author_FiltersTheQueryByAuthor()
    {
        RenderStatic<ContentList>(p => p
            .Add(c => c.Author, "Jane Developer")
            .Add(c => c.EnableSeoMetadata, false));

        _lastQuery.ShouldNotBeNull();
        _lastQuery.Author.ShouldBe("Jane Developer");
    }

    [Fact]
    public void ContentView_AuthorUrlFormatter_LinksTheAuthorsName()
    {
        var item = CreateItem("<p>Post</p>");
        item.SetAuthor("Jane Developer");

        var cut = RenderStatic<ContentView>(p => p
            .Add(c => c.Item, item)
            .Add(c => c.AuthorUrlFormatter, name => $"/authors/{name.ToLowerInvariant().Replace(' ', '-')}"));

        var link = cut.Find(".osirion-content-author a");
        link.GetAttribute("href").ShouldBe("/authors/jane-developer");
        link.GetAttribute("rel").ShouldBe("author");
        link.TextContent.ShouldBe("Jane Developer");
    }

    [Fact]
    public void ContentView_NoAuthorUrlFormatter_ShowsTheNameAsText()
    {
        var item = CreateItem("<p>Post</p>");
        item.SetAuthor("Jane Developer");

        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, item));

        cut.FindAll(".osirion-content-author a").ShouldBeEmpty();
        cut.Find(".osirion-content-author").TextContent.ShouldContain("Jane Developer");
    }
}
