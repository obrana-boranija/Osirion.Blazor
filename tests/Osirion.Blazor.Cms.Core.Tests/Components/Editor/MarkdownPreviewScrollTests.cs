using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownPreviewScrollTests : BunitContext
{
    private readonly ScriptedJSRuntime _jsRuntime = new();
    private readonly IMarkdownRendererService _markdownRenderer = Substitute.For<IMarkdownRendererService>();

    public MarkdownPreviewScrollTests()
    {
        Services.AddSingleton<IJSRuntime>(_jsRuntime);
        Services.AddSingleton(_markdownRenderer);
        SetRendererInfo(new RendererInfo("Server", true));
        _markdownRenderer.RenderToHtml(Arg.Any<string>()).Returns(call => $"<p>{call.Arg<string>()}</p>");
    }

    [Fact]
    public void MarkdownPreview_StaticRender_ShowsTheRenderedMarkdown()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<MarkdownPreview>(parameters => parameters.Add(p => p.Markdown, "Hello"));

        cut.Find(".osirion-markdown-preview-title").TextContent.ShouldBe("Preview");
        cut.Find(".osirion-markdown-preview-content p").TextContent.ShouldBe("Hello");
    }

    [Fact]
    public void MarkdownPreview_SameParametersRenderedAgain_DoesNotRenderTheMarkdownAgain()
    {
        var cut = Render<MarkdownPreview>(parameters => parameters.Add(p => p.Markdown, "Same"));

        cut.Render(parameters => parameters.Add(p => p.Markdown, "Same"));

        _markdownRenderer.Received(1).RenderToHtml("Same");
        cut.Instance.RenderedHtml.ShouldBe("<p>Same</p>");
    }

    [Fact]
    public void MarkdownPreview_Scroll_ReportsThePosition()
    {
        double? reported = null;
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, "Text")
            .Add(p => p.OnScroll, position => reported = position));

        cut.Find(".osirion-markdown-preview-content").Scroll();

        reported.ShouldBe(0.25);
    }

    [Fact]
    public void MarkdownPreview_ScrollWithoutJavaScript_ReportsTheTop()
    {
        _jsRuntime.NumberResult = 0.9;
        _jsRuntime.Fail = true;
        double? reported = null;
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.Markdown, "Text")
            .Add(p => p.OnScroll, position => reported = position));

        cut.Find(".osirion-markdown-preview-content").Scroll();

        reported.ShouldBe(0);
    }

    [Fact]
    public void MarkdownPreview_ScrollWithSyncDisabled_DoesNotReport()
    {
        double? reported = null;
        var cut = Render<MarkdownPreview>(parameters => parameters
            .Add(p => p.SyncScroll, false)
            .Add(p => p.OnScroll, position => reported = position));

        cut.Find(".osirion-markdown-preview-content").Scroll();

        reported.ShouldBeNull();
        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownPreview_ScrollWithoutListener_SendsNoScript()
    {
        var cut = Render<MarkdownPreview>();

        cut.Find(".osirion-markdown-preview-content").Scroll();

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownPreview_SetScrollPositionWithSyncDisabled_SendsNoScript()
    {
        var cut = Render<MarkdownPreview>(parameters => parameters.Add(p => p.SyncScroll, false));

        await cut.InvokeAsync(() => cut.Instance.SetScrollPositionAsync(0.5));

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownPreview_SetScrollPositionWithoutJavaScript_StillReportsLaterScrolls()
    {
        double? reported = null;
        var cut = Render<MarkdownPreview>(parameters => parameters.Add(p => p.OnScroll, position => reported = position));
        _jsRuntime.Fail = true;

        await cut.InvokeAsync(() => cut.Instance.SetScrollPositionAsync(0.5));
        _jsRuntime.Fail = false;
        cut.Find(".osirion-markdown-preview-content").Scroll();

        _jsRuntime.Scripts[0].ShouldContain("const targetScrollTop = 0.5 *");
        reported.ShouldBe(0.25);
    }
}
