using Markdig;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms;

public partial class MarkdownEditorPreview
{
    [Parameter] public string Content { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ContentChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Enter markdown here...";
    [Parameter] public string EditorTitle { get; set; } = "Editor";
    [Parameter] public string PreviewTitle { get; set; } = "Preview";
    [Parameter] public string PreviewPlaceholder { get; set; } = "Preview will appear here";
    [Parameter] public bool AutoFocus { get; set; } = false;
    [Parameter] public bool SyncScroll { get; set; } = true;
    [Parameter] public bool ShowPreview { get; set; } = true;
    [Parameter] public EventCallback<bool> ShowPreviewChanged { get; set; }
    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public bool ShowEditorHeader { get; set; } = true;
    [Parameter] public bool ShowPreviewHeader { get; set; } = true;
    [Parameter] public bool ShowActionsBar { get; set; } = true;
    [Parameter] public string EditorCssClass { get; set; } = string.Empty;
    [Parameter] public string PreviewCssClass { get; set; } = string.Empty;
    [Parameter] public string PreviewContentCssClass { get; set; } = string.Empty;
    [Parameter] public MarkdownPipeline? Pipeline { get; set; }
    [Parameter] public List<ToolbarAction>? ToolbarActions { get; set; }

    private MarkdownEditor? EditorRef;
    private MarkdownPreview? PreviewRef;
    private bool _isSyncing = false;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// Gets the CSS class for the component
    /// </summary>
    private string GetCssClass()
    {
        var classes = new List<string>
        {
            "osirion-markdown-editor-preview",
            ShowPreview ? "preview-visible" : "preview-hidden",
            CssClass ?? string.Empty
        };

        return string.Join(" ", classes).Trim();
    }

    /// <summary>
    /// Handles content changes from the editor
    /// </summary>
    private async Task HandleContentChanged(string value)
    {
        Content = value;

        if (ContentChanged.HasDelegate)
        {
            await ContentChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Handles scroll events from the editor
    /// </summary>
    private async Task HandleEditorScroll(double position)
    {
        if (!SyncScroll || _isSyncing || PreviewRef is null || !ShowPreview)
            return;

        try
        {
            _isSyncing = true;
            await PreviewRef.SetScrollPositionAsync(position);
        }
        finally
        {
            _isSyncing = false;
        }
    }

    /// <summary>
    /// Handles scroll events from the preview
    /// </summary>
    private async Task HandlePreviewScroll(double position)
    {
        if (!SyncScroll || _isSyncing || EditorRef is null || !ShowPreview)
            return;

        try
        {
            _isSyncing = true;
            await EditorRef.SetScrollPositionAsync(position);
        }
        finally
        {
            _isSyncing = false;
        }
    }

    /// <summary>
    /// Toggles the preview visibility
    /// </summary>
    private async Task TogglePreview()
    {
        ShowPreview = !ShowPreview;

        if (ShowPreviewChanged.HasDelegate)
        {
            await ShowPreviewChanged.InvokeAsync(ShowPreview);
        }
    }

    /// <summary>
    /// Focuses the editor
    /// </summary>
    public async Task FocusEditorAsync()
    {
        if (EditorRef is not null)
        {
            await EditorRef.FocusAsync();
        }
    }

    /// <summary>
    /// Inserts text at the cursor position
    /// </summary>
    public async Task InsertTextAsync(string text)
    {
        if (EditorRef is not null)
        {
            await EditorRef.InsertTextAsync(text);
        }
    }

    /// <summary>
    /// Wraps selected text with prefix and suffix
    /// </summary>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText)
    {
        if (EditorRef is not null)
        {
            await EditorRef.WrapTextAsync(prefix, suffix, defaultText);
        }
    }
}