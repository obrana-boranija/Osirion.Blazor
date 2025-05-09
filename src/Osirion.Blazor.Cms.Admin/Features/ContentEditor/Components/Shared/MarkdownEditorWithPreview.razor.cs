using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Admin.Shared.Components;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class MarkdownEditorWithPreview : BaseComponent, IAsyncDisposable
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

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

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await UpdatePreviewAsync();
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

            if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule != null)
            {
                await SyncScrollPositionAsync(true);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error rendering preview: {ex.Message}";
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
                string newContent = await jsModule.InvokeAsync<string>(
                    "insertTextAtCursor", TextAreaRef, prefix, suffix, placeholder);

                if (Content != newContent)
                {
                    EditorContent = newContent;
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
        if (SyncScroll && isEditorFocused && jsInteropAvailable && jsModule != null)
        {
            await SyncScrollPositionAsync(true);
        }
    }

    private async Task OnPreviewScrolled(EventArgs args)
    {
        if (SyncScroll && !isEditorFocused && jsInteropAvailable && jsModule != null)
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
        // Handle tab key for indentation
        if (e.Key == "Tab")
        {
            if (jsInteropAvailable && jsModule != null)
            {
                try
                {
                    string newContent = await jsModule.InvokeAsync<string>(
                        "handleTabKey", TextAreaRef, e.ShiftKey);

                    if (Content != newContent)
                    {
                        EditorContent = newContent;
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
}