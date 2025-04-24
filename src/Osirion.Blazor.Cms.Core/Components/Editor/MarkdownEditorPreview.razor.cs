using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownEditorPreview
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    [Parameter]
    public string PreviewTitle { get; set; } = "Preview";

    [Parameter]
    public string EditorCssClass { get; set; } = string.Empty;

    [Parameter]
    public string PreviewCssClass { get; set; } = string.Empty;

    private async Task HandleContentChanged(string newContent)
    {
        Content = newContent;

        if (ContentChanged.HasDelegate)
        {
            await ContentChanged.InvokeAsync(newContent);
        }
    }

    private string GetCssClass()
    {
        var classes = new List<string>
        {
            "osirion-markdown-editor-preview",
            ShowPreview ? "preview-visible" : "preview-hidden",
            CssClass ?? ""
        };

        return string.Join(" ", classes.Where(c => !string.IsNullOrWhiteSpace(c)));
    }
}