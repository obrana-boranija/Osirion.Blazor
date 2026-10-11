using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Components.Editor;

public class MarkdownEditorToolbarTests : BunitContext
{
    private readonly ScriptedJSRuntime _jsRuntime = new();

    public MarkdownEditorToolbarTests()
    {
        Services.AddSingleton<IJSRuntime>(_jsRuntime);
        SetRendererInfo(new RendererInfo("Server", true));
    }

    // The scripts find the textarea through the attribute Blazor puts on a referenced element.
    private static string TextareaSelector(IRenderedComponent<MarkdownEditor> cut) =>
        $"document.querySelector('[_bl_{cut.Find("textarea").GetAttribute("blazor:elementReference")}]')";

    private IRenderedComponent<MarkdownEditor> RenderEditor(
        Action<ComponentParameterCollectionBuilder<MarkdownEditor>>? configure = null) =>
        Render<MarkdownEditor>(parameters =>
        {
            parameters.Add(p => p.Content, "start");
            configure?.Invoke(parameters);
        });

    [Fact]
    public void MarkdownEditor_StaticRender_ShowsToolbarHeaderAndTextarea()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<MarkdownEditor>(parameters => parameters.Add(p => p.Content, "# Hi"));

        cut.FindAll(".osirion-markdown-toolbar-button").Count.ShouldBe(6);
        cut.Find(".osirion-markdown-editor-title").TextContent.ShouldBe("Editor");
        cut.Find("textarea").GetAttribute("placeholder").ShouldBe("Enter markdown here...");
        cut.Find("textarea").GetAttribute("value").ShouldBe("# Hi");
    }

    [Fact]
    public void MarkdownEditor_ActionWithoutIcon_ShowsTheLabel()
    {
        var cut = RenderEditor(parameters => parameters.Add(p => p.ToolbarActions,
            [new ToolbarAction("Quote", ToolbarActionType.Insert, "> ", "Insert quote")]));

        var button = cut.Find(".osirion-markdown-toolbar-button");

        button.GetAttribute("title").ShouldBe("Insert quote");
        button.QuerySelector("span")!.TextContent.ShouldBe("Quote");
        button.QuerySelector(".osirion-markdown-toolbar-icon").ShouldBeNull();
    }

    [Fact]
    public void MarkdownEditor_InsertAction_SendsTheTextToTheEditor()
    {
        var cut = RenderEditor(parameters => parameters.Add(p => p.ToolbarActions,
            [new ToolbarAction("Quote", ToolbarActionType.Insert, "> \"q\"", "Insert quote")]));

        cut.Find(".osirion-markdown-toolbar-button").Click();

        _jsRuntime.Scripts.ShouldHaveSingleItem().ShouldContain("before + \"\\u003E \\u0022q\\u0022\" + after");
    }

    [Fact]
    public void MarkdownEditor_WrapAction_SendsPrefixSuffixAndDefaultText()
    {
        var cut = RenderEditor(parameters => parameters.Add(p => p.ToolbarActions,
            [new ToolbarAction("B", ToolbarActionType.Wrap, "**|**|bold text", "Bold")]));

        cut.Find(".osirion-markdown-toolbar-button").Click();

        var script = _jsRuntime.Scripts.ShouldHaveSingleItem();
        script.ShouldContain("before + \"**\" + textToWrap + \"**\" + after");
        script.ShouldContain(": \"bold text\";");
    }

    [Theory]
    [InlineData(ToolbarActionType.Wrap, "**|**")]
    [InlineData(ToolbarActionType.Wrap, null)]
    [InlineData(ToolbarActionType.Insert, null)]
    [InlineData(ToolbarActionType.Custom, "custom")]
    public void MarkdownEditor_ActionWithoutUsableData_DoesNothing(ToolbarActionType type, string? action)
    {
        string? changed = null;
        var cut = RenderEditor(parameters => parameters
            .Add(p => p.ToolbarActions, [new ToolbarAction("X", type, action, "X")])
            .Add(p => p.ContentChanged, value => changed = value));

        cut.Find(".osirion-markdown-toolbar-button").Click();

        _jsRuntime.Scripts.ShouldBeEmpty();
        changed.ShouldBeNull();
    }

    [Fact]
    public void MarkdownEditor_InsertActionWithoutJavaScript_AppendsTheText()
    {
        _jsRuntime.Fail = true;
        string? changed = null;
        var cut = RenderEditor(parameters => parameters
            .Add(p => p.ToolbarActions, [new ToolbarAction("L", ToolbarActionType.Insert, "\n- item", "List")])
            .Add(p => p.ContentChanged, value => changed = value));

        cut.Find(".osirion-markdown-toolbar-button").Click();

        changed.ShouldBe("start\n- item");
    }

    [Fact]
    public void MarkdownEditor_WrapActionWithoutJavaScript_AppendsTheWrappedDefaultText()
    {
        _jsRuntime.Fail = true;
        string? changed = null;
        var cut = RenderEditor(parameters => parameters
            .Add(p => p.ToolbarActions, [new ToolbarAction("B", ToolbarActionType.Wrap, "**|**|bold text", "Bold")])
            .Add(p => p.ContentChanged, value => changed = value));

        cut.Find(".osirion-markdown-toolbar-button").Click();

        changed.ShouldBe("start**bold text**");
    }

    [Fact]
    public void MarkdownEditor_TabWithoutJavaScript_AppendsATab()
    {
        _jsRuntime.Fail = true;
        string? changed = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.ContentChanged, value => changed = value));

        cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Tab" });

        changed.ShouldBe("start\t");
    }

    [Fact]
    public void MarkdownEditor_ShiftTab_OutdentsTheTextareaInTheBrowser()
    {
        string? changed = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.ContentChanged, value => changed = value));
        var textarea = TextareaSelector(cut);

        cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Tab", ShiftKey = true });

        var script = _jsRuntime.Scripts.ShouldHaveSingleItem();
        script.ShouldContain($"const textarea = {textarea};");
        script.ShouldContain("if (true) {");
        script.ShouldContain("} else if (!true) {");
        changed.ShouldBeNull();
    }

    // Known defect, pinned as current behavior: without JavaScript Shift+Tab appends a tab, the same as Tab, instead of outdenting. Update this expectation when it is fixed.
    [Fact]
    public void MarkdownEditor_ShiftTabWithoutJavaScript_AppendsATab()
    {
        _jsRuntime.Fail = true;
        string? changed = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.ContentChanged, value => changed = value));

        cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "Tab", ShiftKey = true });

        changed.ShouldBe("start	");
    }

    [Fact]
    public void MarkdownEditor_OtherKey_SendsNoScript()
    {
        var cut = RenderEditor();

        cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = "a" });

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditor_AutoFocus_FocusesTheTextareaAfterFirstRender()
    {
        var cut = RenderEditor(parameters => parameters.Add(p => p.AutoFocus, true));

        _jsRuntime.Scripts.ShouldHaveSingleItem().ShouldBe($"{TextareaSelector(cut)}?.focus()");
    }

    [Fact]
    public void MarkdownEditor_WithoutAutoFocus_SendsNoScript()
    {
        RenderEditor();

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownEditor_FocusWithoutJavaScript_DoesNotThrow()
    {
        var cut = RenderEditor();
        _jsRuntime.Fail = true;

        var exception = await Record.ExceptionAsync(() => cut.InvokeAsync(() => cut.Instance.FocusAsync()));

        exception.ShouldBeNull();
    }

    [Fact]
    public void MarkdownEditor_ContentParameterChanges_ShowsTheNewContent()
    {
        var cut = RenderEditor();

        cut.Render(parameters => parameters.Add(p => p.Content, "changed"));

        cut.Find("textarea").GetAttribute("value").ShouldBe("changed");
    }

    [Fact]
    public void MarkdownEditor_TypingTheCurrentContent_DoesNotNotify()
    {
        var notifications = 0;
        var cut = RenderEditor(parameters => parameters.Add(p => p.ContentChanged, _ => notifications++));

        cut.Find("textarea").Input("start");

        notifications.ShouldBe(0);
    }

    [Fact]
    public void MarkdownEditor_Scroll_ReportsThePosition()
    {
        double? reported = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.OnScroll, position => reported = position));

        cut.Find("textarea").Scroll();

        reported.ShouldBe(0.25);
    }

    [Fact]
    public void MarkdownEditor_ScrollWithoutJavaScript_ReportsTheTop()
    {
        _jsRuntime.NumberResult = 0.9;
        _jsRuntime.Fail = true;
        double? reported = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.OnScroll, position => reported = position));

        cut.Find("textarea").Scroll();

        reported.ShouldBe(0);
    }

    [Fact]
    public void MarkdownEditor_ScrollWithSyncDisabled_DoesNotReport()
    {
        double? reported = null;
        var cut = RenderEditor(parameters => parameters
            .Add(p => p.SyncScroll, false)
            .Add(p => p.OnScroll, position => reported = position));

        cut.Find("textarea").Scroll();

        reported.ShouldBeNull();
        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditor_ScrollWithoutListener_SendsNoScript()
    {
        var cut = RenderEditor();

        cut.Find("textarea").Scroll();

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownEditor_SetScrollPositionWithSyncDisabled_SendsNoScript()
    {
        var cut = RenderEditor(parameters => parameters.Add(p => p.SyncScroll, false));

        await cut.InvokeAsync(() => cut.Instance.SetScrollPositionAsync(0.5));

        _jsRuntime.Scripts.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownEditor_SetScrollPositionWithoutJavaScript_StillReportsLaterScrolls()
    {
        double? reported = null;
        var cut = RenderEditor(parameters => parameters.Add(p => p.OnScroll, position => reported = position));
        _jsRuntime.Fail = true;

        await cut.InvokeAsync(() => cut.Instance.SetScrollPositionAsync(0.5));
        _jsRuntime.Fail = false;
        cut.Find("textarea").Scroll();

        reported.ShouldBe(0.25);
    }
}
