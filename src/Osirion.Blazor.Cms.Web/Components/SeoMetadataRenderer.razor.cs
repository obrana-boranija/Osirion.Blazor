using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Web.Options;
using Osirion.Blazor.Core.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    private string? _mainQuestion;
    private string? _quickAnswer;

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
    /// Gets or sets the <c>applicationCategory</c> of the <see cref="SchemaType.SoftwareApplication"/> schema, one of
    /// the app types search engines support, such as <c>DeveloperApplication</c> or <c>BusinessApplication</c>.
    /// Defaults to <c>DeveloperApplication</c>.
    /// </summary>
    [Parameter]
    public string ApplicationCategory { get; set; } = "DeveloperApplication";

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
        _siteName = SiteNameOverride ?? OrganizationName ?? ExtractSiteName(navigationManager.BaseUri);
        _baseUrl = navigationManager.BaseUri.TrimEnd('/');
        _currentUrl = ResolvePageUrl();
        _locale = GetLocaleCode(Content?.Locale);
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
        _mainQuestion = null;
        _quickAnswer = null;
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

        // Apply site-wide defaults only if parameters are not set
        SiteNameOverride ??= options.SiteName;
        SiteDescription ??= options.SiteDescription;
        SiteLogoUrl ??= options.SiteLogoUrl;
        TwitterSite ??= options.TwitterSite;
        TwitterCreator ??= options.TwitterCreator;
        FacebookAppId ??= options.FacebookAppId;
        DefaultImageUrl ??= options.DefaultImageUrl;
        OrganizationName ??= options.OrganizationName;
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
            _metaDescription = TruncateDescription(CollectionDescription, 160);
            // Aggregate all unique tags from collection
            var allTags = ContentItems.SelectMany(c => c.Tags).Distinct().ToList();
            _metaKeywords = allTags.Any() ? string.Join(", ", allTags) : null;
        }
        else
        {
            // Single content page metadata
            _metaDescription = TruncateDescription(
                GetFirstValue(Content?.Metadata?.SeoProperties?.Description, Content?.Description),
                160
            );
            _metaKeywords = Content?.Tags.Any() == true ? string.Join(", ", Content.Tags) : null;
        }
    }

    private void BuildOpenGraphMetadata()
    {
        if (ContentItems?.Any() == true)
        {
            // Collection page Open Graph metadata
            _ogTitle = TruncateTitle(CollectionTitle ?? _metaTitle, 60);
            _ogDescription = TruncateDescription(CollectionDescription ?? _metaDescription, 200);
            _ogImage = NormalizeImageUrl(ContentItems.FirstOrDefault()?.FeaturedImageUrl ?? DefaultImageUrl);
            _ogType = "website";
            _ogImageAlt = CollectionTitle ?? _siteName;
        }
        else
        {
            // Single content page Open Graph metadata
            _ogTitle = TruncateTitle(
                GetFirstValue(Content?.Metadata?.SeoProperties?.OgTitle, Content?.Title, _metaTitle),
                60
            );
            _ogDescription = TruncateDescription(
                GetFirstValue(Content?.Metadata?.SeoProperties?.OgDescription, _metaDescription),
                200
            );
            _ogImage = NormalizeImageUrl(
                GetFirstValue(Content?.Metadata?.SeoProperties?.OgImageUrl, Content?.FeaturedImageUrl, DefaultImageUrl)
            );
            _ogType = GetFirstValue(Content?.Metadata?.SeoProperties?.OgType, DetermineOgType());
            _ogImageAlt = GetFirstValue(Content?.Title, _siteName);
        }
        
        _ogImageWidth = DefaultImageWidth;
        _ogImageHeight = DefaultImageHeight;
    }

    private void BuildTwitterCardMetadata()
    {
        if (ContentItems?.Any() == true)
        {
            // Collection page Twitter Card metadata
            _twitterCard = "summary_large_image";
            _twitterTitle = TruncateTitle(_ogTitle ?? _metaTitle, 70);
            _twitterDescription = TruncateDescription(_metaDescription, 200);
            _twitterImage = NormalizeImageUrl(_ogImage);
        }
        else
        {
            // Single content page Twitter Card metadata
            _twitterCard = GetFirstValue(Content?.Metadata?.SeoProperties?.TwitterCard, "summary_large_image");
            _twitterTitle = TruncateTitle(
                GetFirstValue(Content?.Metadata?.SeoProperties?.TwitterTitle, _ogTitle, _metaTitle),
                70
            );
            _twitterDescription = TruncateDescription(
                GetFirstValue(Content?.Metadata?.SeoProperties?.TwitterDescription, _metaDescription),
                200
            );
            _twitterImage = NormalizeImageUrl(
                GetFirstValue(Content?.Metadata?.SeoProperties?.TwitterImageUrl, _ogImage)
            );
        }
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
            _entitySummary = TruncateDescription(description, 280);
        }
    }

    private void BuildAeoOptimization()
    {
        // AEO: Identify potential questions this content answers (multi-language support)
        _answersTo = new List<string>();
        _structuredAnswers = new Dictionary<string, string>();

        var title = Content?.Title ?? CollectionTitle;
        var description = Content?.Description ?? CollectionDescription;

        if (!string.IsNullOrWhiteSpace(title))
        {
            // Multi-language question word patterns
            var questionWords = GetQuestionWordsForLanguage(_languageCode);
            
            // Check if title starts with any question word
            var isQuestion = questionWords.Any(qw => 
                title.StartsWith(qw, StringComparison.OrdinalIgnoreCase));

            if (isQuestion)
            {
                _answersTo.Add(title);
                
                if (!string.IsNullOrWhiteSpace(description))
                {
                    _structuredAnswers[title] = TruncateDescription(description, 320) ?? string.Empty;
                }
            }
            
            // Generate implicit questions for non-question titles (e.g., "Guide to X" → "How to X?")
            else
            {
                var implicitQuestion = GenerateImplicitQuestion(title, _languageCode);
                if (!string.IsNullOrWhiteSpace(implicitQuestion))
                {
                    _answersTo.Add(implicitQuestion);
                    _mainQuestion = implicitQuestion;
                    
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        _structuredAnswers[implicitQuestion] = TruncateDescription(description, 320) ?? string.Empty;
                        _quickAnswer = TruncateDescription(description, 160);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns question words for the specified language code.
    /// Supports English, Serbian (Latin &amp; Cyrillic), German, French, Spanish, Italian, Portuguese, and more.
    /// </summary>
    private static string[] GetQuestionWordsForLanguage(string? languageCode)
    {
        return languageCode?.ToLowerInvariant() switch
        {
            // English
            "en" or "en-us" or "en-gb" => new[] { "how", "what", "why", "when", "where", "who", "which", "can", "does", "is", "are", "should" },
            
            // Serbian (Latin)
            "sr" or "sr-latn" or "sr-latn-rs" => new[] { "kako", "šta", "zašto", "kada", "gde", "ko", "koji", "da li", "može", "treba" },
            
            // Serbian (Cyrillic)
            "sr-cyrl" or "sr-cyrl-rs" => new[] { "како", "шта", "зашто", "када", "где", "ко", "који", "да ли", "може", "треба" },
            
            // German
            "de" or "de-de" or "de-at" or "de-ch" => new[] { "wie", "was", "warum", "wann", "wo", "wer", "welche", "kann", "ist", "sind", "soll" },
            
            // French
            "fr" or "fr-fr" or "fr-ca" => new[] { "comment", "quoi", "pourquoi", "quand", "où", "qui", "quel", "quelle", "peut", "est", "sont" },
            
            // Spanish
            "es" or "es-es" or "es-mx" => new[] { "cómo", "qué", "por qué", "cuándo", "dónde", "quién", "cuál", "puede", "es", "son", "debe" },
            
            // Italian
            "it" or "it-it" => new[] { "come", "cosa", "perché", "quando", "dove", "chi", "quale", "può", "è", "sono", "deve" },
            
            // Portuguese
            "pt" or "pt-pt" or "pt-br" => new[] { "como", "o que", "por que", "quando", "onde", "quem", "qual", "pode", "é", "são", "deve" },
            
            // Dutch
            "nl" or "nl-nl" or "nl-be" => new[] { "hoe", "wat", "waarom", "wanneer", "waar", "wie", "welke", "kan", "is", "zijn", "moet" },
            
            // Russian
            "ru" or "ru-ru" => new[] { "как", "что", "почему", "когда", "где", "кто", "который", "может", "является" },
            
            // Default to English
            _ => new[] { "how", "what", "why", "when", "where", "who", "which", "can", "does", "is", "are", "should" }
        };
    }

    /// <summary>
    /// Generates an implicit question from a title based on language (e.g., "Guide to X" → "How to X?").
    /// </summary>
    private static string? GenerateImplicitQuestion(string title, string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(title)) return null;

        return languageCode?.ToLowerInvariant() switch
        {
            // English patterns
            "en" or "en-us" or "en-gb" => title switch
            {
                var t when t.StartsWith("guide to", StringComparison.OrdinalIgnoreCase) => 
                    $"How to {t[9..].Trim()}?",
                var t when t.Contains("tutorial", StringComparison.OrdinalIgnoreCase) => 
                    $"How to {t.Replace("tutorial", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.Contains("introduction to", StringComparison.OrdinalIgnoreCase) => 
                    $"What is {t.Replace("introduction to", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                _ => null
            },
            
            // Serbian (Latin) patterns
            "sr" or "sr-latn" or "sr-latn-rs" => title switch
            {
                var t when t.StartsWith("vodič za", StringComparison.OrdinalIgnoreCase) => 
                    $"Kako {t[8..].Trim()}?",
                var t when t.Contains("tutorijal", StringComparison.OrdinalIgnoreCase) => 
                    $"Kako {t.Replace("tutorijal", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.StartsWith("uvod u", StringComparison.OrdinalIgnoreCase) => 
                    $"Šta je {t[6..].Trim()}?",
                _ => null
            },
            
            // Serbian (Cyrillic) patterns
            "sr-cyrl" or "sr-cyrl-rs" => title switch
            {
                var t when t.StartsWith("водич за", StringComparison.OrdinalIgnoreCase) => 
                    $"Како {t[8..].Trim()}?",
                var t when t.Contains("туторијал", StringComparison.OrdinalIgnoreCase) => 
                    $"Како {t.Replace("туторијал", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.StartsWith("увод у", StringComparison.OrdinalIgnoreCase) => 
                    $"Шта је {t[6..].Trim()}?",
                _ => null
            },
            
            // German patterns
            "de" or "de-de" or "de-at" or "de-ch" => title switch
            {
                var t when t.StartsWith("anleitung zu", StringComparison.OrdinalIgnoreCase) => 
                    $"Wie {t[12..].Trim()}?",
                var t when t.Contains("tutorial", StringComparison.OrdinalIgnoreCase) => 
                    $"Wie {t.Replace("tutorial", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.StartsWith("einführung in", StringComparison.OrdinalIgnoreCase) => 
                    $"Was ist {t[13..].Trim()}?",
                _ => null
            },
            
            // French patterns
            "fr" or "fr-fr" or "fr-ca" => title switch
            {
                var t when t.StartsWith("guide de", StringComparison.OrdinalIgnoreCase) => 
                    $"Comment {t[8..].Trim()}?",
                var t when t.Contains("tutoriel", StringComparison.OrdinalIgnoreCase) => 
                    $"Comment {t.Replace("tutoriel", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.StartsWith("introduction à", StringComparison.OrdinalIgnoreCase) => 
                    $"Qu'est-ce que {t[14..].Trim()}?",
                _ => null
            },
            
            // Spanish patterns
            "es" or "es-es" or "es-mx" => title switch
            {
                var t when t.StartsWith("guía de", StringComparison.OrdinalIgnoreCase) => 
                    $"Cómo {t[8..].Trim()}?",
                var t when t.Contains("tutorial", StringComparison.OrdinalIgnoreCase) => 
                    $"Cómo {t.Replace("tutorial", "", StringComparison.OrdinalIgnoreCase).Trim()}?",
                var t when t.StartsWith("introducción a", StringComparison.OrdinalIgnoreCase) => 
                    $"Qué es {t[14..].Trim()}?",
                _ => null
            },
            
            // Default: no implicit question generation
            _ => null
        };
    }

    #endregion

    #region Rendering Methods

    private string RenderMetaTag(string name, string? content)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // HTML encode content to prevent XSS
        var encodedContent = System.Net.WebUtility.HtmlEncode(content);
        return $"<meta name=\"{name}\" content=\"{encodedContent}\" />";
    }

    private string RenderPropertyTag(string property, string? content)
    {
        if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var encodedContent = System.Net.WebUtility.HtmlEncode(content);
        return $"<meta property=\"{property}\" content=\"{encodedContent}\" />";
    }

    private string RenderMetaTag(string name, DateTime content)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return $"<meta name=\"{name}\" content=\"{content:yyyy-MM-dd}\" />";
    }

    private string RenderMetaTag(string name, DateTime? content)
    {
        if (string.IsNullOrWhiteSpace(name) || !content.HasValue)
            return string.Empty;

        return $"<meta name=\"{name}\" content=\"{content.Value:yyyy-MM-dd}\" />";
    }

    private string RenderMetaTag(string name, IReadOnlyList<string>? content)
    {
        if (string.IsNullOrWhiteSpace(name) || content is null || !content.Any())
            return string.Empty;

        var encodedContent = System.Net.WebUtility.HtmlEncode(string.Join(", ", content));
        return $"<meta name=\"{name}\" content=\"{encodedContent}\" />";
    }

    private string RenderLinkTag(string rel, string href)
    {
        if (string.IsNullOrWhiteSpace(rel) || string.IsNullOrWhiteSpace(href))
            return string.Empty;

        return $"<link rel=\"{rel}\" href=\"{href}\" />";
    }

    #endregion

    #region Title and Description Builders

    private string BuildMetaTitle()
    {
        if (ContentItems?.Any() == true && !string.IsNullOrWhiteSpace(CollectionTitle))
        {
            // Collection page title
            return $"{CollectionTitle} | {_siteName}";
        }
        
        var title = GetFirstValue(Content?.Metadata?.SeoProperties?.Title, Content?.Title);
        
        if (string.IsNullOrWhiteSpace(title))
            return _siteName;
            
        // Optimal title length: 50-60 characters
        return TruncateTitle($"{title} | {_siteName}", 60) ?? _siteName;
    }

    private string? TruncateTitle(string? title, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(title))
            return title;

        if (title.Length <= maxLength)
            return title;

        // Truncate at last complete word before maxLength
        var truncated = title[..maxLength].TrimEnd();
        var lastSpace = truncated.LastIndexOf(' ');
        
        if (lastSpace > 0)
            return truncated[..lastSpace] + "...";
            
        return truncated + "...";
    }

    private string? TruncateDescription(string? description, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(description))
            return description;

        if (description.Length <= maxLength)
            return description;

        // Truncate at last complete sentence or word before maxLength
        var truncated = description[..maxLength].TrimEnd();
        var lastPeriod = truncated.LastIndexOf('.');
        var lastSpace = truncated.LastIndexOf(' ');
        
        if (lastPeriod > maxLength - 50) // Keep if period is close to end
            return truncated[..(lastPeriod + 1)];
            
        if (lastSpace > 0)
            return truncated[..lastSpace] + "...";
            
        return truncated + "...";
    }

    #endregion

    #region URL and Robots Builders

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

        var faqSchema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "FAQPage",
            ["mainEntity"] = _structuredAnswers.Select(answer => new Dictionary<string, object?>
            {
                ["@type"] = "Question",
                ["name"] = answer.Key,
                ["acceptedAnswer"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Answer",
                    ["text"] = answer.Value
                }
            }).ToArray()
        };

        _additionalJsonLdSchemas ??= [];
        _additionalJsonLdSchemas.Add(JsonSerializer.Serialize(faqSchema, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
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

    private string? NormalizeImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return url;

        return $"{_baseUrl}/{url.TrimStart('/')}";
    }

    #endregion

    #region Helper Methods

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

    private string GetLocaleCode(string? locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
            return "en_US";

        // Convert from ISO format (en-US) to Open Graph format (en_US)
        return locale.Replace('-', '_');
    }

    private string? DetermineOgType()
    {
        var primarySchemaType = SchemaTypes?.FirstOrDefault().ToString() ?? 
                                Content?.Metadata?.SeoProperties?.Type ?? 
                                "WebPage";

        if (primarySchemaType.Equals("Article", StringComparison.OrdinalIgnoreCase) ||
            primarySchemaType.Equals("BlogPosting", StringComparison.OrdinalIgnoreCase) ||
            primarySchemaType.Equals("TechArticle", StringComparison.OrdinalIgnoreCase))
            return "article";

        if (primarySchemaType.Equals("ProfilePage", StringComparison.OrdinalIgnoreCase))
            return "profile";

        if (primarySchemaType.Equals("Product", StringComparison.OrdinalIgnoreCase))
            return "product";

        if (primarySchemaType.Equals("VideoObject", StringComparison.OrdinalIgnoreCase))
            return "video.other";

        return "website";
    }

    #endregion

    #region JSON-LD Generation

    private void GenerateAllJsonLdSchemas()
    {
        // Handle collection pages (ContentItems without Content)
        if (Content is null && ContentItems?.Any() == true)
        {
            _additionalJsonLdSchemas = new List<string>();
            
            // Generate collection page schema
            if (SchemaTypes?.Contains(Cms.Web.Components.SchemaType.CollectionPage) == true)
            {
                var collectionSchema = GenerateCollectionPageSchema();
                if (collectionSchema != null)
                {
                    var serialized = JsonSerializer.Serialize(collectionSchema, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });
                    _jsonLdContent = serialized;
                }
            }
            
            // Generate individual BlogPosting schemas for collection items
            GenerateCollectionItemSchemas();
            return;
        }
        
        // Early return if no content at all
        if (Content is null)
            return;

        // Use custom JSON-LD if provided. It is written inside a script element, so it is re-serialized with
        // HTML-sensitive characters escaped; anything that is not a JSON object or array is ignored.
        var customJsonLd = Content.Metadata?.SeoProperties?.JsonLd;
        if (!string.IsNullOrWhiteSpace(customJsonLd))
        {
            if (JsonLdNormalizer.TryNormalize(customJsonLd, out var normalizedJsonLd))
            {
                _jsonLdContent = normalizedJsonLd;
                return;
            }

            Logger.LogWarning(
                "Ignoring custom JSON-LD for content {ContentPath} because it is not a valid JSON object or array; generated structured data is used instead",
                Content.Path);
        }

        _additionalJsonLdSchemas = new List<string>();

        // Determine which schemas to generate
        var schemasToGenerate = new List<SchemaType>();
        
        if (SchemaTypes?.Any() == true)
        {
            // Use explicitly provided schema types
            schemasToGenerate.AddRange(SchemaTypes);
        }
        else if (!string.IsNullOrWhiteSpace(Content?.Metadata?.SeoProperties?.Type) &&
                 Enum.TryParse<SchemaType>(Content.Metadata.SeoProperties.Type, out var contentSchemaType))
        {
            // Use schema type from content metadata
            schemasToGenerate.Add(contentSchemaType);
        }
        else
        {
            // Default to WebPage
            schemasToGenerate.Add(Cms.Web.Components.SchemaType.WebPage);
        }

        // Auto-add breadcrumbs only if:
        // 1. GenerateMultipleSchemas is enabled
        // 2. Only one schema type specified (if multiple schemas explicitly provided, don't auto-add)
        // 3. Content has a path
        // 4. Breadcrumbs not already included
        var shouldAutoAddBreadcrumbs = GenerateMultipleSchemas && 
                                       schemasToGenerate.Count == 1 && 
                                       Content?.Path != null && 
                                       !schemasToGenerate.Contains(Cms.Web.Components.SchemaType.BreadcrumbList);
        
        if (shouldAutoAddBreadcrumbs)
        {
            schemasToGenerate.Add(Cms.Web.Components.SchemaType.BreadcrumbList);
        }

        // Generate all schemas
        var generatedSchemas = new List<string>();
        foreach (var schemaType in schemasToGenerate)
        {
            var schema = GenerateSchemaByType(schemaType);
            if (schema != null)
            {
                var serialized = JsonSerializer.Serialize(schema, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
                generatedSchemas.Add(serialized);
            }
        }

        // First schema becomes primary, rest go to additional
        if (generatedSchemas.Any())
        {
            _jsonLdContent = generatedSchemas.First();
            if (generatedSchemas.Count > 1)
            {
                _additionalJsonLdSchemas.AddRange(generatedSchemas.Skip(1));
            }
        }
        
        // Generate individual BlogPosting schemas for collection items
        GenerateCollectionItemSchemas();
    }
    
    /// <summary>
    /// Generates individual BlogPosting schemas for each item in a collection (Blog/Category/Tag pages).
    /// </summary>
    private void GenerateCollectionItemSchemas()
    {
        if (ContentItems?.Any() != true)
            return;
            
        foreach (var item in ContentItems)
        {
            var articleSchema = new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "BlogPosting",
                ["headline"] = item.Title,
                ["description"] = item.Description,
                ["image"] = NormalizeImageUrl(item.FeaturedImageUrl) ?? _ogImage,
                ["datePublished"] = ContentUrls.FormatUtc(item.PublishDate),
                ["dateModified"] = ContentUrls.FormatUtc(item.LastModified ?? item.PublishDate),
                ["author"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Person",
                    ["name"] = item.Author ?? _author ?? "Anonymous"
                },
                ["publisher"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Organization",
                    ["name"] = _siteName,
                    ["logo"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "ImageObject",
                        ["url"] = NormalizeImageUrl(SiteLogoUrl) ?? $"{_baseUrl}/logo.png"
                    }
                },
                ["url"] = $"{_baseUrl.TrimEnd('/')}{item.Url}",
                ["mainEntityOfPage"] = new Dictionary<string, object?>
                {
                    ["@type"] = "WebPage",
                    ["@id"] = $"{_baseUrl.TrimEnd('/')}{item.Url}"
                },
                ["keywords"] = item.Tags?.Any() == true ? string.Join(", ", item.Tags) : null,
                ["articleSection"] = item.Categories?.FirstOrDefault(),
                ["inLanguage"] = item.Locale ?? _languageCode,
                ["timeRequired"] = item.ReadTimeMinutes > 0 ? $"PT{item.ReadTimeMinutes}M" : null
            };
            
            var serialized = JsonSerializer.Serialize(articleSchema, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            
            _additionalJsonLdSchemas ??= new List<string>();
            _additionalJsonLdSchemas.Add(serialized);
        }
    }

    private object? GenerateSchemaByType(SchemaType schemaType)
    {
        return schemaType switch
        {
            Cms.Web.Components.SchemaType.Article => GenerateArticleSchema(),
            Cms.Web.Components.SchemaType.BlogPosting => GenerateBlogPostingSchema(),
            Cms.Web.Components.SchemaType.WebPage => GenerateWebPageSchema(),
            Cms.Web.Components.SchemaType.Product => GenerateProductSchema(),
            Cms.Web.Components.SchemaType.Organization => GenerateOrganizationSchema(),
            Cms.Web.Components.SchemaType.Person => GeneratePersonSchema(),
            Cms.Web.Components.SchemaType.LocalBusiness => GenerateLocalBusinessSchema(),
            Cms.Web.Components.SchemaType.Event => GenerateEventSchema(),
            Cms.Web.Components.SchemaType.Recipe => GenerateRecipeSchema(),
            Cms.Web.Components.SchemaType.FAQ => GenerateFAQSchema(),
            Cms.Web.Components.SchemaType.HowTo => GenerateHowToSchema(),
            Cms.Web.Components.SchemaType.VideoObject => GenerateVideoSchema(),
            Cms.Web.Components.SchemaType.Course => GenerateCourseSchema(),
            Cms.Web.Components.SchemaType.Review => GenerateReviewSchema(),
            Cms.Web.Components.SchemaType.BreadcrumbList => GenerateBreadcrumbSchema(),
            Cms.Web.Components.SchemaType.WebSite => GenerateWebSiteSchema(),
            Cms.Web.Components.SchemaType.CollectionPage => GenerateCollectionPageSchema(),
            Cms.Web.Components.SchemaType.TechArticle => GenerateArticleSchema("TechArticle"),
            Cms.Web.Components.SchemaType.SoftwareApplication => GenerateSoftwareApplicationSchema(),
            Cms.Web.Components.SchemaType.ProfilePage => GenerateProfilePageSchema(),
            Cms.Web.Components.SchemaType.AboutPage => GenerateAboutPageSchema(),
            Cms.Web.Components.SchemaType.ContactPage => GenerateWebPageSchema("ContactPage"),
            _ => GenerateDefaultSchema()
        };
    }

    #endregion

    #region Schema Generators

    private object GenerateArticleSchema(string type = "Article")
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = type,
            ["headline"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = Content.Author,
                ["url"] = $"{_baseUrl}/author/{Content.Author.ToLowerInvariant().Replace(" ", "-")}"
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["logo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ImageObject",
                    ["url"] = NormalizeImageUrl(SiteLogoUrl) ?? $"{_baseUrl}/logo.png",
                    ["width"] = 600,
                    ["height"] = 60
                }
            },
            ["datePublished"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["dateModified"] = ContentUrls.FormatUtc(Content.LastModified ?? Content.PublishDate),
            ["mainEntityOfPage"] = new Dictionary<string, object?>
            {
                ["@type"] = "WebPage",
                ["@id"] = _currentUrl
            },
            ["articleSection"] = Content.Categories.FirstOrDefault(),
            ["wordCount"] = Content.Content?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            ["inLanguage"] = _languageCode,
            ["url"] = _currentUrl,
            ["timeRequired"] = $"PT{Content.ReadTimeMinutes}M"
        };

        if (Content.Tags.Any())
            schema["keywords"] = string.Join(", ", Content.Tags);

        return schema;
    }

    private object GenerateBlogPostingSchema()
    {
        return GenerateArticleSchema();
    }

    private Dictionary<string, object?> GenerateWebPageSchema(string type = "WebPage")
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = type,
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["url"] = _currentUrl,
            ["datePublished"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["dateModified"] = (Content.LastModified is { } lastModified ? ContentUrls.FormatUtc(lastModified) : null),
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = Content.Author
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName
            }
        };
    }

    private object GenerateAboutPageSchema()
    {
        var schema = GenerateWebPageSchema("AboutPage");
        schema["mainEntity"] = new Dictionary<string, object?>
        {
            ["@type"] = "Organization",
            ["name"] = _siteName,
            ["url"] = _baseUrl,
            ["logo"] = NormalizeImageUrl(SiteLogoUrl)
        };

        return schema;
    }

    // Search engines show software rich results only with a price and a rating or review, which a content page does
    // not carry; front matter json_ld can supply the full schema when those exist.
    private object GenerateSoftwareApplicationSchema()
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "SoftwareApplication",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["url"] = _currentUrl,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["applicationCategory"] = ApplicationCategory,
            ["inLanguage"] = _languageCode,
            ["dateModified"] = ContentUrls.FormatUtc(Content.LastModified ?? Content.PublishDate),
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName
            }
        };

        if (Content.Tags.Any())
            schema["keywords"] = string.Join(", ", Content.Tags);

        return schema;
    }

    // An author's page: the page is the ProfilePage and the person it describes is its main entity. Only tags that
    // are absolute links are profiles elsewhere (sameAs).
    private object GenerateProfilePageSchema()
    {
        var profiles = Content!.Tags
            .Where(tag => Uri.TryCreate(tag, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
            .ToArray();

        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "ProfilePage",
            ["url"] = _currentUrl,
            ["dateCreated"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["dateModified"] = Content.LastModified is { } lastModified ? ContentUrls.FormatUtc(lastModified) : null,
            ["inLanguage"] = _languageCode,
            ["mainEntity"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = string.IsNullOrWhiteSpace(Content.Author) ? Content.Title : Content.Author,
                ["description"] = Content.Description,
                ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
                ["sameAs"] = profiles.Length > 0 ? profiles : null
            }
        };
    }

    private object GenerateProductSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Product",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["url"] = _currentUrl,
            ["brand"] = new Dictionary<string, object?>
            {
                ["@type"] = "Brand",
                ["name"] = _siteName
            },
            ["offers"] = new Dictionary<string, object?>
            {
                ["@type"] = "Offer",
                ["url"] = _currentUrl,
                ["priceCurrency"] = "USD",
                ["availability"] = "https://schema.org/InStock"
            }
        };
    }

    private object GenerateOrganizationSchema()
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Organization",
            ["name"] = Content!.Title ?? _siteName,
            ["description"] = Content.Description,
            ["url"] = _currentUrl,
            ["logo"] = NormalizeImageUrl(Content.FeaturedImageUrl) ?? $"{_baseUrl}/logo.png",
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl)
        };

        if (Content.Tags.Any())
            schema["sameAs"] = Content.Tags.ToArray();

        return schema;
    }

    private object GeneratePersonSchema()
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Person",
            ["name"] = Content!.Title ?? Content.Author,
            ["description"] = Content.Description,
            ["url"] = _currentUrl,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl)
        };

        if (Content.Tags.Any())
            schema["sameAs"] = Content.Tags.ToArray();

        return schema;
    }

    private object GenerateLocalBusinessSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "LocalBusiness",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["url"] = _currentUrl,
            ["telephone"] = Content.Metadata?.SeoProperties?.Type,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = "",
                ["addressLocality"] = "",
                ["addressRegion"] = "",
                ["postalCode"] = "",
                ["addressCountry"] = ""
            },
            ["geo"] = new Dictionary<string, object?>
            {
                ["@type"] = "GeoCoordinates",
                ["latitude"] = "",
                ["longitude"] = ""
            },
            ["openingHoursSpecification"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["@type"] = "OpeningHoursSpecification",
                    ["dayOfWeek"] = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" },
                    ["opens"] = "09:00",
                    ["closes"] = "17:00"
                }
            }
        };
    }

    private object GenerateEventSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Event",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["url"] = _currentUrl,
            ["startDate"] = ContentUrls.FormatUtc(Content.PublishDate),
                        ["eventStatus"] = "https://schema.org/EventScheduled",
            ["eventAttendanceMode"] = "https://schema.org/OfflineEventAttendanceMode",
            ["location"] = new Dictionary<string, object?>
            {
                ["@type"] = "Place",
                ["name"] = _siteName,
                ["address"] = new Dictionary<string, object?>
                {
                    ["@type"] = "PostalAddress",
                    ["streetAddress"] = "",
                    ["addressLocality"] = "",
                    ["addressRegion"] = "",
                    ["postalCode"] = "",
                    ["addressCountry"] = ""
                }
            },
            ["organizer"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["url"] = _baseUrl
            }
        };
    }

    private object GenerateRecipeSchema()
    {
        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Recipe",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = Content.Author
            },
            ["datePublished"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["prepTime"] = "PT30M",
            ["cookTime"] = "PT1H",
            ["totalTime"] = "PT1H30M",
            ["recipeYield"] = "4 servings",
            ["recipeCategory"] = Content.Categories.FirstOrDefault(),
            ["recipeCuisine"] = "",
            ["recipeIngredient"] = Array.Empty<string>(),
            ["recipeInstructions"] = Array.Empty<object>(),
            ["aggregateRating"] = new Dictionary<string, object?>
            {
                ["@type"] = "AggregateRating",
                ["ratingValue"] = "5",
                ["ratingCount"] = "1"
            }
        };

        if (Content.Tags.Any())
            schema["keywords"] = string.Join(", ", Content.Tags);

        return schema;
    }

    // A FAQPage needs real questions and answers. The only source is the generated FAQ data, which is opt-in,
    // so an explicit FAQ schema type adds nothing by itself and an empty FAQPage is never emitted.
    private object? GenerateFAQSchema() => null;

    private object GenerateHowToSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "HowTo",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["totalTime"] = "PT1H",
            ["estimatedCost"] = new Dictionary<string, object?>
            {
                ["@type"] = "MonetaryAmount",
                ["currency"] = "USD",
                ["value"] = "0"
            },
            ["tool"] = Array.Empty<object>(),
            ["supply"] = Array.Empty<object>(),
            ["step"] = Array.Empty<object>()
        };
    }

    private object GenerateVideoSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "VideoObject",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["thumbnailUrl"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["uploadDate"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["contentUrl"] = _currentUrl,
            ["embedUrl"] = _currentUrl,
            ["duration"] = "PT1M",
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = Content.Author
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["logo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ImageObject",
                    ["url"] = $"{_baseUrl}/logo.png"
                }
            }
        };
    }

    private object GenerateCourseSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Course",
            ["name"] = Content!.Title,
            ["description"] = Content.Description,
            ["provider"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["url"] = _baseUrl
            },
            ["image"] = NormalizeImageUrl(Content.FeaturedImageUrl),
            ["aggregateRating"] = new Dictionary<string, object?>
            {
                ["@type"] = "AggregateRating",
                ["ratingValue"] = "5",
                ["ratingCount"] = "1"
            },
            ["offers"] = new Dictionary<string, object?>
            {
                ["@type"] = "Offer",
                ["category"] = "Paid",
                ["priceCurrency"] = "USD",
                ["price"] = "0"
            }
        };
    }

    private object GenerateReviewSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Review",
            ["itemReviewed"] = new Dictionary<string, object?>
            {
                ["@type"] = "Thing",
                ["name"] = Content!.Title
            },
            ["author"] = new Dictionary<string, object?>
            {
                ["@type"] = "Person",
                ["name"] = Content.Author
            },
            ["datePublished"] = ContentUrls.FormatUtc(Content.PublishDate),
            ["reviewBody"] = Content.Description,
            ["reviewRating"] = new Dictionary<string, object?>
            {
                ["@type"] = "Rating",
                ["ratingValue"] = "5",
                ["bestRating"] = "5",
                ["worstRating"] = "1"
            }
        };
    }

    private object GenerateBreadcrumbSchema()
    {
        var breadcrumbItems = new List<Dictionary<string, object?>>();
        
        // Always start with Home
        breadcrumbItems.Add(new Dictionary<string, object?>
        {
            ["@type"] = "ListItem",
            ["position"] = 1,
            ["name"] = BreadcrumbHomeText,
            ["item"] = _baseUrl
        });

        // Use custom breadcrumbs if provided
        if (CustomBreadcrumbs?.Any() == true)
        {
            var position = 2;
            foreach (var (name, url) in CustomBreadcrumbs)
            {
                breadcrumbItems.Add(new Dictionary<string, object?>
                {
                    ["@type"] = "ListItem",
                    ["position"] = position++,
                    ["name"] = name,
                    ["item"] = url
                });
            }
        }
        // Parse the content path into breadcrumb segments
        else if (!string.IsNullOrWhiteSpace(Content?.Url))
        {
            var breadcrumbPath = new Osirion.Blazor.Components.BreadcrumbPath(Content.Url);
            var position = 2; // Start after Home

            foreach (var segment in breadcrumbPath.Segments)
            {
                // The last item is this page: its title keeps the punctuation and casing a slug loses. Earlier segments
                // are directories, named from the slug (slug-case to Title Case).
                var formattedName = segment.IsLast ? Content.Title : FormatBreadcrumbSegment(segment.Name);
                
                // Build full URL for this breadcrumb item
                var itemUrl = $"{_baseUrl}/{segment.Path}";

                breadcrumbItems.Add(new Dictionary<string, object?>
                {
                    ["@type"] = "ListItem",
                    ["position"] = position++,
                    ["name"] = formattedName,
                    ["item"] = segment.IsLast ? _currentUrl : itemUrl
                });
            }
        }
        // If no path, use current page as single breadcrumb after home
        else if (Content != null)
        {
            breadcrumbItems.Add(new Dictionary<string, object?>
            {
                ["@type"] = "ListItem",
                ["position"] = 2,
                ["name"] = Content.Title,
                ["item"] = _currentUrl
            });
        }

        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "BreadcrumbList",
            ["itemListElement"] = breadcrumbItems.ToArray()
        };
    }

    /// <summary>
    /// Formats a breadcrumb segment name from slug-case to Title Case
    /// </summary>
    private static string FormatBreadcrumbSegment(string segmentName)
    {
        if (string.IsNullOrWhiteSpace(segmentName))
            return string.Empty;

        // Replace hyphens and underscores with spaces
        var words = segmentName
            .Replace('-', ' ')
            .Replace('_', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Title case each word
        var titleCased = words.Select(word => 
            word.Length > 0 
                ? char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant() 
                : word);

        return string.Join(' ', titleCased);
    }

    private object GenerateDefaultSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = Content?.Metadata?.SeoProperties?.Type ?? "Thing",
            ["name"] = Content?.Title,
            ["description"] = Content?.Description,
            ["url"] = _currentUrl,
            ["inLanguage"] = _languageCode
        };
    }

    private object GenerateWebSiteSchema()
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "WebSite",
            ["name"] = _siteName,
            ["url"] = _baseUrl,
            ["description"] = Content?.Description,
            ["inLanguage"] = _languageCode,
            ["potentialAction"] = new Dictionary<string, object?>
            {
                ["@type"] = "SearchAction",
                ["target"] = new Dictionary<string, object?>
                {
                    ["@type"] = "EntryPoint",
                    ["urlTemplate"] = $"{_baseUrl}/search?q={{search_term_string}}"
                },
                ["query-input"] = "required name=search_term_string"
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["url"] = _baseUrl,
                ["logo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ImageObject",
                    ["url"] = NormalizeImageUrl(SiteLogoUrl) ?? $"{_baseUrl}/logo.png"
                }
            }
        };
    }

    private object GenerateCollectionPageSchema()
    {
        var itemListElements = new List<Dictionary<string, object?>>();
        
        if (ContentItems?.Any() == true)
        {
            var position = 1;
            foreach (var item in ContentItems)
            {
                itemListElements.Add(new Dictionary<string, object?>
                {
                    ["@type"] = "ListItem",
                    ["position"] = position++,
                    ["url"] = $"{_baseUrl}{item.Url}",
                    ["name"] = item.Title,
                    ["description"] = item.Description,
                    ["datePublished"] = ContentUrls.FormatUtc(item.PublishDate),
                    ["image"] = NormalizeImageUrl(item.FeaturedImageUrl)
                });
            }
        }

        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "CollectionPage",
            ["name"] = CollectionTitle ?? _metaTitle,
            ["description"] = CollectionDescription ?? _metaDescription,
            ["url"] = _currentUrl,
            ["inLanguage"] = _languageCode,
            ["mainEntity"] = new Dictionary<string, object?>
            {
                ["@type"] = "ItemList",
                ["numberOfItems"] = ContentItems?.Count ?? 0,
                ["itemListElement"] = itemListElements
            },
            ["breadcrumb"] = new Dictionary<string, object?>
            {
                ["@type"] = "BreadcrumbList",
                ["itemListElement"] = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "ListItem",
                        ["position"] = 1,
                        ["name"] = BreadcrumbHomeText,
                        ["item"] = _baseUrl
                    },
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "ListItem",
                        ["position"] = 2,
                        ["name"] = CollectionTitle ?? "Collection",
                        ["item"] = _currentUrl
                    }
                }
            },
            ["publisher"] = new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = _siteName,
                ["url"] = _baseUrl,
                ["logo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ImageObject",
                    ["url"] = NormalizeImageUrl(SiteLogoUrl) ?? $"{_baseUrl}/logo.png"
                }
            }
        };
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
    CollectionPage,

    /// <summary>
    /// Schema.org TechArticle type for technical documentation and how-to articles: the Article schema with a more
    /// specific type.
    /// </summary>
    TechArticle,

    /// <summary>
    /// Schema.org SoftwareApplication type for a product page about an app or a library, with the
    /// <c>applicationCategory</c> from <see cref="SeoMetadataRenderer.ApplicationCategory"/>.
    /// </summary>
    SoftwareApplication,

    /// <summary>
    /// Schema.org ProfilePage type for an author's page, with the person as its main entity. The author (or the
    /// title) is the person's name, and tags that are absolute links become <c>sameAs</c> profiles.
    /// </summary>
    ProfilePage,

    /// <summary>
    /// Schema.org AboutPage type, with the site's organization as its main entity.
    /// </summary>
    AboutPage,

    /// <summary>
    /// Schema.org ContactPage type for a contact page.
    /// </summary>
    ContactPage
}
