using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public enum EditorMode
{
    Edit,
    Preview,
    Split
}

public partial class MarkdownEditorWithPreview : IAsyncDisposable
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public string EditorLabel { get; set; } = "Markdown";

    [Parameter]
    public string PreviewLabel { get; set; } = "Preview";

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    [Parameter]
    public bool AutoFocus { get; set; } = false;

    [Parameter]
    public bool SyncScroll { get; set; } = true;

    [Parameter]
    public bool SpellCheck { get; set; } = false;

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

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await UpdatePreviewAsync();
        CalculateLineAndColumn(Content);
    }

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

    public async Task UpdatePreviewAsync()
    {
        try
        {
            Preview = await MarkdownService.RenderToHtmlAsync(Content);
            StateHasChanged();

            if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule != null && EditorMode == EditorMode.Split)
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
        if (string.IsNullOrEmpty(text) || caretPosition == 0)
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

    public async Task FocusEditorAsync()
    {
        try
        {
            if (jsInteropAvailable && jsModule != null)
            {
                await jsModule.InvokeVoidAsync("focusElement", TextAreaRef);
            }
        }
        catch
        {
            // Silently fail if JS interop isn't available
        }
    }

    public async Task InsertMarkdown(string prefix, string suffix, string placeholder)
    {
        try
        {
            if (jsInteropAvailable && jsModule != null)
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
        if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule != null && EditorMode == EditorMode.Split)
        {
            await SyncScrollPositionAsync(true);
        }
    }

    private async Task OnPreviewScrolled(EventArgs args)
    {
        if (SyncScroll && !isEditorFocused && jsInteropAvailable && jsModule != null && EditorMode == EditorMode.Split)
        {
            await SyncScrollPositionAsync(false);
        }
    }

    private async Task SyncScrollPositionAsync(bool fromEditor)
    {
        try
        {
            if (!jsInteropAvailable || jsModule == null) return;

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
            if (jsInteropAvailable && jsModule != null)
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
            if (jsInteropAvailable && jsModule != null)
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
        if (jsInteropAvailable && jsModule != null)
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
            if (jsModule != null)
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