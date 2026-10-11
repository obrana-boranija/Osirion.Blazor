using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// Pins how the renderer uses the answered question: the answers-to meta tag and the generated FAQ page. The
/// question words and title patterns of each language are pinned in AnswerQuestionsTests.
/// </summary>
public class SeoMetadataRendererAnswerTests : CmsWebComponentTestBase
{
    [Fact]
    public void SeoMetadataRenderer_TitlePattern_AnswersTheDerivedQuestion()
    {
        var cut = RenderWithGeneratedFaq("EN-US", "Introduction to Blazor");

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe("What is Blazor?");
        cut.Markup.ShouldContain("\"name\": \"What is Blazor?\"");
    }

    [Theory]
    [InlineData("en", "How does it work")]
    [InlineData("ja", "How to")]
    public void SeoMetadataRenderer_QuestionTitle_AnswersTheTitleItself(string locale, string title)
    {
        var cut = RenderWithGeneratedFaq(locale, title);

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe(title);
    }

    [Theory]
    [InlineData("en", "Plain title")]
    [InlineData("ja", "Guide to x")]
    public void SeoMetadataRenderer_TitleWithoutQuestion_RendersNoAnswerOrFaqPage(string locale, string title)
    {
        var cut = RenderWithGeneratedFaq(locale, title);

        cut.FindAll("meta[name='answers-to']").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("FAQPage");
    }

    // Known defect, pinned as current behavior: the question derived from "Guide to testing Blazor" is the ungrammatical "How to testing Blazor?". Update this expectation when it is fixed.
    [Fact]
    public void SeoMetadataRenderer_QuestionWithoutDescription_AnswersButRendersNoFaqPage()
    {
        var item = SeoTestContent.Article(title: "Guide to testing Blazor");
        item.Description = string.Empty;
        Services.GetRequiredService<NavigationManager>().NavigateTo(SeoTestContent.PageUrl);

        var cut = RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.Content, item)
            .Add(c => c.EnableGeneratedFaq, true));

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe("How to testing Blazor?");
        cut.Markup.ShouldNotContain("FAQPage");
    }

    [Fact]
    public void SeoMetadataRenderer_CollectionTitleQuestion_AnswersWithTheCollectionDescription()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(SeoTestContent.PageUrl);

        var cut = RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.ContentItems, [SeoTestContent.BarePost()])
            .Add(c => c.CollectionTitle, "How we test")
            .Add(c => c.CollectionDescription, "With bUnit.")
            .Add(c => c.EnableGeneratedFaq, true));

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe("How we test");
        cut.Markup.ShouldContain("\"text\": \"With bUnit.\"");
    }

    private IRenderedComponent<SeoMetadataRenderer> RenderWithGeneratedFaq(string locale, string title)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(SeoTestContent.PageUrl);

        return RenderStatic<SeoMetadataRenderer>(p => p
            .Add(c => c.Content, SeoTestContent.Article(title: title, locale: locale))
            .Add(c => c.EnableGeneratedFaq, true));
    }
}
