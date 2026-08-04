using Markdig;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms;

/// <summary>Provides a split markdown editor and rendered preview.</summary>
public partial class MarkdownEditorPreview
{
    /// <summary>Gets or sets the markdown content.</summary>
    [Parameter] public string Content { get; set; } = string.Empty;
    /// <summary>Gets or sets the callback invoked when content changes.</summary>
    [Parameter] public EventCallback<string> ContentChanged { get; set; }
    /// <summary>Gets or sets the editor placeholder text.</summary>
    [Parameter] public string Placeholder { get; set; } = "Enter markdown here...";
    /// <summary>Gets or sets the editor header text.</summary>
    [Parameter] public string EditorTitle { get; set; } = "Editor";
    /// <summary>Gets or sets the preview header text.</summary>
    [Parameter] public string PreviewTitle { get; set; } = "Preview";
    /// <summary>Gets or sets the placeholder shown when the preview is empty.</summary>
    [Parameter] public string PreviewPlaceholder { get; set; } = "Preview will appear here";
    /// <summary>Gets or sets whether the editor receives focus automatically.</summary>
    [Parameter] public bool AutoFocus { get; set; } = false;
    /// <summary>Gets or sets whether editor and preview scrolling are synchronized.</summary>
    [Parameter] public bool SyncScroll { get; set; } = true;
    /// <summary>Gets or sets whether the preview is visible.</summary>
    [Parameter] public bool ShowPreview { get; set; } = true;
    /// <summary>Gets or sets the callback invoked when preview visibility changes.</summary>
    [Parameter] public EventCallback<bool> ShowPreviewChanged { get; set; }
    /// <summary>Gets or sets whether the editor toolbar is visible.</summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;
    /// <summary>Gets or sets whether the editor header is visible.</summary>
    [Parameter] public bool ShowEditorHeader { get; set; } = true;
    /// <summary>Gets or sets whether the preview header is visible.</summary>
    [Parameter] public bool ShowPreviewHeader { get; set; } = true;
    /// <summary>Gets or sets whether the actions bar is visible.</summary>
    [Parameter] public bool ShowActionsBar { get; set; } = true;
    /// <summary>Gets or sets the CSS class applied to the editor.</summary>
    [Parameter] public string EditorCssClass { get; set; } = string.Empty;
    /// <summary>Gets or sets the CSS class applied to the preview.</summary>
    [Parameter] public string PreviewCssClass { get; set; } = string.Empty;
    /// <summary>Gets or sets the CSS class applied to preview content.</summary>
    [Parameter] public string PreviewContentCssClass { get; set; } = string.Empty;
    /// <summary>Gets or sets the Markdig pipeline used to render markdown.</summary>
    [Parameter] public MarkdownPipeline? Pipeline { get; set; }
    /// <summary>Gets or sets the actions displayed in the editor toolbar.</summary>
    [Parameter] public List<ToolbarAction>? ToolbarActions { get; set; }

    private MarkdownEditor? EditorRef;
    private MarkdownPreview? PreviewRef;
    private bool _isSyncing = false;

    /// <summary>Initializes the component state and required services.</summary>
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
            Class ?? string.Empty
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

    /// <summary>Focuses the editor.</summary>
    public async Task FocusEditorAsync()
    {
        if (EditorRef is not null)
        {
            await EditorRef.FocusAsync();
        }
    }

    /// <summary>Inserts text at the cursor position.</summary>
    public async Task InsertTextAsync(string text)
    {
        if (EditorRef is not null)
        {
            await EditorRef.InsertTextAsync(text);
        }
    }

    /// <summary>Wraps selected text with a prefix and suffix.</summary>
    public async Task WrapTextAsync(string prefix, string suffix, string defaultText)
    {
        if (EditorRef is not null)
        {
            await EditorRef.WrapTextAsync(prefix, suffix, defaultText);
        }
    }
}
