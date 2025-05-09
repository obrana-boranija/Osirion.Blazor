using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class MetadataEditor
{
    [Parameter]
    public FrontMatter Metadata { get; set; } = FrontMatter.Create("New Post");

    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    [Parameter]
    public bool ShowPreview { get; set; } = true;

    [Parameter]
    public bool ShowActions { get; set; } = true;

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private string CategoriesInput
    {
        get => string.Join(", ", Metadata.Categories);
        set
        {
            var updatedMetadata = Metadata.WithCategories(ParseList(value));
            Metadata = updatedMetadata;
            NotifyMetadataChanged();
        }
    }

    private string TagsInput
    {
        get => string.Join(", ", Metadata.Tags);
        set
        {
            var updatedMetadata = Metadata.WithTags(ParseList(value));
            Metadata = updatedMetadata;
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
            Metadata = Metadata.WithDate(value);
            NotifyMetadataChanged();
        }
    }

    protected override void OnParametersSet()
    {
        // Initialize with default empty front matter if null
        Metadata ??= FrontMatter.Create("New Post");
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

    private async Task RefreshMetadata()
    {
        if (OnRefresh.HasDelegate)
        {
            await OnRefresh.InvokeAsync();
        }
    }
}