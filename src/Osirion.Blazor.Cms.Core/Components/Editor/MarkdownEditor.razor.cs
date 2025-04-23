using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownEditor(IJSRuntime JSRuntime)
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Enter markdown content here...";

    [Parameter]
    public bool AutoFocus { get; set; } = false;

    [Parameter]
    public bool SyncScroll { get; set; } = true;

    [Parameter]
    public EventCallback<double> OnScroll { get; set; }

    /// <summary>
    /// Exposes editor scroll information to parent components
    /// </summary>
    public double ScrollPosition => _scrollPosition;

    /// <summary>
    /// Sets scroll position programmatically (for sync scrolling)
    /// </summary>
    /// <param name="position">Scroll position as percentage (0-1)</param>
    /// <returns>Task representing the operation</returns>
    public async Task SetScrollPositionAsync(double position)
    {
        if (!SyncScroll || _isScrolling) return;

        try
        {
            _isScrolling = true;

            if (OperatingSystem.IsBrowser() && TextAreaRef != null)
            {
                await JSRuntime.InvokeVoidAsync("setScrollPosition", TextAreaRef, position);
            }
        }
        finally
        {
            _isScrolling = false;
        }
    }

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusAsync()
    {
        if (OperatingSystem.IsBrowser() && TextAreaRef is not null)
        {
            await JSRuntime.InvokeVoidAsync("focusElement", TextAreaRef);
        }
    }

    /// <summary>
    /// Gets the currently selected text in the editor
    /// </summary>
    public async Task<(string text, int start, int end)> GetSelectionAsync()
    {
        if (OperatingSystem.IsBrowser() && TextAreaRef is not null)
        {
            var selection = await JSRuntime.InvokeAsync<TextSelection>("getTextAreaSelection", TextAreaRef);
            return (selection.Text, selection.Start, selection.End);
        }

        return (string.Empty, 0, 0);
    }

    /// <summary>
    /// Inserts text at the current cursor position or replaces selected text
    /// </summary>
    /// <param name="text">Text to insert</param>
    public async Task InsertTextAsync(string text)
    {
        if (OperatingSystem.IsBrowser() && TextAreaRef is not null)
        {
            await JSRuntime.InvokeVoidAsync("insertTextAtCursor", TextAreaRef, text);

            // Update the bound Content property with the new value from the textarea
            var newContent = await JSRuntime.InvokeAsync<string>("getElementValue", TextAreaRef);
            if (Content != newContent)
            {
                Content = newContent;
                await ContentChanged.InvokeAsync(Content);
            }
        }
    }

    /// <summary>
    /// Wraps the selected text with prefix and suffix
    /// </summary>
    /// <param name="prefix">Text to add before selection</param>
    /// <param name="suffix">Text to add after selection</param>
    /// <param name="defaultText">Text to use if no selection</param>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText = "")
    {
        if (OperatingSystem.IsBrowser() && TextAreaRef != null)
        {
            await JSRuntime.InvokeVoidAsync("wrapTextSelection", TextAreaRef, prefix, suffix, defaultText);

            // Update the bound Content property with the new value from the textarea
            var newContent = await JSRuntime.InvokeAsync<string>("getElementValue", TextAreaRef);
            if (Content != newContent)
            {
                Content = newContent;
                await ContentChanged.InvokeAsync(Content);
            }
        }
    }

    private ElementReference? TextAreaRef;
    private double _scrollPosition;
    private bool _isScrolling = false;
    private DotNetObjectReference<MarkdownEditor>? _dotNetRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && OperatingSystem.IsBrowser())
        {
            // Initialize JS functionality
            _dotNetRef = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("initializeMarkdownEditor", TextAreaRef, _dotNetRef);

            // Set initial focus if requested
            if (AutoFocus)
            {
                await FocusAsync();
            }
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }

    private async Task OnEditorScroll()
    {
        if (!SyncScroll || _isScrolling) return;

        try
        {
            _isScrolling = true;

            if (OperatingSystem.IsBrowser() && TextAreaRef != null)
            {
                // Calculate scroll position as percentage
                var scrollInfo = await JSRuntime.InvokeAsync<ScrollInfo>("getScrollInfo", TextAreaRef);
                _scrollPosition = scrollInfo.Position;

                // Notify parent of scroll position change
                if (OnScroll.HasDelegate)
                {
                    await OnScroll.InvokeAsync(_scrollPosition);
                }
            }
        }
        finally
        {
            _isScrolling = false;
        }
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        // Handle tab key for indentation
        if (e.Key == "Tab")
        {
            if (OperatingSystem.IsBrowser() && TextAreaRef != null)
            {
                await JSRuntime.InvokeVoidAsync("handleTabKey", TextAreaRef, e.ShiftKey);

                // Update the bound Content property with the new value from the textarea
                var newContent = await JSRuntime.InvokeAsync<string>("getElementValue", TextAreaRef);
                if (Content != newContent)
                {
                    Content = newContent;
                    await ContentChanged.InvokeAsync(Content);
                }
            }
        }
    }

    private void OnEditorClick()
    {
        // Reset any special states or handlers on click if needed
    }

    // Toolbar button handlers
    private async Task InsertHeading()
    {
        await WrapTextAsync("## ", "", "Heading");
    }

    private async Task InsertBold()
    {
        await WrapTextAsync("**", "**", "bold text");
    }

    private async Task InsertItalic()
    {
        await WrapTextAsync("*", "*", "italic text");
    }

    private async Task InsertLink()
    {
        await WrapTextAsync("[", "](https://example.com)", "link text");
    }

    private async Task InsertImage()
    {
        await InsertTextAsync("![alt text](https://example.com/image.jpg)");
    }

    private async Task InsertList()
    {
        await InsertTextAsync("\n- Item 1\n- Item 2\n- Item 3");
    }

    private async Task InsertCodeBlock()
    {
        await WrapTextAsync("\n```\n", "\n```", "code goes here");
    }

    private async Task InsertTable()
    {
        var tableTemplate = "\n| Header 1 | Header 2 | Header 3 |\n" +
                           "| -------- | -------- | -------- |\n" +
                           "| Cell 1   | Cell 2   | Cell 3   |\n" +
                           "| Cell 4   | Cell 5   | Cell 6   |";

        await InsertTextAsync(tableTemplate);
    }

    private async Task InsertHorizontalRule()
    {
        await InsertTextAsync("\n\n---\n\n");
    }

    private string GetEditorClass()
    {
        return $"osirion-markdown-editor {CssClass}".Trim();
    }

    [JSInvokable]
    public void UpdateScrollPosition(double position)
    {
        _scrollPosition = position;
    }
}