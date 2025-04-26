using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Models;

namespace Osirion.Blazor.Cms.Admin.Components.Editor;

public partial class MetadataEditor
{
    [Parameter]
    public FrontMatter Metadata { get; set; } = new();

    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    private string CategoriesInput
    {
        get => string.Join(", ", Metadata.Categories);
        set
        {
            Metadata.Categories = ParseList(value);
            NotifyMetadataChanged();
        }
    }

    private string TagsInput
    {
        get => string.Join(", ", Metadata.Tags);
        set
        {
            Metadata.Tags = ParseList(value);
            NotifyMetadataChanged();
        }
    }

    private DateTime PostDate
    {
        get
        {
            if (DateTime.TryParse(Metadata.Date, out var date))
            {
                return date;
            }

            return DateTime.Now;
        }
        set
        {
            Metadata.Date = value.ToString("yyyy-MM-dd");
            NotifyMetadataChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // Initialize with empty collections if null
        Metadata.Categories ??= new List<string>();
        Metadata.Tags ??= new List<string>();
    }

    private List<string> ParseList(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new List<string>();
        }

        return input
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
    }

    private async Task NotifyMetadataChanged()
    {
        if (MetadataChanged.HasDelegate)
        {
            await MetadataChanged.InvokeAsync(Metadata);
        }
    }

    private string GetFormattedFrontMatter()
    {
        return Metadata.ToYaml();
    }

    private string GetMetadataEditorClass()
    {
        return $"osirion-admin-metadata-editor {CssClass}".Trim();
    }
}
