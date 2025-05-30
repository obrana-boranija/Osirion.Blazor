using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using System.Text;

namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;

public partial class MetadataPreview
{
    [Parameter]
    public FrontMatter Metadata { get; set; } = FrontMatter.Create("New Post");

    [Parameter]
    public SeoMetadata SeoMetadata { get; set; } = SeoMetadata.Create("", "");

    [Parameter]
    public bool ShowActions { get; set; } = true;

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    private string BaseUrl { get; set; } = "https://example.com";

    protected override void OnParametersSet()
    {
        Metadata ??= FrontMatter.Create("New Post");
        SeoMetadata ??= SeoMetadata.Create("", "");
    }

    private async Task RefreshPreview()
    {
        if (OnRefresh.HasDelegate)
        {
            await OnRefresh.InvokeAsync();
        }
    }

    private string GetPreviewUrl()
    {
        if (!string.IsNullOrEmpty(SeoMetadata.CanonicalUrl))
        {
            return SeoMetadata.CanonicalUrl;
        }

        var slug = !string.IsNullOrEmpty(Metadata.Slug)
            ? Metadata.Slug
            : Metadata.Title.ToLower().Replace(" ", "-");

        return $"{BaseUrl}/{slug}";
    }

    private string GetDomain()
    {
        var url = GetPreviewUrl();
        try
        {
            var uri = new Uri(url);
            return uri.Host;
        }
        catch
        {
            return "example.com";
        }
    }

    private string GetSearchTitle()
    {
        var title = !string.IsNullOrEmpty(SeoMetadata.MetaTitle)
            ? SeoMetadata.MetaTitle
            : Metadata.Title;

        // Google typically truncates at ~60 characters
        if (title.Length > 60)
        {
            return title.Substring(0, 57) + "...";
        }

        return title;
    }

    private string GetSearchDescription()
    {
        var description = !string.IsNullOrEmpty(SeoMetadata.MetaDescription)
            ? SeoMetadata.MetaDescription
            : Metadata.Description;

        // Google typically truncates at ~160 characters
        if (description.Length > 160)
        {
            return description.Substring(0, 157) + "...";
        }

        return description;
    }

    private string GetOgTitle()
    {
        return !string.IsNullOrEmpty(SeoMetadata.OgTitle)
            ? SeoMetadata.OgTitle
            : GetSearchTitle();
    }

    private string GetOgDescription()
    {
        return !string.IsNullOrEmpty(SeoMetadata.OgDescription)
            ? SeoMetadata.OgDescription
            : GetSearchDescription();
    }

    private string GetOgImage()
    {
        return !string.IsNullOrEmpty(SeoMetadata.OgImageUrl)
            ? SeoMetadata.OgImageUrl
            : Metadata.FeaturedImage ?? "";
    }

    private string GetTwitterTitle()
    {
        return !string.IsNullOrEmpty(SeoMetadata.TwitterTitle)
            ? SeoMetadata.TwitterTitle
            : GetOgTitle();
    }

    private string GetTwitterDescription()
    {
        return !string.IsNullOrEmpty(SeoMetadata.TwitterDescription)
            ? SeoMetadata.TwitterDescription
            : GetOgDescription();
    }

    private string GetTwitterImage()
    {
        return !string.IsNullOrEmpty(SeoMetadata.TwitterImageUrl)
            ? SeoMetadata.TwitterImageUrl
            : GetOgImage();
    }

    private string GetFormattedFrontMatter()
    {
        return Metadata.ToYaml();
    }

    private string GetMetaTags()
    {
        var sb = new StringBuilder();

        // Basic meta tags
        sb.AppendLine($"<title>{GetSearchTitle()}</title>");
        sb.AppendLine($"<meta name=\"description\" content=\"{GetSearchDescription()}\" />");

        if (!string.IsNullOrEmpty(SeoMetadata.CanonicalUrl))
        {
            sb.AppendLine($"<link rel=\"canonical\" href=\"{SeoMetadata.CanonicalUrl}\" />");
        }

        sb.AppendLine($"<meta name=\"robots\" content=\"{SeoMetadata.Robots}\" />");

        // Open Graph tags
        sb.AppendLine($"<meta property=\"og:title\" content=\"{GetOgTitle()}\" />");
        sb.AppendLine($"<meta property=\"og:description\" content=\"{GetOgDescription()}\" />");
        sb.AppendLine($"<meta property=\"og:type\" content=\"{SeoMetadata.OgType}\" />");
        sb.AppendLine($"<meta property=\"og:url\" content=\"{GetPreviewUrl()}\" />");

        if (!string.IsNullOrEmpty(GetOgImage()))
        {
            sb.AppendLine($"<meta property=\"og:image\" content=\"{GetOgImage()}\" />");
        }

        // Twitter Card tags
        sb.AppendLine($"<meta name=\"twitter:card\" content=\"{SeoMetadata.TwitterCard}\" />");
        sb.AppendLine($"<meta name=\"twitter:title\" content=\"{GetTwitterTitle()}\" />");
        sb.AppendLine($"<meta name=\"twitter:description\" content=\"{GetTwitterDescription()}\" />");

        if (!string.IsNullOrEmpty(GetTwitterImage()))
        {
            sb.AppendLine($"<meta name=\"twitter:image\" content=\"{GetTwitterImage()}\" />");
        }

        // Schema.org structured data
        if (!string.IsNullOrEmpty(SeoMetadata.JsonLd))
        {
            sb.AppendLine($"<script type=\"application/ld+json\">");
            sb.AppendLine(SeoMetadata.JsonLd);
            sb.AppendLine("</script>");
        }

        return sb.ToString();
    }
}