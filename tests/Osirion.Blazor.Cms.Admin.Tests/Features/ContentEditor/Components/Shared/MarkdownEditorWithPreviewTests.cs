using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class MarkdownEditorWithPreviewTests : AdminComponentTestContext
{
    private const string ModulePath = "./_content/Osirion.Blazor.Cms.Admin/js/markdownEditor.js";

    private readonly IMarkdownProcessor _processor = Substitute.For<IMarkdownProcessor>();
    private readonly List<string> _changes = [];
    private BunitJSModuleInterop? _module;

    public MarkdownEditorWithPreviewTests()
    {
        _processor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>())
            .Returns(call => $"<p>{call.Arg<string>()}</p>");
        Services.AddSingleton(_processor);
        RenderInteractively();
    }

    private IRenderedComponent<MarkdownEditorWithPreview> RenderEditor(
        string content,
        Action<ComponentParameterCollectionBuilder<MarkdownEditorWithPreview>>? configure = null) =>
        RenderWithModulePlanned(parameters =>
        {
            parameters
                .Add(p => p.Content, content)
                .Add(p => p.ContentChanged, (string value) => _changes.Add(value));
            configure?.Invoke(parameters);
        });

    private BunitJSModuleInterop Module => _module ??= PlanModule();

    private BunitJSModuleInterop PlanModule()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.Mode = JSRuntimeMode.Loose;
        return module;
    }

    // Without a planned module a strict runtime throws on import, as a browser does when the script is missing.
    private IRenderedComponent<MarkdownEditorWithPreview> RenderWithModulePlanned(
        Action<ComponentParameterCollectionBuilder<MarkdownEditorWithPreview>> configure)
    {
        if (JSInterop.Mode == JSRuntimeMode.Loose)
        {
            _module ??= PlanModule();
        }

        return Render(configure);
    }

    private void MakeModuleImportFail() => JSInterop.Mode = JSRuntimeMode.Strict;

    private void ShouldOnlyHaveTriedTheImport() =>
        JSInterop.Invocations.Select(invocation => invocation.Identifier).ShouldBe(["import"]);

    private static string StatusBar(IRenderedComponent<MarkdownEditorWithPreview> cut) =>
        cut.Find(".editor-status-bar").TextContent.Trim();

    private static void ClickToolbar(IRenderedComponent<MarkdownEditorWithPreview> cut, string title) =>
        cut.Find($".editor-toolbar button[title='{title}']").Click();

    private void SetupScrollInfo(double percentage) =>
        Module.Setup<MarkdownEditorWithPreview.ScrollInfo>("getScrollInfo", _ => true)
            .SetResult(new MarkdownEditorWithPreview.ScrollInfo { Percentage = percentage });

    [Fact]
    public void MarkdownEditorWithPreview_StaticRender_ShowsEditorToolbarAndPreview()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = RenderEditor("Hello");

        cut.Find(".markdown-editor").ClassList.ShouldContain("split");
        cut.FindAll(".editor-toolbar button").Count.ShouldBe(16);
        cut.Find("textarea.editor-textarea").GetAttribute("value").ShouldBe("Hello");
        cut.Find(".preview-container").InnerHtml.ShouldContain("<p>Hello</p>");
        StatusBar(cut).ShouldBe("Line: 1 Col: 1");
    }

    [Fact]
    public void MarkdownEditorWithPreview_EmptyContent_ShowsPreviewHint()
    {
        _processor.RenderToHtmlAsync(string.Empty, Arg.Any<bool>()).Returns(string.Empty);

        var cut = RenderEditor(string.Empty);

        cut.Find(".preview-container").TextContent.ShouldContain("Preview will appear here as you type.");
    }

    [Fact]
    public void MarkdownEditorWithPreview_WithoutToolbar_HidesToolbar()
    {
        var cut = RenderEditor("x", p => p.Add(c => c.ShowToolbar, false));

        cut.FindAll(".editor-toolbar").ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditorWithPreview_ModeButtons_SwitchPanes()
    {
        var cut = RenderEditor("x");
        var modeButtons = () => cut.FindAll(".editor-tabs button");

        modeButtons()[0].Click();
        cut.FindAll(".preview-pane").ShouldBeEmpty();
        cut.Find(".editor-pane").ClassList.ShouldContain("w-100");
        modeButtons()[0].ClassList.ShouldContain("btn-primary");

        modeButtons()[1].Click();
        cut.FindAll(".editor-pane").ShouldBeEmpty();
        cut.Find(".preview-pane").ClassList.ShouldContain("w-100");

        modeButtons()[2].Click();
        cut.Find(".editor-pane").ClassList.ShouldContain("w-50");
        cut.Find(".preview-pane").ClassList.ShouldContain("border-start");
    }

    [Fact]
    public void MarkdownEditorWithPreview_Typing_RaisesChangeAndRendersPreview()
    {
        var cut = RenderEditor("old");

        cut.Find("textarea.editor-textarea").Input("new text");

        _changes.ShouldBe(["new text"]);
        cut.Find(".preview-container").InnerHtml.ShouldContain("<p>new text</p>");
    }

    [Fact]
    public void MarkdownEditorWithPreview_TypingSameText_DoesNotRaiseChange()
    {
        var cut = RenderEditor("same");

        cut.Find("textarea.editor-textarea").Input("same");

        _changes.ShouldBeEmpty();
    }

    [Fact]
    public async Task MarkdownEditorWithPreview_PreviewRenderingFails_ReportsError()
    {
        var cut = Render<Probe>(parameters => parameters.Add(p => p.Content, "x"));
        _processor.RenderToHtmlAsync(Arg.Any<string>(), Arg.Any<bool>()).ThrowsAsync(new InvalidOperationException("bad markdown"));

        await cut.InvokeAsync(() => cut.Instance.UpdatePreviewAsync());

        cut.Instance.Error.ShouldBe("Error rendering preview: bad markdown");
    }

    [Fact]
    public void MarkdownEditorWithPreview_AutoFocus_FocusesEditor()
    {
        RenderEditor("x", p => p.Add(c => c.AutoFocus, true));

        Module.VerifyInvoke("focusElement");
    }

    [Fact]
    public void MarkdownEditorWithPreview_AutoFocusFails_StillRenders()
    {
        Module.SetupVoid("focusElement", _ => true).SetException(new JSException("no focus"));

        var cut = RenderEditor("x", p => p.Add(c => c.AutoFocus, true));

        cut.Find("textarea.editor-textarea").ShouldNotBeNull();
    }

    [Fact]
    public async Task MarkdownEditorWithPreview_FocusWithoutModule_DoesNotCallJs()
    {
        MakeModuleImportFail();
        var cut = RenderEditor("x");

        await cut.InvokeAsync(() => cut.Instance.FocusEditorAsync());

        ShouldOnlyHaveTriedTheImport();
    }

    [Fact]
    public void MarkdownEditorWithPreview_ToolbarWithModule_UsesInsertionResult()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("insertTextAtCursor", _ => true)
            .SetResult(new MarkdownEditorWithPreview.InsertionResult { Text = "a\n**bold text**", CaretPosition = 15 });
        var cut = RenderEditor("a\n");

        ClickToolbar(cut, "Bold");

        var invocation = Module.VerifyInvoke("insertTextAtCursor");
        invocation.Arguments.Skip(1).ShouldBe(["**", "**", "bold text"]);
        _changes.ShouldBe(["a\n**bold text**"]);
        StatusBar(cut).ShouldBe("Line: 2 Col: 14");
    }

    [Fact]
    public void MarkdownEditorWithPreview_ToolbarResultUnchanged_DoesNotRaiseChange()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("insertTextAtCursor", _ => true)
            .SetResult(new MarkdownEditorWithPreview.InsertionResult { Text = "same", CaretPosition = 2 });
        var cut = RenderEditor("same");

        ClickToolbar(cut, "Italic");

        _changes.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditorWithPreview_ToolbarInsertionFails_AppendsMarkdown()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("insertTextAtCursor", _ => true)
            .SetException(new JSException("broken"));
        var cut = RenderEditor("Text ");

        ClickToolbar(cut, "Link");

        _changes.ShouldBe(["Text [link text](url)"]);
    }

    [Fact]
    public void MarkdownEditorWithPreview_ToolbarWithoutModule_AppendsMarkdown()
    {
        MakeModuleImportFail();
        var cut = RenderEditor("Text ");

        ClickToolbar(cut, "Heading 2");

        _changes.ShouldBe(["Text ## Heading 2"]);
    }

    [Theory]
    [InlineData("Strikethrough", "~~strikethrough text~~")]
    [InlineData("Heading 1", "# Heading 1")]
    [InlineData("Heading 3", "### Heading 3")]
    [InlineData("Unordered List", "- List item")]
    [InlineData("Ordered List", "1. List item")]
    [InlineData("Task List", "- [ ] Task item")]
    [InlineData("Image", "![alt text](image-url)")]
    [InlineData("Code Block", "```\ncode\n```")]
    [InlineData("Inline Code", "`code`")]
    [InlineData("Quote", "> Blockquote")]
    [InlineData("Horizontal Rule", "\n\n---\n\n")]
    [InlineData("Table", "| Header 1 | Header 2 |\n| -------- | -------- |\n| Cell 1   | Cell 2   |\n")]
    [InlineData("Bold", "**bold text**")]
    [InlineData("Italic", "*italic text*")]
    public void MarkdownEditorWithPreview_ToolbarButtonsWithoutModule_AppendTheirMarkdown(string title, string expected)
    {
        MakeModuleImportFail();
        var cut = RenderEditor(string.Empty);

        ClickToolbar(cut, title);

        _changes.ShouldBe([expected]);
    }

    [Fact]
    public void MarkdownEditorWithPreview_KeyDown_ShowsCaretLineAndColumn()
    {
        Module.Setup<int>("getCaretPosition", _ => true).SetResult(4);
        var cut = RenderEditor("ab\ncd");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "a" });

        StatusBar(cut).ShouldBe("Line: 2 Col: 2");
    }

    [Fact]
    public void MarkdownEditorWithPreview_KeyDownOnFirstLine_ShowsColumnAfterCaret()
    {
        Module.Setup<int>("getCaretPosition", _ => true).SetResult(2);
        var cut = RenderEditor("abcd");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        StatusBar(cut).ShouldBe("Line: 1 Col: 3");
    }

    [Fact]
    public void MarkdownEditorWithPreview_CaretLookupFails_KeepsFirstPosition()
    {
        Module.Setup<int>("getCaretPosition", _ => true).SetException(new JSException("no caret"));
        var cut = RenderEditor("ab\ncd");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "a" });

        StatusBar(cut).ShouldBe("Line: 1 Col: 1");
    }

    [Fact]
    public void MarkdownEditorWithPreview_Tab_IndentsThroughModule()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("handleTabKey", _ => true)
            .SetResult(new MarkdownEditorWithPreview.InsertionResult { Text = "    x", CaretPosition = 4 });
        var cut = RenderEditor("x");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "Tab", ShiftKey = true });

        Module.VerifyInvoke("handleTabKey").Arguments[1].ShouldBe(true);
        _changes.ShouldBe(["    x"]);
        StatusBar(cut).ShouldBe("Line: 1 Col: 5");
    }

    [Fact]
    public void MarkdownEditorWithPreview_TabWithoutChange_DoesNotRaiseChange()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("handleTabKey", _ => true)
            .SetResult(new MarkdownEditorWithPreview.InsertionResult { Text = "x", CaretPosition = 1 });
        var cut = RenderEditor("x");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "Tab" });

        _changes.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditorWithPreview_TabHandlingFails_KeepsContent()
    {
        Module.Setup<MarkdownEditorWithPreview.InsertionResult>("handleTabKey", _ => true)
            .SetException(new JSException("no tab"));
        var cut = RenderEditor("x");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "Tab" });

        _changes.ShouldBeEmpty();
        cut.Find("textarea.editor-textarea").GetAttribute("value").ShouldBe("x");
    }

    [Fact]
    public void MarkdownEditorWithPreview_TabWithoutModule_DoesNotCallJs()
    {
        MakeModuleImportFail();
        var cut = RenderEditor("x");

        cut.Find("textarea.editor-textarea").KeyDown(new KeyboardEventArgs { Key = "Tab" });

        ShouldOnlyHaveTriedTheImport();
        _changes.ShouldBeEmpty();
    }

    [Fact]
    public void MarkdownEditorWithPreview_FocusedEditorScrolled_ScrollsPreview()
    {
        SetupScrollInfo(0.25);
        var cut = RenderEditor("x");
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find("textarea.editor-textarea").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyInvoke("getScrollInfo");
        Module.VerifyInvoke("setScrollPercentage").Arguments[1].ShouldBe(0.25);
    }

    [Fact]
    public void MarkdownEditorWithPreview_TypingWhileFocused_SyncsPreviewScroll()
    {
        SetupScrollInfo(0.5);
        var cut = RenderEditor("x");
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find("textarea.editor-textarea").Input("xy");

        Module.VerifyInvoke("setScrollPercentage").Arguments[1].ShouldBe(0.5);
    }

    [Fact]
    public void MarkdownEditorWithPreview_UnfocusedEditorScrolled_DoesNotSync()
    {
        var cut = RenderEditor("x");
        cut.Find("textarea.editor-textarea").Focus();
        cut.Find("textarea.editor-textarea").Blur();

        cut.Find("textarea.editor-textarea").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("getScrollInfo");
    }

    [Fact]
    public void MarkdownEditorWithPreview_SyncScrollOff_DoesNotSync()
    {
        var cut = RenderEditor("x", p => p.Add(c => c.SyncScroll, false));
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find("textarea.editor-textarea").TriggerEvent("onscroll", EventArgs.Empty);
        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("getScrollInfo");
    }

    [Fact]
    public void MarkdownEditorWithPreview_EditModeScrolled_DoesNotSync()
    {
        var cut = RenderEditor("x");
        cut.FindAll(".editor-tabs button")[0].Click();
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find("textarea.editor-textarea").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("getScrollInfo");
    }

    [Fact]
    public void MarkdownEditorWithPreview_ScrolledWithoutModule_DoesNotCallJs()
    {
        MakeModuleImportFail();
        var cut = RenderEditor("x");
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find("textarea.editor-textarea").TriggerEvent("onscroll", EventArgs.Empty);
        cut.Find("textarea.editor-textarea").Blur();
        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        ShouldOnlyHaveTriedTheImport();
    }

    [Fact]
    public void MarkdownEditorWithPreview_PreviewScrolled_ScrollsEditor()
    {
        SetupScrollInfo(0.75);
        var cut = RenderEditor("x");

        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyInvoke("setScrollPercentage").Arguments[1].ShouldBe(0.75);
    }

    [Fact]
    public void MarkdownEditorWithPreview_PreviewScrolledWhileEditorFocused_DoesNotSync()
    {
        var cut = RenderEditor("x");
        cut.Find("textarea.editor-textarea").Focus();

        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("getScrollInfo");
    }

    [Fact]
    public void MarkdownEditorWithPreview_PreviewModeScrolled_DoesNotSync()
    {
        var cut = RenderEditor("x");
        cut.FindAll(".editor-tabs button")[1].Click();

        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("getScrollInfo");
    }

    [Fact]
    public void MarkdownEditorWithPreview_ScrollInfoFails_DoesNotApplyScroll()
    {
        Module.Setup<MarkdownEditorWithPreview.ScrollInfo>("getScrollInfo", _ => true)
            .SetException(new JSException("no scroll"));
        var cut = RenderEditor("x");

        cut.Find(".preview-container").TriggerEvent("onscroll", EventArgs.Empty);

        Module.VerifyNotInvoke("setScrollPercentage");
    }

    private sealed class Probe : MarkdownEditorWithPreview
    {
        public string? Error => ErrorMessage;
    }
}
