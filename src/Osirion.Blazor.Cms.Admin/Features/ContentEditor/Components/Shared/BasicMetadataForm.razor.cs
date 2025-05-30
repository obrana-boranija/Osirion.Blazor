using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class BasicMetadataForm
{
    [Parameter]
    public FrontMatter Metadata { get; set; } = FrontMatter.Create("New Post");

    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    private bool ImageLoadError { get; set; } = false;

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
            NotifyMetadataChanged().ConfigureAwait(false);
        }
    }

    protected override void OnParametersSet()
    {
        Metadata ??= FrontMatter.Create("New Post");
        ImageLoadError = false;
    }

    private async Task NotifyMetadataChanged()
    {
        if (MetadataChanged.HasDelegate)
        {
            await MetadataChanged.InvokeAsync(Metadata);
        }
    }

    private async Task OnCategoriesChanged(List<string> categories)
    {
        Metadata = Metadata.WithCategories(categories);
        await NotifyMetadataChanged();
    }

    private async Task OnTagsChanged(List<string> tags)
    {
        Metadata = Metadata.WithTags(tags);
        await NotifyMetadataChanged();
    }
}