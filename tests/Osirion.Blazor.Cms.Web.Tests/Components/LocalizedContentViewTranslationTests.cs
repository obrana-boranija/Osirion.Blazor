using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class LocalizedContentViewTranslationTests : CmsWebComponentTestBase
{
    private const string LocalizationId = "post-translations";

    private readonly ContentItem _english = CreateTranslation("post-en", "en", "en/blog/post");

    public LocalizedContentViewTranslationTests()
    {
        var provider = Substitute.For<IContentProvider>();
        provider.GetContentTranslationsAsync(LocalizationId).Returns(new Dictionary<string, ContentItem>
        {
            ["en"] = _english,
            ["sr"] = CreateTranslation("post-sr", "sr", "/sr/blog/post")
        });
        Services.GetRequiredService<IContentProviderManager>().GetDefaultProvider().Returns(provider);
    }

    [Fact]
    public void LocalizedContentView_ItemWithTranslations_LinksToOtherLocalesOnTheSameSite()
    {
        var cut = RenderStatic<LocalizedContentView>(parameters => parameters.Add(p => p.Item, _english));

        var link = cut.Find(".osirion-translations a");
        link.GetAttribute("href").ShouldBe("/sr/blog/post");
        link.TextContent.Trim().ShouldBe("SR");
    }

    [Fact]
    public void LocalizedContentView_TranslationUrlFormatter_ReceivesTheLocale()
    {
        var cut = RenderStatic<LocalizedContentView>(parameters => parameters
            .Add(p => p.Item, _english)
            .Add(p => p.TranslationUrlFormatter, (id, locale) => $"/{locale}/translated/{id}"));

        cut.Find(".osirion-translations a").GetAttribute("href").ShouldBe($"/sr/translated/{LocalizationId}");
    }

    private static ContentItem CreateTranslation(string id, string locale, string url)
    {
        var item = ContentItem.Create(id, "Post", "<p>Post</p>", $"{locale}/blog/post.md", "test");
        item.SetLocale(locale);
        item.SetContentId(LocalizationId);
        item.SetUrl(url);
        return item;
    }
}
