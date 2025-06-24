using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

/// <summary>
/// TagInput component for managing a list of tags
/// </summary>
public partial class TagInput
{
    /// <summary>
    /// List of tags to manage
    /// </summary>
    [Parameter]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Event callback that is triggered when the list of tags changes
    /// </summary>
    [Parameter]
    public EventCallback<List<string>> TagsChanged { get; set; }

    /// <summary>
    /// Placeholder text for the input field
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Add tag and press Enter";

    /// <summary>
    /// Maximum number of tags allowed. 0 means unlimited.
    /// </summary>
    [Parameter]
    public int MaxTags { get; set; } = 0; // 0 means unlimited

    private ElementReference inputElement;
    private string CurrentInput { get; set; } = "";

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == ",")
        {
            await AddTag();
        }
        else if (e.Key == "Backspace" && string.IsNullOrWhiteSpace(CurrentInput) && Tags.Count > 0)
        {
            await RemoveTag(Tags.Last());
        }
    }

    private async Task AddTag()
    {
        var tag = CurrentInput.Trim();

        if (string.IsNullOrWhiteSpace(tag))
            return;

        if (MaxTags > 0 && Tags.Count >= MaxTags)
            return;

        if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            Tags.Add(tag);
            CurrentInput = "";

            if (TagsChanged.HasDelegate)
            {
                await TagsChanged.InvokeAsync(Tags);
            }
        }
        else
        {
            // Tag already exists - clear input
            CurrentInput = "";
        }
    }

    private async Task RemoveTag(string tag)
    {
        Tags.Remove(tag);

        if (TagsChanged.HasDelegate)
        {
            await TagsChanged.InvokeAsync(Tags);
        }
    }

    private async Task FocusInput()
    {
        await inputElement.FocusAsync();
    }
}