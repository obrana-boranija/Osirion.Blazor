using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

public partial class MarkdownEditor(IJSRuntime jSRuntime)
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ContentChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = "Enter markdown here...";

    [Parameter]
    public List<MarkdownToolbarAction> ToolbarActions { get; set; } = DefaultToolbarActions();

    private ElementReference _textAreaRef;

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Tab")
        {
            await HandleTabKey(e.ShiftKey);
        }
    }

    private async Task HandleTabKey(bool isShiftKey)
    {
        await jSRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const textarea = document.querySelector('[_bl_{_textAreaRef.Id}]');
                    const start = textarea.selectionStart;
                    const end = textarea.selectionEnd;

                    if (start !== end) {{
                        const selectedText = textarea.value.substring(start, end);
                        
                        if (selectedText.includes('\n')) {{
                            const before = textarea.value.substring(0, start);
                            const after = textarea.value.substring(end);
                            
                            const newText = {(isShiftKey ?
                            "selectedText.replace(/^(\\t|  )/gm, '')" :
                            "selectedText.replace(/^/gm, '\\t')")};
                            
                            textarea.value = before + newText + after;
                            textarea.selectionStart = start;
                            textarea.selectionEnd = start + newText.length;
                        }}
                    }} else if (!{isShiftKey.ToString().ToLowerInvariant()}) {{
                        const before = textarea.value.substring(0, start);
                        const after = textarea.value.substring(end);
                        
                        textarea.value = before + '\\t' + after;
                        textarea.selectionStart = textarea.selectionEnd = start + 1;
                    }}
                    
                    // Prevent default tab behavior
                    event.preventDefault();
                    
                    // Trigger input event to update Blazor binding
                    textarea.dispatchEvent(new Event('input'));
                }})();
            ");
    }

    private string GetCssClass()
    {
        return $"osirion-markdown-editor {CssClass}".Trim();
    }

    private static List<MarkdownToolbarAction> DefaultToolbarActions()
    {
        return new List<MarkdownToolbarAction>
        {
            new MarkdownToolbarAction("B", content => WrapText(content, "**", "**", "bold text")),
            new MarkdownToolbarAction("I", content => WrapText(content, "*", "*", "italic text")),
            new MarkdownToolbarAction("H", content => WrapText(content, "## ", "", "Heading")),
            new MarkdownToolbarAction("Link", content => WrapText(content, "[", "](url)", "link text"))
        };
    }

    private static string WrapText(string content, string prefix, string suffix, string defaultText)
    {
        // In-memory text wrapping logic
        // Note: Actual cursor positioning requires JavaScript
        return string.IsNullOrEmpty(content.Trim())
            ? $"{prefix}{defaultText}{suffix}"
            : content;
    }
}

/// <summary>
/// Represents a toolbar action in the Markdown editor
/// </summary>
public class MarkdownToolbarAction
{
    /// <summary>
    /// Label for the toolbar button
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Action to perform when the button is clicked
    /// </summary>
    public Func<string, string> Action { get; }

    /// <summary>
    /// Creates a new Markdown toolbar action
    /// </summary>
    public MarkdownToolbarAction(string label, Func<string, string> action)
    {
        Label = label;
        Action = action;
    }
}