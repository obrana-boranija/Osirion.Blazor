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
    private DotNetObjectReference<MarkdownEditorWithPreview>? objRef;
    private IJSObjectReference? editorModule;
    private bool isEditorFocused = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await UpdatePreviewAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            objRef = DotNetObjectReference.Create(this);
            editorModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Osirion.Blazor.Cms.Admin/js/markdownEditor.js");

            if (SyncScroll)
            {
                await InitializeSyncScrollAsync();
            }

            if (AutoFocus)
            {
                await FocusEditorAsync();
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

            if (SyncScroll && isEditorFocused && editorModule != null)
            {
                await editorModule.InvokeVoidAsync("syncScrollPositions", TextAreaRef, PreviewRef, isEditorFocused);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error rendering preview: {ex.Message}";
        }
    }

    private async Task InitializeSyncScrollAsync()
    {
        if (editorModule != null)
        {
            await editorModule.InvokeVoidAsync("initializeSyncScroll", TextAreaRef, PreviewRef, objRef);
        }
    }

    public async Task FocusEditorAsync()
    {
        if (editorModule != null)
        {
            await editorModule.InvokeVoidAsync("focusEditor", TextAreaRef);
        }
    }

    public async Task InsertMarkdown(string prefix, string suffix, string placeholder)
    {
        if (editorModule != null)
        {
            string newContent = await editorModule.InvokeAsync<string>("insertText",
                TextAreaRef, prefix, suffix, placeholder);

            if (Content != newContent)
            {
                Content = newContent;
                await ContentChanged.InvokeAsync(Content);
                await UpdatePreviewAsync();
            }
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

    [JSInvokable]
    public async Task OnEditorScrolled()
    {
        if (SyncScroll && isEditorFocused && editorModule != null)
        {
            await editorModule.InvokeVoidAsync("syncScrollPositions", TextAreaRef, PreviewRef, true);
        }
    }

    [JSInvokable]
    public async Task OnPreviewScrolled()
    {
        if (SyncScroll && !isEditorFocused && editorModule != null)
        {
            await editorModule.InvokeVoidAsync("syncScrollPositions", TextAreaRef, PreviewRef, false);
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        // Handle tab key for indentation
        if (e.Key == "Tab")
        {
            if (editorModule != null)
            {
                string newContent = await editorModule.InvokeAsync<string>("handleTabKey",
                    TextAreaRef, e.ShiftKey);

                if (Content != newContent)
                {
                    Content = newContent;
                    await ContentChanged.InvokeAsync(Content);
                }
            }
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        try
        {
            if (editorModule != null)
            {
                await editorModule.InvokeVoidAsync("cleanup", TextAreaRef, PreviewRef);
                await editorModule.DisposeAsync();
            }

            objRef?.Dispose();
        }
        catch
        {
            // Ignore errors during disposal
        }
    }
}