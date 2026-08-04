using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

    /// <summary>Defines the public member type.</summary>
public partial class SocialMetadataForm
{
    /// <summary>Performs the SeoMetadata operation.</summary>
    [Parameter]
    public SeoMetadata SeoMetadata { get; set; } = SeoMetadata.Create("", "");

    /// <summary>Gets or sets the SeoMetadataChanged value.</summary>
    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    /// <summary>Performs the OnParametersSet operation.</summary>
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
