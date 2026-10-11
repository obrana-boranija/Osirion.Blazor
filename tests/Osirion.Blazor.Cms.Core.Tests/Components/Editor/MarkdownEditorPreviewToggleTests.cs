using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownEditorPreviewToggleTests : BunitContext
{
    private readonly ScriptedJSRuntime _jsRuntime = new();

    public MarkdownEditorPreviewToggleTests()
    {
        var markdownRenderer = Substitute.For<IMarkdownRendererService>();
        markdownRenderer.RenderToHtml(Arg.Any<string>()).Returns(call => $"<p>{call.Arg<string>()}</p>");
        Services.AddSingleton<IJSRuntime>(_jsRuntime);
        Services.AddSingleton(markdownRenderer);
        SetRendererInfo(new RendererInfo("Server", true));
    }

    [Fact]
    public void MarkdownEditorPreview_StaticRender_ShowsEditorAndPreview()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.Content, "Hello")
            .Add(p => p.Class, "extra"));

        cut.Find(".osirion-markdown-editor-preview").ClassList.ShouldBe(["osirion-markdown-editor-preview", "preview-visible", "extra"], ignoreOrder: true);
        cut.Find("textarea").GetAttribute("value").ShouldBe("Hello");
        cut.Find(".osirion-markdown-preview-content p").TextContent.ShouldBe("Hello");
    }

    [Fact]
    public void MarkdownEditorPreview_Toggle_NotifiesTheNewVisibility()
    {
        bool? visible = null;
        var cut = Render<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.ShowPreviewChanged, value => visible = value));

        cut.Find(".osirion-markdown-action-button").Click();

        visible.ShouldBe(false);
        cut.Find(".osirion-markdown-action-button").GetAttribute("title").ShouldBe("Show Preview");
        cut.FindAll(".osirion-markdown-preview-section").ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditorPreview_EditorScrollsWhilePreviewHidden_DoesNotScrollThePreview()
    {
        var cut = Render<MarkdownEditorPreview>(parameters => parameters
            .Add(p => p.Content, "Text")
            .Add(p => p.ShowPreview, false));

        cut.Find("textarea").Scroll();

        _jsRuntime.Scripts.ShouldNotContain(script => script.Contains("targetScrollTop"));
    }

    [Fact]
    public void MarkdownEditorPreview_WithoutActionsBar_HidesTheToggle()
    {
        var cut = Render<MarkdownEditorPreview>(parameters => parameters.Add(p => p.ShowActionsBar, false));

        cut.FindAll(".osirion-markdown-editor-preview-actions").ShouldBeEmpty();
    }
}
