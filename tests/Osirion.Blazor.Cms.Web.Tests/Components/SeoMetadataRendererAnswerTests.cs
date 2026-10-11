using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

/// <summary>
/// Pins the question the generated FAQ answers for each supported language: a title that is a question, or a
/// question derived from a title pattern.
/// </summary>
public class SeoMetadataRendererAnswerTests : CmsWebComponentTestBase
{
    [Theory]
    [InlineData("en", "Guide to testing Blazor", "How to testing Blazor?")]
    [InlineData("en", "Blazor tutorial basics", "How to Blazor  basics?")]
    [InlineData("en", "An introduction to Blazor", "What is An  Blazor?")]
    [InlineData("EN-US", "Guide to x", "How to x?")]
    [InlineData("sr", "Vodič za testiranje", "Kako testiranje?")]
    [InlineData("sr-latn", "Osnovni tutorijal", "Kako Osnovni?")]
    [InlineData("sr-latn-rs", "Uvod u Blazor", "Šta je Blazor?")]
    [InlineData("sr-cyrl", "Водич за тестирање", "Како тестирање?")]
    [InlineData("sr-cyrl-rs", "Основни туторијал", "Како Основни?")]
    [InlineData("sr-cyrl", "Увод у Блазор", "Шта је Блазор?")]
    [InlineData("de", "Anleitung zu Blazor", "Wie Blazor?")]
    [InlineData("de-de", "Blazor Tutorial", "Wie Blazor?")]
    [InlineData("de-at", "Einführung in Blazor", "Was ist Blazor?")]
    [InlineData("fr", "Guide de Blazor", "Comment Blazor?")]
    [InlineData("fr-fr", "Tutoriel Blazor", "Comment Blazor?")]
    [InlineData("fr-ca", "Introduction à Blazor", "Qu'est-ce que Blazor?")]
    [InlineData("es", "Guía de Blazor", "Cómo Blazor?")]
    [InlineData("es-es", "Tutorial de Blazor", "Cómo de Blazor?")]
    [InlineData("es-mx", "Introducción a Blazor", "Qué es Blazor?")]
    public void SeoMetadataRenderer_TitlePattern_AnswersTheDerivedQuestion(string locale, string title, string expectedQuestion)
    {
        var cut = RenderWithGeneratedFaq(locale, title);

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe(expectedQuestion);
    }

    [Theory]
    [InlineData("en", "How does it work")]
    [InlineData("en-gb", "What is it")]
    [InlineData("sr", "Kako radi")]
    [InlineData("sr-cyrl", "Како ради")]
    [InlineData("de-ch", "Wie geht das")]
    [InlineData("fr", "Comment faire")]
    [InlineData("es", "Cómo hacer")]
    [InlineData("it", "Come fare")]
    [InlineData("pt-br", "Como fazer")]
    [InlineData("pt-pt", "o que")]
    [InlineData("nl", "Hoe werkt")]
    [InlineData("ru", "Как это")]
    [InlineData("ja", "How to")]
    public void SeoMetadataRenderer_QuestionTitle_AnswersTheTitleItself(string locale, string title)
    {
        var cut = RenderWithGeneratedFaq(locale, title);

        cut.FindAll("meta[name='answers-to']").ShouldHaveSingleItem().GetAttribute("content").ShouldBe(title);
    }

    [Theory]
    [InlineData("en", "Plain title")]
    [InlineData("sr", "Obican naslov")]
    [InlineData("sr-cyrl", "Обичан")]
    [InlineData("de", "Normal")]
    [InlineData("fr", "Normal")]
    [InlineData("es", "Normal")]
    [InlineData("it-it", "Guida")]
    [InlineData("nl-be", "Gids")]
    [InlineData("ru-ru", "Гид")]
    [InlineData("ja", "Guide to x")]
    [InlineData("", "Guide to x")]
    public void SeoMetadataRenderer_TitleWithoutQuestion_RendersNoAnswerOrFaqPage(string locale, string title)
    {
        var cut = RenderWithGeneratedFaq(locale, title);

        cut.FindAll("meta[name='answers-to']").ShouldBeEmpty();
        cut.Markup.ShouldNotContain("FAQPage");
    }

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
