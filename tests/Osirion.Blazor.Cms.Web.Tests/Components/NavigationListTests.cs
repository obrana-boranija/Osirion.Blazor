using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class NavigationListTests : CmsWebComponentTestBase
{
    private readonly IContentProvider _provider = Substitute.For<IContentProvider>();

    public NavigationListTests()
    {
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(_provider);
        _provider.GetCategoriesAsync().Returns(Task.FromResult<IReadOnlyList<ContentCategory>>([]));
        _provider.GetTagsAsync().Returns(Task.FromResult<IReadOnlyList<ContentTag>>([]));
    }

    // Fails before the fix: OnInitializedAsync and OnParametersSetAsync both loaded on the first render.
    [Fact]
    public async Task CategoriesList_FirstRender_FetchesCategoriesOnce()
    {
        RenderStatic<CategoriesList>(_ => { });

        await _provider.Received(1).GetCategoriesAsync();
    }

    [Fact]
    public async Task TagCloud_FirstRender_FetchesTagsOnce()
    {
        RenderStatic<TagCloud>(_ => { });

        await _provider.Received(1).GetTagsAsync();
    }

    // Fails before the fix: the inner element had class="@GetTagCloudClass", a method group, so it had no usable class.
    [Fact]
    public void TagCloud_WithTags_WrapsTheLinksInAContainerWithAClass()
    {
        _provider.GetTagsAsync().Returns(Task.FromResult<IReadOnlyList<ContentTag>>([ContentTag.Create("C#", "c%23", 3)]));

        var cut = RenderStatic<TagCloud>(_ => { });

        cut.Find("a.osirion-tag-link").ParentElement!.ClassList.ShouldContain("osirion-tags-container");
        cut.Markup.ShouldNotContain("Func");
    }

    // The failure is logged and the empty state shown, never the exception message.
    [Fact]
    public void TagCloud_ProviderThrows_RendersTheEmptyState()
    {
        _provider.GetTagsAsync().Returns<Task<IReadOnlyList<ContentTag>>>(_ => throw new InvalidOperationException("down"));

        var cut = RenderStatic<TagCloud>(_ => { });

        cut.Markup.ShouldNotContain("down");
        cut.FindAll(".osirion-no-tags").ShouldHaveSingleItem();
    }
}
