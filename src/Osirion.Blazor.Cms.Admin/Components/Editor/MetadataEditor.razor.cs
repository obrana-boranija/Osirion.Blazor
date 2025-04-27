using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Components.Editor;

public partial class MetadataEditor
{
    [Parameter]
    public FrontMatter Metadata { get; set; }

    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    private string CategoriesInput
    {
        get => string.Join(", ", Metadata.Categories);
        set
        {
            Metadata.WithCategories(ParseList(value));
            NotifyMetadataChanged();
        }
    }

    private string TagsInput
    {
        get => string.Join(", ", Metadata.Tags);
        set
        {
            Metadata.WithTags(ParseList(value));
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
            Metadata.WithDate(value);
            NotifyMetadataChanged();
        }
    }

    private List<string> ParseList(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new List<string>();
        }

        return input
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
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
