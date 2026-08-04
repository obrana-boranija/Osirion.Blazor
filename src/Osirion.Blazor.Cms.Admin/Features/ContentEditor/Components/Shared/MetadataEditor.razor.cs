using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

/// <summary>Edits content front matter and SEO metadata.</summary>
public partial class MetadataEditor
{
    /// <summary>Gets or sets the content front matter.</summary>
    [Parameter]
    public FrontMatter Metadata { get; set; } = FrontMatter.Create("New Post");

    /// <summary>Gets or sets the callback invoked when front matter changes.</summary>
    [Parameter]
    public EventCallback<FrontMatter> MetadataChanged { get; set; }

    /// <summary>Gets or sets the SEO metadata.</summary>
    [Parameter]
    public SeoMetadata SeoData { get; set; } = SeoMetadata.Create("", "");

    /// <summary>Gets or sets the callback invoked when SEO metadata changes.</summary>
    [Parameter]
    public EventCallback<SeoMetadata> SeoMetadataChanged { get; set; }

    /// <summary>Gets or sets a value indicating whether action controls are shown.</summary>
    [Parameter]
    public bool ShowActions { get; set; } = true;

    /// <summary>Gets or sets the callback invoked to refresh metadata.</summary>
    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private MetadataSection ActiveSection { get; set; } = MetadataSection.Basic;
    private bool ShowMobilePreview { get; set; } = false;

    /// <summary>Performs the OnParametersSet operation.</summary>
    protected override void OnParametersSet()
    {
        // Initialize with default empty objects if null
        Metadata ??= FrontMatter.Create("New Post");
        SeoData ??= SeoMetadata.Create("", "");

        // Auto-populate SEO from FrontMatter if SEO is empty
        if (string.IsNullOrWhiteSpace(SeoData.Title) && !string.IsNullOrWhiteSpace(Metadata.Title))
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
        if (SeoData.Title == Metadata.Title || string.IsNullOrWhiteSpace(SeoData.Title))
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

    /// <summary>Identifies a metadata editor section.</summary>
    public enum MetadataSection
    {
        /// <summary>The basic metadata section.</summary>
        Basic,
        /// <summary>The SEO metadata section.</summary>
        Seo,
        /// <summary>The social metadata section.</summary>
        Social,
        /// <summary>The advanced metadata section.</summary>
        Advanced
    }
}
