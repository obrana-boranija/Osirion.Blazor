using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using System.Text;

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
    private IJSObjectReference? minimalJsModule;
    private bool jsInteropAvailable = false;
    private bool isPrerendering = true;

    protected override async Task OnInitializedAsync()
    {
        // Check if we're prerendering
        isPrerendering = NavigationManager.Uri.StartsWith("http://localhost:") == false &&
                          NavigationManager.Uri.Contains("://");

        await base.OnInitializedAsync();
        await UpdatePreviewAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Load only the minimal required JavaScript
                minimalJsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/Osirion.Blazor.Cms.Admin/js/minimalMarkdownEditor.js");

                jsInteropAvailable = true;

                if (AutoFocus && !isPrerendering)
                {
                    await FocusEditorAsync();
                }
            }
            catch (Exception ex)
            {
                // JS interop not available - continue without it
                jsInteropAvailable = false;
                Console.WriteLine($"JS interop not available: {ex.Message}");
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

            if (SyncScroll && isEditorFocused && jsInteropAvailable && !isPrerendering)
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
        if (isPrerendering) return;

        try
        {
            if (jsInteropAvailable && minimalJsModule != null)
            {
                await minimalJsModule.InvokeVoidAsync("focusElement", TextAreaRef);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Focus error: {ex.Message}");
            // Silently fail if JS interop isn't available
        }
    }

    public async Task InsertMarkdown(string prefix, string suffix, string placeholder)
    {
        if (isPrerendering) return;

        if (jsInteropAvailable && minimalJsModule != null)
        {
            try
            {
                string newContent = await minimalJsModule.InvokeAsync<string>(
                    "insertTextAtCursor", TextAreaRef, prefix, suffix, placeholder);

                if (Content != newContent)
                {
                    Content = newContent;
                    await ContentChanged.InvokeAsync(Content);
                    await UpdatePreviewAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insert markdown error: {ex.Message}");
                // Fallback for when JS interop fails
                Content = InsertTextManually(Content, prefix, suffix, placeholder);
                await ContentChanged.InvokeAsync(Content);
                await UpdatePreviewAsync();
            }
        }
        else
        {
            // Fallback for when JS interop isn't available
            Content = Content + prefix + placeholder + suffix;
            await ContentChanged.InvokeAsync(Content);
            await UpdatePreviewAsync();
        }
    }

    private string InsertTextManually(string text, string prefix, string suffix, string placeholder)
    {
        // This is a simple fallback when JS interop is not available
        return text + prefix + placeholder + suffix;
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
        if (isPrerendering) return;

        if (SyncScroll && isEditorFocused && jsInteropAvailable)
        {
            await SyncScrollPositionAsync(true);
        }
    }

    private async Task OnPreviewScrolled(EventArgs args)
    {
        if (isPrerendering) return;

        if (SyncScroll && !isEditorFocused && jsInteropAvailable)
        {
            await SyncScrollPositionAsync(false);
        }
    }

    private async Task SyncScrollPositionAsync(bool fromEditor)
    {
        if (isPrerendering || !jsInteropAvailable || minimalJsModule == null) return;

        try
        {
            if (fromEditor)
            {
                // Get editor scroll info
                var scrollInfo = await minimalJsModule.InvokeAsync<ScrollInfo>("getScrollInfo", TextAreaRef);
                editorScrollPercentage = scrollInfo.Percentage;

                // Apply to preview
                await minimalJsModule.InvokeVoidAsync("setScrollPercentage", PreviewRef, editorScrollPercentage);
            }
            else
            {
                // Get preview scroll info
                var scrollInfo = await minimalJsModule.InvokeAsync<ScrollInfo>("getScrollInfo", PreviewRef);
                previewScrollPercentage = scrollInfo.Percentage;

                // Apply to editor
                await minimalJsModule.InvokeVoidAsync("setScrollPercentage", TextAreaRef, previewScrollPercentage);
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
        if (isPrerendering) return;

        // Handle tab key for indentation
        if (e.Key == "Tab")
        {
            if (jsInteropAvailable && minimalJsModule != null)
            {
                try
                {
                    string newContent = await minimalJsModule.InvokeAsync<string>(
                        "handleTabKey", TextAreaRef, e.ShiftKey);

                    if (Content != newContent)
                    {
                        Content = newContent;
                        await ContentChanged.InvokeAsync(Content);
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
        if (jsInteropAvailable)
        {
            try
            {
                if (minimalJsModule != null)
                {
                    await minimalJsModule.DisposeAsync();
                }
            }
            catch
            {
                // Ignore errors during disposal
            }
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