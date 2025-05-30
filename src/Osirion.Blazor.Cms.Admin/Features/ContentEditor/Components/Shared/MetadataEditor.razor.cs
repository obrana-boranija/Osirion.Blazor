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
    public SeoMetadata SeoData { get; set; } = SeoMetadata.Create("", "");

    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    [Parameter]
    public bool ShowActions { get; set; } = true;

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private MetadataSection ActiveSection { get; set; } = MetadataSection.Basic;
    private bool ShowMobilePreview { get; set; } = false;

    protected override void OnParametersSet()
    {
        // Initialize with default empty objects if null
        Metadata ??= FrontMatter.Create("New Post");
        SeoData ??= SeoMetadata.Create("", "");

        // Auto-populate SEO from FrontMatter if SEO is empty
        if (string.IsNullOrEmpty(SeoData.MetaTitle) && !string.IsNullOrEmpty(Metadata.Title))
        {
            SeoData = SeoData
                .WithMetaTitle(Metadata.Title)
                .WithMetaDescription(Metadata.Description)
                .WithOpenGraph(Metadata.Title, Metadata.Description, Metadata.FeaturedImage ?? "")
                .WithTwitterCard(Metadata.Title, Metadata.Description, Metadata.FeaturedImage ?? "");
        }
    }

    private void SetActiveSection(MetadataSection section)
    {
        ActiveSection = section;
    }

    private void ToggleMobilePreview()
    {
        ShowMobilePreview = !ShowMobilePreview;
    }

    private async Task OnMetadataChanged(FrontMatter newMetadata)
    {
        Metadata = newMetadata;

        // Auto-sync to SEO if titles match
        if (SeoData.MetaTitle == Metadata.Title || string.IsNullOrEmpty(SeoData.MetaTitle))
        {
            SeoData = SeoData
                .WithMetaTitle(newMetadata.Title)
                .WithMetaDescription(newMetadata.Description);
        }

        if (MetadataChanged.HasDelegate)
        {
            await MetadataChanged.InvokeAsync(Metadata);
        }
    }

    private async Task OnSeoMetadataChanged(SeoMetadata newSeoData)
    {
        SeoData = newSeoData;

        if (SeoMetadataChanged.HasDelegate)
        {
            await SeoMetadataChanged.InvokeAsync(SeoData);
        }
    }

    private async Task RefreshMetadata()
    {
        if (OnRefresh.HasDelegate)
        {
            await OnRefresh.InvokeAsync();
        }
    }

    public enum MetadataSection
    {
        Basic,
        Seo,
        Social,
        Advanced
    }
}