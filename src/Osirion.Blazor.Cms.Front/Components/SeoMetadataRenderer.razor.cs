using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Components;
public partial class SeoMetadataRenderer
{
    [Parameter]
    public ContentItem? Content { get; set; }

    [Parameter]
    public string? SiteName { get; set; }

    [Parameter]
    public string? BaseUrl { get; set; }

    private string GetMetaTitle()
    {
        if (!string.IsNullOrEmpty(Content?.Seo.MetaTitle))
            return Content.Seo.MetaTitle;

        if (!string.IsNullOrEmpty(Content?.Title))
            return Content.Title;

        return "No Title";
    }

    private string GenerateJsonLd()
    {
        if (Content == null)
            return "{}";

        var json = new System.Text.StringBuilder();
        json.Append("{\n");
        json.Append($"  \"@context\": \"https://schema.org\",\n");
        json.Append($"  \"@type\": \"{Content.Seo.SchemaType}\",\n");
        json.Append($"  \"headline\": \"{JsonEscape(Content.Title)}\",\n");

        if (!string.IsNullOrEmpty(Content.Description))
            json.Append($"  \"description\": \"{JsonEscape(Content.Description)}\",\n");

        if (!string.IsNullOrEmpty(Content.Author))
            json.Append($"  \"author\": {{ \"@type\": \"Person\", \"name\": \"{JsonEscape(Content.Author)}\" }},\n");

        json.Append($"  \"datePublished\": \"{Content.DateCreated:yyyy-MM-dd}\",\n");

        if (Content.LastModified.HasValue)
            json.Append($"  \"dateModified\": \"{Content.LastModified.Value:yyyy-MM-dd}\",\n");

        if (!string.IsNullOrEmpty(Content.FeaturedImageUrl))
            json.Append($"  \"image\": \"{EnsureAbsoluteUrl(Content.FeaturedImageUrl)}\",\n");

        if (!string.IsNullOrEmpty(SiteName))
            json.Append($"  \"publisher\": {{ \"@type\": \"Organization\", \"name\": \"{JsonEscape(SiteName)}\" }},\n");

        if (!string.IsNullOrEmpty(BaseUrl))
            json.Append($"  \"url\": \"{EnsureAbsoluteUrl(Content.Path)}\"\n");
        else
            json.Append($"  \"url\": \"/{Content.Path}\"\n");

        json.Append("}");
        return json.ToString();
    }

    private string JsonEscape(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private string EnsureAbsoluteUrl(string url)
    {
        if (url.StartsWith("http://") || url.StartsWith("https://"))
            return url;

        if (string.IsNullOrEmpty(BaseUrl))
            return url.StartsWith("/") ? url : $"/{url}";

        var baseUrlWithoutTrailingSlash = BaseUrl.TrimEnd('/');
        var urlWithoutLeadingSlash = url.TrimStart('/');
        return $"{baseUrlWithoutTrailingSlash}/{urlWithoutLeadingSlash}";
    }
}