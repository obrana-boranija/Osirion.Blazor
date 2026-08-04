using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

/// <summary>Specifies the display mode for the markdown editor.</summary>
public enum EditorMode
{
    /// <summary>Performs the public member operation.</summary>
    Edit,
    /// <summary>Performs the public member operation.</summary>
    Preview,
    /// <summary>Performs the public member operation.</summary>
    Split
}

/// <summary>Provides an interactive markdown editor with a rendered preview.</summary>
public partial class MarkdownEditorWithPreview : IAsyncDisposable
{
    /// <summary>Gets or sets the markdown content.</summary>
    [Parameter]
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the callback invoked when content changes.</summary>
    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    /// <summary>Gets or sets the editor label.</summary>
    [Parameter]
    public string EditorLabel { get; set; } = "Markdown";

    /// <summary>Gets or sets the preview label.</summary>
    [Parameter]
    public string PreviewLabel { get; set; } = "Preview";

    /// <summary>Gets or sets whether the preview is visible.</summary>
    [Parameter]
    public bool ShowPreview { get; set; } = true;

    /// <summary>Gets or sets whether the toolbar is visible.</summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>Gets or sets whether the editor receives focus automatically.</summary>
    [Parameter]
    public bool AutoFocus { get; set; } = false;

    /// <summary>Gets or sets whether editor and preview scrolling are synchronized.</summary>
    [Parameter]
    public bool SyncScroll { get; set; } = true;

    /// <summary>Gets or sets whether browser spell checking is enabled.</summary>
    [Parameter]
    public bool SpellCheck { get; set; } = false;

    [Inject]
    private IMarkdownProcessor MarkdownProcessor { get; set; } = default!;

    private string EditorContent
    {
        get => Content;
        set
        {
            if (Content != value)
            {
                Content = value;
                _ = ContentChanged.InvokeAsync(value);
                _ = UpdatePreviewAsync();
                CalculateLineAndColumn(value);
            }
        }
    }

    private ElementReference TextAreaRef;
    private ElementReference PreviewRef;
    private string Preview = string.Empty;
    private bool isEditorFocused = false;
    private double editorScrollPercentage = 0;
    private double previewScrollPercentage = 0;
    private IJSObjectReference? jsModule;
    private bool jsInteropAvailable = false;
    private bool IsFullscreen = false;
    private EditorMode EditorMode { get; set; } = EditorMode.Split;
    private int CurrentLine { get; set; } = 1;
    private int CurrentColumn { get; set; } = 1;
    private int caretPosition = 0;

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await UpdatePreviewAsync();
        CalculateLineAndColumn(Content);
    }

    /// <summary>Performs the OnAfterRender operation asynchronously.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Import the JavaScript module
                jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/Osirion.Blazor.Cms.Admin/js/markdownEditor.js");

                jsInteropAvailable = true;

                if (AutoFocus)
                {
                    await FocusEditorAsync();
                }
            }
            catch (Exception ex)
            {
                // If JS interop fails, log error but continue without it
                Console.WriteLine($"JavaScript interop initialization failed: {ex.Message}");
                jsInteropAvailable = false;
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>Renders the current markdown content into the preview.</summary>
    public async Task UpdatePreviewAsync()
    {
        try
        {
            Preview = await MarkdownProcessor.RenderToHtmlAsync(Content);
            StateHasChanged();

            if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule is not null && EditorMode == EditorMode.Split)
            {
                await SyncScrollPositionAsync(true);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error rendering preview: {ex.Message}";
        }
    }

    private void CalculateLineAndColumn(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || caretPosition == 0)
        {
            CurrentLine = 1;
            CurrentColumn = 1;
            return;
        }

        // Get the text up to the caret position
        string textUpToCaret = text.Substring(0, Math.Min(caretPosition, text.Length));

        // Count the number of newlines for line number
        CurrentLine = Regex.Matches(textUpToCaret, "\n").Count + 1;

        // Find the last newline before the caret position
        int lastNewlineIndex = textUpToCaret.LastIndexOf('\n');
        if (lastNewlineIndex == -1)
        {
            // If there's no newline, the column is the caret position + 1
            CurrentColumn = caretPosition + 1;
        }
        else
        {
            // The column is the number of characters after the last newline + 1
            CurrentColumn = textUpToCaret.Length - lastNewlineIndex;
        }
    }

    /// <summary>Focuses the markdown editor.</summary>
    public async Task FocusEditorAsync()
    {
        try
        {
            if (jsInteropAvailable && jsModule is not null)
            {
                await jsModule.InvokeVoidAsync("focusElement", TextAreaRef);
            }
        }
        catch
        {
            // Silently fail if JS interop isn't available
        }
    }

    /// <summary>Inserts markdown around the current selection.</summary>
    public async Task InsertMarkdown(string prefix, string suffix, string placeholder)
    {
        try
        {
            if (jsInteropAvailable && jsModule is not null)
            {
                var result = await jsModule.InvokeAsync<InsertionResult>(
                    "insertTextAtCursor", TextAreaRef, prefix, suffix, placeholder);

                if (Content != result.Text)
                {
                    EditorContent = result.Text;
                    caretPosition = result.CaretPosition;
                    CalculateLineAndColumn(result.Text);
                }
            }
            else
            {
                // Fallback for when JS interop isn't available
                EditorContent = Content + prefix + placeholder + suffix;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting markdown: {ex.Message}");
            // Fallback for when JS interop fails
            EditorContent = Content + prefix + placeholder + suffix;
        }
    }

    private void OnEditorFocus()
    {
        isEditorFocused = true;
    }

    private void OnEditorBlur()
    {
        isEditorFocused = false;
    }

    private async Task OnEditorScrolled(EventArgs args)
    {
        if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule is not null && EditorMode == EditorMode.Split)
        {
            await SyncScrollPositionAsync(true);
        }
    }

    private async Task OnPreviewScrolled(EventArgs args)
    {
        if (SyncScroll && !isEditorFocused && jsInteropAvailable && jsModule is not null && EditorMode == EditorMode.Split)
        {
            await SyncScrollPositionAsync(false);
        }
    }

    private async Task SyncScrollPositionAsync(bool fromEditor)
    {
        try
        {
            if (!jsInteropAvailable || jsModule is null) return;

            if (fromEditor)
            {
                // Get editor scroll info
                var scrollInfo = await jsModule.InvokeAsync<ScrollInfo>("getScrollInfo", TextAreaRef);
                editorScrollPercentage = scrollInfo.Percentage;

                // Apply to preview
                await jsModule.InvokeVoidAsync("setScrollPercentage", PreviewRef, editorScrollPercentage);
            }
            else
            {
                // Get preview scroll info
                var scrollInfo = await jsModule.InvokeAsync<ScrollInfo>("getScrollInfo", PreviewRef);
                previewScrollPercentage = scrollInfo.Percentage;

                // Apply to editor
                await jsModule.InvokeVoidAsync("setScrollPercentage", TextAreaRef, previewScrollPercentage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing scroll: {ex.Message}");
            // Do not rethrow - this is non-critical functionality
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        // Get current caret position after any key press
        try
        {
            if (jsInteropAvailable && jsModule is not null)
            {
                caretPosition = await jsModule.InvokeAsync<int>("getCaretPosition", TextAreaRef);
                CalculateLineAndColumn(Content);
            }
        }
        catch
        {
            // Silently ignore any errors getting caret position
        }

        // Handle tab key for indentation
        if (e.Key == "Tab")
        {
            if (jsInteropAvailable && jsModule is not null)
            {
                try
                {
                    var result = await jsModule.InvokeAsync<InsertionResult>(
                        "handleTabKey", TextAreaRef, e.ShiftKey);

                    if (Content != result.Text)
                    {
                        EditorContent = result.Text;
                        caretPosition = result.CaretPosition;
                        CalculateLineAndColumn(result.Text);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Tab key handling error: {ex.Message}");
                    // Just continue - tab key handling is a non-critical enhancement
                }
            }
        }
    }

    private async Task ToggleFullscreen()
    {
        if (jsInteropAvailable && jsModule is not null)
        {
            try
            {
                IsFullscreen = await jsModule.InvokeAsync<bool>("toggleFullscreen", ".markdown-editor");
                StateHasChanged();
            }
            catch
            {
                // Silently fail if JS interop isn't available
            }
        }
    }

    private void SetEditorMode(EditorMode mode)
    {
        EditorMode = mode;
        StateHasChanged();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        try
        {
            if (jsModule is not null)
            {
                await jsModule.DisposeAsync();
            }
        }
        catch
        {
            // Ignore errors during disposal
        }
    }

    private class ScrollInfo
    {
        public double ScrollTop { get; set; }
        public double ScrollHeight { get; set; }
        public double ClientHeight { get; set; }
        public double Percentage { get; set; }
    }

    private class InsertionResult
    {
        public string Text { get; set; } = string.Empty;
        public int CaretPosition { get; set; }
    }
}
