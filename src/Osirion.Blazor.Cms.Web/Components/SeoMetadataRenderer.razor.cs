using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Web.Components.Seo;
using Osirion.Blazor.Cms.Web.Options;
using Osirion.Blazor.Core.Utils;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Advanced SEO metadata renderer supporting traditional SEO, GEO (Generative Engine Optimization), and AEO (Answer Engine Optimization).
/// Implements comprehensive Schema.org structured data, Open Graph, Twitter Cards, and AI discovery meta tags.
/// </summary>
public partial class SeoMetadataRenderer(NavigationManager navigationManager)
{
    /// <summary>
    /// Injected SEO metadata options providing site-wide defaults
    /// </summary>
    [Inject]
    private IOptions<SeoMetadataOptions>? SeoOptions { get; set; }

    [Inject]
    private ILogger<SeoMetadataRenderer> Logger { get; set; } = default!;
    // Core properties
    private string _siteName = string.Empty;
    private string _baseUrl = string.Empty;
    private string _currentUrl = string.Empty;
    private string _locale = "en_US";
    
    // Meta tags
    private string _metaTitle = string.Empty;
    private string? _metaDescription;
    private string? _metaKeywords;
    
    // Open Graph
    private string? _ogTitle;
    private string? _ogDescription;
    private string? _ogImage;
    private string? _ogType;
    private int? _ogImageWidth;
    private int? _ogImageHeight;
    private string? _ogImageAlt;
    
    // Twitter Card
    private string? _twitterTitle;
    private string? _twitterDescription;
    private string? _twitterImage;
    private string? _twitterCard;
    
    // SEO essentials
    private string? _canonicalUrl;
    private string? _robotsDirective;
    private string? _languageCode;
    
    // Structured data
    private string? _jsonLdContent;
    private List<string>? _additionalJsonLdSchemas;
    
    // GEO & AEO optimization
    private string? _entitySummary;
    private List<string>? _answersTo;
    private Dictionary<string, string>? _structuredAnswers;

    // Helper properties for defensive programming (Content or ContentItems scenarios)
    private bool _hasContent;
    private string? _author;
    private bool _isArticle;
    private DateTime? _publishDate;
    private DateTime? _modifiedDate;
    private int? _readTimeMinutes;
    private IReadOnlyList<string>? _tags;
    private IReadOnlyList<string>? _categories;

    #region Parameters

    /// <summary>
    /// The content item to render metadata for (required for single content pages).
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Collection of content items for collection pages (Blog, Category, Tag pages).
    /// Used to generate CollectionPage schema with itemListElement.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? ContentItems { get; set; }

    /// <summary>
    /// Title for the collection page (used when ContentItems is provided).
    /// </summary>
    [Parameter]
    public string? CollectionTitle { get; set; }

    /// <summary>
    /// Description for the collection page (used when ContentItems is provided).
    /// </summary>
    [Parameter]
    public string? CollectionDescription { get; set; }

    /// <summary>
    /// Override the default site name extracted from the domain.
    /// </summary>
    [Parameter]
    public string? SiteNameOverride { get; set; }

    /// <summary>
    /// Site-wide description used for organization schema.
    /// </summary>
    [Parameter]
    public string? SiteDescription { get; set; }

    /// <summary>
    /// URL to the site logo (recommended: square format, min 112x112px).
    /// </summary>
    [Parameter]
    public string? SiteLogoUrl { get; set; }

    /// <summary>
    /// Twitter handle for the site (e.g., "@yoursite").
    /// </summary>
    [Parameter]
    public string? TwitterSite { get; set; }

    /// <summary>
    /// Twitter handle for the content creator (e.g., "@author").
    /// </summary>
    [Parameter]
    public string? TwitterCreator { get; set; }

    /// <summary>
    /// Facebook App ID for Facebook Insights.
    /// </summary>
    [Parameter]
    public string? FacebookAppId { get; set; }

    /// <summary>
    /// Schema types to generate for structured data. Multiple schemas can be specified.
    /// Example: new[] { SchemaType.BlogPosting, SchemaType.BreadcrumbList, SchemaType.FAQ }
    /// </summary>
    [Parameter]
    public SchemaType[]? SchemaTypes { get; set; }

    /// <summary>
    /// Allow AI systems to discover and index this content.
    /// </summary>
    [Parameter]
    public bool AllowAiDiscovery { get; set; } = true;

    /// <summary>
    /// Allow AI systems to use this content for training purposes.
    /// </summary>
    [Parameter]
    public bool AllowAiTraining { get; set; } = true;

    /// <summary>
    /// Allow traditional search engines to index this content.
    /// </summary>
    [Parameter]
    public bool AllowSearchIndexing { get; set; } = true;

    /// <summary>
    /// Enable Generative Engine Optimization (GEO) meta tags.
    /// </summary>
    [Parameter]
    public bool EnableGeoOptimization { get; set; } = true;

    /// <summary>
    /// Enable Answer Engine Optimization (AEO) structured answers.
    /// </summary>
    [Parameter]
    public bool EnableAeoOptimization { get; set; } = true;

    /// <summary>
    /// Generate multiple schema types when applicable (e.g., Article + BreadcrumbList).
    /// </summary>
    [Parameter]
    public bool GenerateMultipleSchemas { get; set; } = true;

    /// <summary>
    /// Default fallback image URL when content has no featured image.
    /// </summary>
    [Parameter]
    public string? DefaultImageUrl { get; set; }

    /// <summary>
    /// Default image width for Open Graph (recommended: 1200px).
    /// </summary>
    [Parameter]
    public int DefaultImageWidth { get; set; } = 1200;

    /// <summary>
    /// Default image height for Open Graph (recommended: 630px).
    /// </summary>
    [Parameter]
    public int DefaultImageHeight { get; set; } = 630;

    /// <summary>
    /// Organization name for structured data.
    /// </summary>
    [Parameter]
    public string? OrganizationName { get; set; }

    /// <summary>
    /// Alternate language versions of this page for hreflang tags.
    /// </summary>
    [Parameter]
    public List<string>? AlternateLanguageUrls { get; set; }

    /// <summary>
    /// URL to the previous page in a series or pagination.
    /// </summary>
    [Parameter]
    public string? PrevPageUrl { get; set; }

    /// <summary>
    /// URL to the next page in a series or pagination.
    /// </summary>
    [Parameter]
    public string? NextPageUrl { get; set; }

    /// <summary>
    /// Text label for the home breadcrumb (default: "Home").
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Custom breadcrumb items to override automatic generation from path.
    /// Format: List of tuples (name, url) or use automatic path parsing if null.
    /// </summary>
    [Parameter]
    public List<(string Name, string Url)>? CustomBreadcrumbs { get; set; }

    /// <summary>
    /// Query parameters kept in the canonical URL, og:url and structured data URLs (for example <c>page</c> for
    /// paginated lists, where each page is its own document). All other parameters and the fragment are dropped.
    /// A canonical URL declared in the content front matter always wins.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<string>? CanonicalQueryParameters { get; set; }

    /// <summary>
    /// Emits the viewport, theme-color and mobile web app meta tags. Off by default: the host document owns
    /// these, and emitting them again duplicates its tags.
    /// </summary>
    [Parameter]
    public bool EmitMobileMetaTags { get; set; }

    /// <summary>
    /// Generates FAQ structured data and answer meta tags from content titles and descriptions
    /// (requires <see cref="EnableAeoOptimization"/>). Off by default because the questions are derived, not authored.
    /// </summary>
    [Parameter]
    public bool EnableGeneratedFaq { get; set; }

    #endregion

    /// <summary>
    /// Alternate language versions as <c>(hreflang, href)</c> pairs parsed from <see cref="AlternateLanguageUrls"/>.
    /// </summary>
    private List<(string HrefLang, string Href)> _alternateLinks = [];

    private static readonly Dictionary<string, string> OgTypesBySchemaType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Article"] = "article",
        ["BlogPosting"] = "article",
        ["Product"] = "product",
        ["VideoObject"] = "video.other"
    };

    /// <summary>
    /// Recomputes all SEO metadata whenever the parameters are set, so a reused component instance never
    /// shows the title, canonical URL or structured data of the previous page.
    /// </summary>
    protected override void OnParametersSet()
    {
        ResetState();

        // Require either Content or ContentItems
        if (Content is null && (ContentItems is null || !ContentItems.Any())) return;

        // Apply defaults from options if available
        ApplyDefaultsFromOptions();

        // Initialize helper properties based on whether we have single Content or ContentItems
        InitializeHelperProperties();

        // Core setup
        _siteName = SiteNameOverride ?? OrganizationName ?? SeoText.ExtractSiteName(navigationManager.BaseUri);
        _baseUrl = navigationManager.BaseUri.TrimEnd('/');
        _currentUrl = ResolvePageUrl();
        _locale = SeoText.ToOpenGraphLocale(Content?.Locale);
        _languageCode = Content?.Locale ?? "en";
        _alternateLinks = ParseAlternateLanguageUrls();

        // Build all metadata
        BuildMetadata();
        BuildOpenGraphMetadata();
        _isArticle = string.Equals(_ogType, "article", StringComparison.OrdinalIgnoreCase);
        BuildTwitterCardMetadata();
        BuildSeoEssentials();

        // GEO & AEO optimization (before the structured data, which reuses the generated answers)
        if (EnableGeoOptimization)
        {
            BuildGeoOptimization();
        }

        if (EnableAeoOptimization && EnableGeneratedFaq)
        {
            BuildAeoOptimization();
        }

        // Generate structured data
        GenerateAllJsonLdSchemas();
        AddGeneratedFaqSchema();
    }

    private void ResetState()
    {
        _hasContent = false;
        _metaTitle = string.Empty;
        _metaDescription = null;
        _metaKeywords = null;
        _ogTitle = null;
        _ogDescription = null;
        _ogImage = null;
        _ogType = null;
        _ogImageWidth = null;
        _ogImageHeight = null;
        _ogImageAlt = null;
        _twitterTitle = null;
        _twitterDescription = null;
        _twitterImage = null;
        _twitterCard = null;
        _canonicalUrl = null;
        _currentUrl = string.Empty;
        _robotsDirective = null;
        _languageCode = null;
        _jsonLdContent = null;
        _additionalJsonLdSchemas = null;
        _entitySummary = null;
        _answersTo = null;
        _structuredAnswers = null;
        _author = null;
        _isArticle = false;
        _publishDate = null;
        _modifiedDate = null;
        _readTimeMinutes = null;
        _tags = null;
        _categories = null;
        _alternateLinks = [];
    }

    /// <summary>
    /// The URL this page declares for itself: the current address without its fragment and without query
    /// parameters, except those named in <see cref="CanonicalQueryParameters"/>. Tracking parameters such as
    /// <c>utm_source</c> therefore never produce a second canonical URL.
    /// </summary>
    private string ResolvePageUrl() =>
        ContentUrls.StripQueryAndFragment(navigationManager.Uri, CanonicalQueryParameters);

    private List<(string HrefLang, string Href)> ParseAlternateLanguageUrls()
    {
        var links = new List<(string, string)>();
        foreach (var entry in AlternateLanguageUrls ?? [])
        {
            var separator = entry?.IndexOf('|', StringComparison.Ordinal) ?? -1;
            if (separator <= 0 || separator == entry!.Length - 1)
            {
                Logger.LogWarning("Skipping malformed alternate language entry '{Entry}'; expected 'language|url'", entry);
                continue;
            }

            links.Add((entry[..separator].Trim(), entry[(separator + 1)..].Trim()));
        }

        return links;
    }

    /// <summary>
    /// Applies default values from injected SeoMetadataOptions if parameters are not explicitly set.
    /// </summary>
    private void ApplyDefaultsFromOptions()
    {
        var options = SeoOptions?.Value;
        if (options is null) return;

        ApplySiteDefaults(options);
        ApplyRenderingDefaults(options);
    }

    private void ApplySiteDefaults(SeoMetadataOptions options)
    {
        // Apply site-wide defaults only if parameters are not set
        SiteNameOverride ??= options.SiteName;
        SiteDescription ??= options.SiteDescription;
        SiteLogoUrl ??= options.SiteLogoUrl;
        TwitterSite ??= options.TwitterSite;
        TwitterCreator ??= options.TwitterCreator;
        FacebookAppId ??= options.FacebookAppId;
        DefaultImageUrl ??= options.DefaultImageUrl;
        OrganizationName ??= options.OrganizationName;
    }

    private void ApplyRenderingDefaults(SeoMetadataOptions options)
    {
        BreadcrumbHomeText = string.IsNullOrWhiteSpace(BreadcrumbHomeText) ? options.BreadcrumbHomeText : BreadcrumbHomeText;

        // Apply default settings if not explicitly overridden
        if (DefaultImageWidth == 1200) DefaultImageWidth = options.DefaultImageWidth;
        if (DefaultImageHeight == 630) DefaultImageHeight = options.DefaultImageHeight;

        EmitMobileMetaTags |= options.EmitMobileMetaTags;
        EnableGeneratedFaq |= options.EnableGeneratedFaq;

        // Apply schema types if not set
        SchemaTypes ??= options.DefaultSchemaTypes;
    }

    /// <summary>
    /// Initializes helper properties based on whether we have single Content or ContentItems.
    /// Provides defensive programming to avoid null reference exceptions in markup.
    /// </summary>
    private void InitializeHelperProperties()
    {
        _hasContent = Content is not null || (ContentItems?.Any() == true);

        if (Content is not null)
        {
            // Single content page
            _author = Content.Author;
            _publishDate = Content.PublishDate;
            _modifiedDate = Content.LastModified;
            _readTimeMinutes = Content.ReadTimeMinutes;
            _tags = Content.Tags;
            _categories = Content.Categories;
        }
        else if (ContentItems?.Any() == true)
        {
            // Collection page - extract from first item or use defaults
            var firstItem = ContentItems.First();
            _author = firstItem.Author;
            _publishDate = firstItem.PublishDate;
            _modifiedDate = ContentItems.Max(c => c.LastModified);
            _readTimeMinutes = null; // Collections don't have read time
            _tags = ContentItems.SelectMany(c => c.Tags).Distinct().ToList();
            _categories = ContentItems.SelectMany(c => c.Categories).Distinct().ToList();
        }
    }

    #region Metadata Builders

    private void BuildMetadata()
    {
        _metaTitle = BuildMetaTitle();

        if (ContentItems?.Any() == true)
        {
            // Collection page metadata
            _metaDescription = SeoText.TruncateDescription(CollectionDescription, 160);
            // Aggregate all unique tags from collection
            var allTags = ContentItems.SelectMany(c => c.Tags).Distinct().ToList();
            _metaKeywords = allTags.Any() ? string.Join(", ", allTags) : null;
        }
        else
        {
            // Single content page metadata
            _metaDescription = SeoText.TruncateDescription(
                SeoText.FirstNonBlank(Content?.Metadata?.SeoProperties?.Description, Content?.Description),
                160
            );
            _metaKeywords = Content?.Tags.Any() == true ? string.Join(", ", Content.Tags) : null;
        }
    }

    private void BuildOpenGraphMetadata()
    {
        if (ContentItems?.Any() == true)
        {
            BuildCollectionOpenGraphMetadata(ContentItems);
        }
        else
        {
            BuildContentOpenGraphMetadata();
        }

        _ogImageWidth = DefaultImageWidth;
        _ogImageHeight = DefaultImageHeight;
    }

    private void BuildCollectionOpenGraphMetadata(IReadOnlyList<ContentItem> contentItems)
    {
        _ogTitle = SeoText.TruncateTitle(CollectionTitle ?? _metaTitle, 60);
        _ogDescription = SeoText.TruncateDescription(CollectionDescription ?? _metaDescription, 200);
        _ogImage = NormalizeImageUrl(contentItems.FirstOrDefault()?.FeaturedImageUrl ?? DefaultImageUrl);
        _ogType = "website";
        _ogImageAlt = CollectionTitle ?? _siteName;
    }

    private void BuildContentOpenGraphMetadata()
    {
        var seo = Content?.Metadata?.SeoProperties;
        _ogTitle = SeoText.TruncateTitle(SeoText.FirstNonBlank(seo?.OgTitle, Content?.Title, _metaTitle), 60);
        _ogDescription = SeoText.TruncateDescription(SeoText.FirstNonBlank(seo?.OgDescription, _metaDescription), 200);
        _ogImage = NormalizeImageUrl(SeoText.FirstNonBlank(seo?.OgImageUrl, Content?.FeaturedImageUrl, DefaultImageUrl));
        _ogType = SeoText.FirstNonBlank(seo?.OgType, DetermineOgType());
        _ogImageAlt = SeoText.FirstNonBlank(Content?.Title, _siteName);
    }

    private void BuildTwitterCardMetadata()
    {
        if (ContentItems?.Any() == true)
        {
            // Collection page Twitter Card metadata
            _twitterCard = "summary_large_image";
            _twitterTitle = SeoText.TruncateTitle(_ogTitle ?? _metaTitle, 70);
            _twitterDescription = SeoText.TruncateDescription(_metaDescription, 200);
            _twitterImage = NormalizeImageUrl(_ogImage);
            return;
        }

        var seo = Content?.Metadata?.SeoProperties;
        _twitterCard = SeoText.FirstNonBlank(seo?.TwitterCard, "summary_large_image");
        _twitterTitle = SeoText.TruncateTitle(SeoText.FirstNonBlank(seo?.TwitterTitle, _ogTitle, _metaTitle), 70);
        _twitterDescription = SeoText.TruncateDescription(SeoText.FirstNonBlank(seo?.TwitterDescription, _metaDescription), 200);
        _twitterImage = NormalizeImageUrl(SeoText.FirstNonBlank(seo?.TwitterImageUrl, _ogImage));
    }

    private void BuildSeoEssentials()
    {
        _canonicalUrl = BuildCanonicalUrl();
        _robotsDirective = BuildRobotsDirective();
    }

    private void BuildGeoOptimization()
    {
        // GEO: Create concise entity summary for AI engines
        var description = Content?.Description ?? CollectionDescription;

        if (!string.IsNullOrWhiteSpace(description))
        {
            _entitySummary = SeoText.TruncateDescription(description, 280);
        }
    }

    /// <summary>
    /// AEO: records the question the page answers, either the title itself when it is a question, or a question
    /// derived from the title pattern ("Guide to X" becomes "How to X?"), with the description as its answer.
    /// </summary>
    private void BuildAeoOptimization()
    {
        _answersTo = [];
        _structuredAnswers = [];

        var title = Content?.Title ?? CollectionTitle;
        if (string.IsNullOrWhiteSpace(title)) return;

        var question = AnswerQuestions.IsQuestion(title, _languageCode)
            ? title
            : AnswerQuestions.GenerateImplicitQuestion(title, _languageCode);
        if (string.IsNullOrWhiteSpace(question)) return;

        _answersTo.Add(question);

        var description = Content?.Description ?? CollectionDescription;
        if (!string.IsNullOrWhiteSpace(description))
        {
            _structuredAnswers[question] = SeoText.TruncateDescription(description, 320) ?? string.Empty;
        }
    }

    #endregion

    #region Rendering Methods

    private static string RenderMetaTag(string name, string? content)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // HTML encode content to prevent XSS
        var encodedContent = System.Net.WebUtility.HtmlEncode(content);
        return $"<meta name=\"{name}\" content=\"{encodedContent}\" />";
    }

    private static string RenderPropertyTag(string property, string? content)
    {
        if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var encodedContent = System.Net.WebUtility.HtmlEncode(content);
        return $"<meta property=\"{property}\" content=\"{encodedContent}\" />";
    }

    private static string RenderMetaTag(string name, DateTime content)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return $"<meta name=\"{name}\" content=\"{content:yyyy-MM-dd}\" />";
    }

    #endregion

    #region Title and URL Builders

    private string BuildMetaTitle()
    {
        if (ContentItems?.Any() == true && !string.IsNullOrWhiteSpace(CollectionTitle))
        {
            // Collection page title
            return $"{CollectionTitle} | {_siteName}";
        }

        var title = SeoText.FirstNonBlank(Content?.Metadata?.SeoProperties?.Title, Content?.Title);

        if (string.IsNullOrWhiteSpace(title))
            return _siteName;

        // Optimal title length: 50-60 characters
        return SeoText.TruncateTitle($"{title} | {_siteName}", 60) ?? _siteName;
    }

    private string? BuildCanonicalUrl()
    {
        var declared = Content?.Metadata?.SeoProperties?.Canonical;
        var canonical = !string.IsNullOrWhiteSpace(declared) ? declared : _currentUrl;

        // og:url and the structured data url must agree with the canonical URL.
        if (!string.IsNullOrWhiteSpace(canonical))
        {
            _currentUrl = canonical;
        }

        return string.IsNullOrWhiteSpace(canonical) ? null : canonical;
    }

    private string BuildRobotsDirective()
    {
        if (!AllowSearchIndexing)
            return "noindex, nofollow";

        if (!string.IsNullOrWhiteSpace(Content?.Metadata?.SeoProperties?.Robots))
            return Content.Metadata.SeoProperties.Robots;

        // Default: index, follow, max-snippet:-1, max-image-preview:large, max-video-preview:-1
        return "index, follow, max-snippet:-1, max-image-preview:large, max-video-preview:-1";
    }

    private string? NormalizeImageUrl(string? url) => SeoText.NormalizeImageUrl(url, _baseUrl);

    private string DetermineOgType() => OgTypesBySchemaType.GetValueOrDefault(PrimarySchemaTypeName, "website");

    #endregion

    #region Values Shown in the Markup

    /// <summary>
    /// The name of the first schema type, else the front matter type, else WebPage. An empty
    /// <see cref="SchemaTypes"/> array yields the default enum value, Article.
    /// </summary>
    private string PrimarySchemaTypeName =>
        SchemaTypes?.FirstOrDefault().ToString() ?? Content?.Metadata?.SeoProperties?.Type ?? "WebPage";

    private bool HasCategories => _categories?.Any() == true;

    private string? PublishedTimeUtc => _publishDate is { } published ? ContentUrls.FormatUtc(published) : null;

    private string? ModifiedTimeUtc => _modifiedDate is { } modified ? ContentUrls.FormatUtc(modified) : null;

    private string ContentFreshness =>
        _publishDate is { } published && (DateTime.UtcNow - published).TotalDays < 30 ? "recent" : "evergreen";

    #endregion

    #region JSON-LD Generation

    private SeoSchemaContext CreateSchemaContext() => new()
    {
        Content = Content,
        ContentItems = ContentItems,
        SiteName = _siteName,
        BaseUrl = _baseUrl,
        CurrentUrl = _currentUrl,
        LanguageCode = _languageCode,
        SiteLogoUrl = SiteLogoUrl,
        BreadcrumbHomeText = BreadcrumbHomeText,
        CustomBreadcrumbs = CustomBreadcrumbs,
        CollectionTitle = CollectionTitle,
        CollectionDescription = CollectionDescription,
        MetaTitle = _metaTitle,
        MetaDescription = _metaDescription,
        OgImage = _ogImage,
        Author = _author
    };

    private void GenerateAllJsonLdSchemas()
    {
        var context = CreateSchemaContext();

        // Collection pages: OnParametersSet only gets here without Content when ContentItems has items
        if (Content is null)
        {
            GenerateCollectionJsonLd(context);
            return;
        }

        if (TryUseCustomJsonLd(Content))
            return;

        _additionalJsonLdSchemas = [];

        // First schema becomes primary, rest go to additional
        var generatedSchemas = ResolveSchemaTypes(Content)
            .Select(schemaType => SeoSchemas.Create(schemaType, context))
            .OfType<object>()
            .Select(SeoSchemas.Serialize)
            .ToList();
        if (generatedSchemas.Count > 0)
        {
            _jsonLdContent = generatedSchemas[0];
            _additionalJsonLdSchemas.AddRange(generatedSchemas.Skip(1));
        }

        GenerateCollectionItemSchemas(context);
    }

    private void GenerateCollectionJsonLd(SeoSchemaContext context)
    {
        _additionalJsonLdSchemas = [];

        if (SchemaTypes?.Contains(Cms.Web.Components.SchemaType.CollectionPage) == true)
        {
            _jsonLdContent = SeoSchemas.Serialize(NavigationSchemas.CollectionPage(context));
        }

        GenerateCollectionItemSchemas(context);
    }

    /// <summary>
    /// Uses the custom JSON-LD from the front matter when it has one. It is written inside a script element, so it
    /// is re-serialized with HTML-sensitive characters escaped; anything that is not a JSON object or array is ignored.
    /// </summary>
    private bool TryUseCustomJsonLd(ContentItem content)
    {
        var customJsonLd = content.Metadata?.SeoProperties?.JsonLd;
        if (string.IsNullOrWhiteSpace(customJsonLd))
            return false;

        if (JsonLdNormalizer.TryNormalize(customJsonLd, out var normalizedJsonLd))
        {
            _jsonLdContent = normalizedJsonLd;
            return true;
        }

        Logger.LogWarning(
            "Ignoring custom JSON-LD for content {ContentPath} because it is not a valid JSON object or array; generated structured data is used instead",
            content.Path);
        return false;
    }

    /// <summary>
    /// The schema types to generate: the explicit <see cref="SchemaTypes"/>, else the front matter type, else
    /// WebPage. A single type gets a BreadcrumbList added when <see cref="GenerateMultipleSchemas"/> is on.
    /// </summary>
    private List<SchemaType> ResolveSchemaTypes(ContentItem content)
    {
        List<SchemaType> schemaTypes;
        var declaredType = content.Metadata?.SeoProperties?.Type;

        if (SchemaTypes?.Any() == true)
        {
            schemaTypes = [.. SchemaTypes];
        }
        else if (!string.IsNullOrWhiteSpace(declaredType) && Enum.TryParse<SchemaType>(declaredType, out var contentSchemaType))
        {
            schemaTypes = [contentSchemaType];
        }
        else
        {
            schemaTypes = [Cms.Web.Components.SchemaType.WebPage];
        }

        // When several schemas are given explicitly, the caller chose them all, so nothing is added.
        var shouldAutoAddBreadcrumbs = GenerateMultipleSchemas &&
                                       schemaTypes.Count == 1 &&
                                       content.Path != null &&
                                       !schemaTypes.Contains(Cms.Web.Components.SchemaType.BreadcrumbList);

        if (shouldAutoAddBreadcrumbs)
        {
            schemaTypes.Add(Cms.Web.Components.SchemaType.BreadcrumbList);
        }

        return schemaTypes;
    }

    /// <summary>
    /// Generates individual BlogPosting schemas for each item in a collection (Blog/Category/Tag pages).
    /// </summary>
    private void GenerateCollectionItemSchemas(SeoSchemaContext context)
    {
        if (ContentItems?.Any() != true)
            return;

        _additionalJsonLdSchemas ??= [];
        _additionalJsonLdSchemas.AddRange(ContentItems.Select(item =>
            SeoSchemas.Serialize(NavigationSchemas.CollectionItemPosting(item, context))));
    }

    /// <summary>
    /// Adds a schema.org FAQPage built from the generated questions. Only runs when
    /// <see cref="EnableGeneratedFaq"/> is on, because the questions are derived from titles, not authored.
    /// </summary>
    private void AddGeneratedFaqSchema()
    {
        if (!EnableAeoOptimization || !EnableGeneratedFaq || _structuredAnswers is not { Count: > 0 })
        {
            return;
        }

        _additionalJsonLdSchemas ??= [];
        _additionalJsonLdSchemas.Add(SeoSchemas.Serialize(NavigationSchemas.FaqPage(_structuredAnswers)));
    }

    #endregion
}

/// <summary>
/// Defines the types of Schema.org structured data schemas that can be generated for SEO purposes.
/// </summary>
public enum SchemaType
{
    /// <summary>
    /// Schema.org Article type for news articles, blog posts, and editorial content.
    /// </summary>
    Article,

    /// <summary>
    /// Schema.org BlogPosting type specifically for blog posts.
    /// </summary>
    BlogPosting,

    /// <summary>
    /// Schema.org WebPage type for general web pages.
    /// </summary>
    WebPage,

    /// <summary>
    /// Schema.org Product type for e-commerce products and items for sale.
    /// </summary>
    Product,

    /// <summary>
    /// Schema.org Organization type for companies, businesses, and organizations.
    /// </summary>
    Organization,

    /// <summary>
    /// Schema.org Person type for individual people and profiles.
    /// </summary>
    Person,

    /// <summary>
    /// Schema.org LocalBusiness type for physical business locations with address and hours.
    /// </summary>
    LocalBusiness,

    /// <summary>
    /// Schema.org Event type for events, conferences, and gatherings.
    /// </summary>
    Event,

    /// <summary>
    /// Schema.org Recipe type for cooking recipes with ingredients and instructions.
    /// </summary>
    Recipe,

    /// <summary>
    /// Schema.org FAQPage type for frequently asked questions pages.
    /// </summary>
    FAQ,

    /// <summary>
    /// Schema.org HowTo type for step-by-step instructions and tutorials.
    /// </summary>
    HowTo,

    /// <summary>
    /// Schema.org VideoObject type for video content.
    /// </summary>
    VideoObject,

    /// <summary>
    /// Schema.org Course type for educational courses and training programs.
    /// </summary>
    Course,

    /// <summary>
    /// Schema.org Review type for product or service reviews.
    /// </summary>
    Review,

    /// <summary>
    /// Schema.org BreadcrumbList type for navigation breadcrumbs.
    /// </summary>
    BreadcrumbList,

    /// <summary>
    /// Schema.org WebSite type with SearchAction for homepages and main site pages.
    /// </summary>
    WebSite,

    /// <summary>
    /// Schema.org CollectionPage type for pages listing multiple items (blog index, category, tag pages).
    /// </summary>
    CollectionPage
}
