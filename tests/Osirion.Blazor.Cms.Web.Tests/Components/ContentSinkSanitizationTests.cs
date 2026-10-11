using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

// Content from a custom provider, or an item passed in by the page, reaches these components without going
// through the built-in ingestion sanitizer. Each test fails before the fix, when Item.Content was written
// to the page as a MarkupString unchanged.
public class ContentSinkSanitizationTests : CmsWebComponentTestBase
{
    [Fact]
    public void ContentView_ItemContentWithScriptAndHandlers_RendersSanitizedBody()
    {
        var cut = RenderStatic<ContentView>(parameters => parameters
            .Add(p => p.Item, CreateItem(HostileContent)));

        cut.Find(".osirion-content-body").InnerHtml.MarkupMatches(SanitizedContent);
    }

    [Fact]
    public void LocalizedContentView_ItemContentWithScriptAndHandlers_RendersSanitizedBody()
    {
        var cut = RenderStatic<LocalizedContentView>(parameters => parameters
            .Add(p => p.Item, CreateItem(HostileContent)));

        cut.Find(".osirion-content-body").InnerHtml.MarkupMatches(SanitizedContent);
    }

    [Fact]
    public void DocumentPage_ContentWithScriptAndHandlers_RendersSanitizedBody()
    {
        var cut = RenderStatic<DocumentPage>(parameters => parameters
            .Add(p => p.Content, CreateItem(HostileContent)));

        cut.Find(".osirion-document-body").InnerHtml.MarkupMatches(SanitizedContent);
    }

    [Fact]
    public void ContentRenderer_ItemContentWithScriptAndHandlers_RendersSanitizedBody()
    {
        var cut = RenderStatic<ContentRenderer>(parameters => parameters
            .Add(p => p.Item, CreateItem(HostileContent)));

        cut.Find("article").InnerHtml.MarkupMatches(SanitizedContent);
    }

    // A component renders again on every state change; before the fix each render sanitized the whole document again.
    [Fact]
    public void ContentView_RenderedAgainWithSameItem_SanitizesContentOnce()
    {
        var sanitizer = RegisterCountingSanitizer();
        var cut = RenderStatic<ContentView>(parameters => parameters.Add(p => p.Item, CreateItem("<p>Post</p>")));

        cut.Render();

        sanitizer.Received(1).Sanitize("<p>Post</p>");
    }

    [Fact]
    public void LocalizedContentView_RenderedAgainWithSameItem_SanitizesContentOnce()
    {
        var sanitizer = RegisterCountingSanitizer();
        var cut = RenderStatic<LocalizedContentView>(parameters => parameters.Add(p => p.Item, CreateItem("<p>Post</p>")));

        cut.Render();

        sanitizer.Received(1).Sanitize("<p>Post</p>");
    }

    [Fact]
    public void DocumentPage_RenderedAgainWithSameContent_SanitizesContentOnce()
    {
        var sanitizer = RegisterCountingSanitizer();
        var cut = RenderStatic<DocumentPage>(parameters => parameters.Add(p => p.Content, CreateItem("<p>Post</p>")));

        cut.Render();

        sanitizer.Received(1).Sanitize("<p>Post</p>");
    }

    [Fact]
    public void ContentView_ItemContentChanges_RendersNewContentSanitized()
    {
        var cut = RenderStatic<ContentView>(parameters => parameters.Add(p => p.Item, CreateItem("<p>First</p>")));

        cut.Render(parameters => parameters.Add(p => p.Item, CreateItem(HostileContent)));

        cut.Find(".osirion-content-body").InnerHtml.MarkupMatches(SanitizedContent);
    }

    [Fact]
    public void ContentView_HostAllowList_KeepsCustomElement()
    {
        Services.AddOsirionHtmlSanitizer(options => options.AllowElement("chart", "type", "data"));

        var cut = RenderStatic<ContentView>(parameters => parameters
            .Add(p => p.Item, CreateItem("<chart type=\"line\" data=\"[[1, 2]]\" onclick=\"alert(1)\"></chart>")));

        cut.Find(".osirion-content-body").InnerHtml.MarkupMatches("<chart type=\"line\" data=\"[[1, 2]]\"></chart>");
    }

    private IHtmlContentSanitizer RegisterCountingSanitizer()
    {
        var sanitizer = Substitute.For<IHtmlContentSanitizer>();
        sanitizer.Sanitize(Arg.Any<string?>()).Returns(call => call.Arg<string?>() ?? string.Empty);
        Services.AddSingleton(sanitizer);
        return sanitizer;
    }
}
