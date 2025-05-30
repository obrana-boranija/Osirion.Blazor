using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class SocialMetadataForm
{
    [Parameter]
    public SeoMetadata SeoMetadata { get; set; } = SeoMetadata.Create("", "");

    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    protected override void OnParametersSet()
    {
        SeoMetadata ??= SeoMetadata.Create("", "");
    }

    private async Task NotifySeoMetadataChanged()
    {
        if (SeoMetadataChanged.HasDelegate)
        {
            await SeoMetadataChanged.InvokeAsync(SeoMetadata);
        }
    }
}