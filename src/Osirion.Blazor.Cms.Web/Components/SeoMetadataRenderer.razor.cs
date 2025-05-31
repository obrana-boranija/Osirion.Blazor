using Microsoft.AspNetCore.Components;
using Octokit;
using Osirion.Blazor.Cms.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Components;

public partial class SeoMetadataRenderer(NavigationManager navigationManager)
{
    private string _siteName = string.Empty;
    private string _baseUrl = string.Empty;
    private string _currentUrl = string.Empty;
    private string _metaTitle = string.Empty;
    private string? _metaDescription = string.Empty;
    private string? _ogTitle = string.Empty;
    private string? _ogDescription = string.Empty;
    private string? _ogImage = string.Empty;
    private string? _twitterTitle = string.Empty;
    private string? _twitterDescription = string.Empty;
    private string? _twitterImage = string.Empty;
    private string? _canonicalUrl = string.Empty;
    private string? _jsonLdContent = string.Empty;
    private string _schemaType = string.Empty;

    [Parameter, EditorRequired]
    public ContentItem? Content { get; set; }

    [Parameter]
    public string? SiteNameOverride { get; set; }

    [Parameter]
    public string? TwitterSite { get; set; }

    [Parameter]
    public string? TwitterCreator { get; set; }

    [Parameter]
    public SchemaType? SchemaType { get; set; }

    [Parameter]
    public bool AllowAiDiscovery { get; set; } = true;

    protected override void OnInitialized()
    {
        _siteName = SiteNameOverride ?? ExtractSiteName(navigationManager.BaseUri);
        _baseUrl = navigationManager.BaseUri.TrimEnd('/');
        _currentUrl = navigationManager.Uri;

        _metaTitle = BuildMetaTitle();
        _metaDescription = GetFirstValue(Content?.Metadata.SeoProperties?.Description, Content?.Description);
        _ogTitle = GetFirstValue(Content?.Metadata.SeoProperties?.OgTitle, _metaTitle);
        _ogDescription = GetFirstValue(Content?.Metadata.SeoProperties?.OgDescription, _metaDescription);
        _ogImage = NormalizeImageUrl(GetFirstValue(Content?.Metadata.SeoProperties?.OgImageUrl, Content?.FeaturedImageUrl));
        _twitterTitle = GetFirstValue(Content?.Metadata.SeoProperties?.TwitterTitle, _ogTitle);
        _twitterDescription = GetFirstValue(Content?.Metadata.SeoProperties?.TwitterDescription, _ogDescription);
        _twitterImage = NormalizeImageUrl(GetFirstValue(Content?.Metadata.SeoProperties?.TwitterImageUrl, _ogImage));
        _canonicalUrl = BuildCanonicalUrl();
        _schemaType = GetFirstValue(SchemaType?.ToString(), Content?.Metadata.SeoProperties?.Type) ?? "WebPage";
        _jsonLdContent = GenerateJsonLd();

        base.OnInitialized();
    }

    private string RenderMetaTag(string name, string? content)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        return $"<meta name=\"{name}\" content=\"{content}\" />";
    }

    private string RenderMetaTag(string name, DateTime content)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        return $"<meta name=\"{name}\" content=\"{content.ToString("yyyy-MM-dd")}\" />";
    }

    private string RenderMetaTag(string name, DateTime? content)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        if (!content.HasValue)
        {
            return string.Empty;
        }

        return $"<meta name=\"{name}\" content=\"{content.Value.ToString("yyyy-MM-dd")}\" />";
    }

    private string RenderMetaTag(string name, IReadOnlyList<string> content)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }
        if (content is null || content.Any())
        {
            return string.Empty;
        }

        return $"<meta name=\"{name}\" content=\"{string.Join(", ", content)}\" />";
    }

    private string BuildMetaTitle()
    {
        var title = GetFirstValue(Content?.Metadata.SeoProperties?.Title, Content?.Title);
        return string.IsNullOrWhiteSpace(title) ? _siteName : $"{title} | {_siteName}";
    }

    private string? BuildCanonicalUrl()
    {
        if (!string.IsNullOrWhiteSpace(Content?.Metadata.SeoProperties?.Canonical))
            return Content.Metadata.SeoProperties.Canonical;

        return Content?.Path is not null ? _currentUrl : null;
    }

    private string? NormalizeImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (url.StartsWith("http://") || url.StartsWith("https://"))
            return url;

        return $"{_baseUrl}/{url.TrimStart('/')}";
    }

    private static string? GetFirstValue(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }

    private static string ExtractSiteName(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var uri = new Uri(url);
        return uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            ? uri.Host[4..]
            : uri.Host;
    }

    private string? GenerateJsonLd()
    {
        if (Content is null)
            return null;

        if (!string.IsNullOrWhiteSpace(Content.Metadata.SeoProperties.JsonLd))
            return Content.Metadata.SeoProperties.JsonLd;

        var jsonLd = _schemaType switch
        {
            "Article" => GenerateArticleSchema(),
            "BlogPosting" => GenerateBlogPostingSchema(),
            "WebPage" => GenerateWebPageSchema(),
            _ => GenerateDefaultSchema()
        };

        return JsonSerializer.Serialize(jsonLd, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private object GenerateArticleSchema()
    {
        return new
        {
            @context = "https://schema.org",
            @type = "Article",
            headline = Content!.Title,
            description = Content.Description,
            image = NormalizeImageUrl(Content.FeaturedImageUrl),
            author = new
            {
                @type = "Person",
                name = Content.Author
            },
            publisher = new
            {
                @type = "Organization",
                name = _siteName,
                logo = new
                {
                    @type = "ImageObject",
                    url = $"{_baseUrl}/logo.png"
                }
            },
            datePublished = Content.PublishDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            dateModified = Content.LastModified?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            mainEntityOfPage = new
            {
                @type = "WebPage",
                @id = _currentUrl
            },
            keywords = Content.Tags.Any() ? string.Join(", ", Content.Tags) : null,
            articleSection = Content.Categories.FirstOrDefault(),
            wordCount = Content.Content?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            url = _currentUrl
        };
    }

    private object GenerateBlogPostingSchema()
    {
        return GenerateArticleSchema();
        //var article = GenerateArticleSchema();
        //return new { @context = "https://schema.org", @type = "BlogPosting" }
        //    .GetType()
        //    .GetProperties()
        //    .Concat(article.GetType().GetProperties())
        //    .ToDictionary(p => p.Name, p => p.GetValue(article));
    }

    private object GenerateWebPageSchema()
    {
        return new
        {
            @context = "https://schema.org",
            @type = "WebPage",
            name = Content!.Title,
            description = Content.Description,
            url = _currentUrl,
            datePublished = Content.PublishDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            dateModified = Content.LastModified?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            author = new
            {
                @type = "Person",
                name = Content.Author
            },
            publisher = new
            {
                @type = "Organization",
                name = _siteName
            }
        };
    }

    private object GenerateDefaultSchema()
    {
        return new
        {
            @context = "https://schema.org",
            @type = Content?.Metadata?.SeoProperties.Type,
            name = Content?.Title,
            description = Content?.Description,
            url = _currentUrl
        };
    }
}

public enum SchemaType
{
    Article,
    BlogPosting,
    WebPage,
}