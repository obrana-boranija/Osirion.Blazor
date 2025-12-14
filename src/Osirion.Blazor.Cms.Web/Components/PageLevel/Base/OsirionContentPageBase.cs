using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Base class for content page components in the Osirion CMS.
/// Provides common parameters and properties for displaying content, hero sections, breadcrumbs, categories, tags, and loading states.
/// </summary>
public abstract partial class OsirionContentPageBase : OsirionComponentBase
{
    /// <summary>
    /// Gets or sets the locale of article content
    /// </summary>
    [Parameter]
    public string? Locale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the content to display
    /// </summary>
    [Parameter]
    public ContentQuery? Query { get; set; }

    #region SEO Metadata

    /// <summary>
    /// Gets or sets the schema.org type
    /// </summary>
    [Parameter]
    public SchemaType[]? SchemaTypes { get; set; }

    /// <summary>
    /// Generate multiple schema types when applicable (e.g., Article + BreadcrumbList).
    /// </summary>
    [Parameter]
    public bool GenerateMultipleSchemas { get; set; } = true;

    /// <summary>
    /// Gets or sets the website name
    /// </summary>
    [Parameter]
    public string? WebsiteName { get; set; }

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
    /// Gets or sets custom breadcrumb items to override automatic generation
    /// </summary>
    [Parameter]
    public List<(string Name, string Url)>? CustomBreadcrumbs { get; set; }

    /// <summary>
    /// Gets or sets the URL to the previous page in a series or pagination
    /// </summary>
    [Parameter]
    public string? PrevPageUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL to the next page in a series or pagination
    /// </summary>
    [Parameter]
    public string? NextPageUrl { get; set; }

    /// <summary>
    /// Gets or sets alternate language versions of this page for hreflang tags
    /// </summary>
    [Parameter]
    public List<string>? AlternateLanguageUrls { get; set; }

    #endregion

    #region Loading and Fetching

    /// <summary>
    /// Gets or sets whether the component is loading
    /// </summary>
    protected bool IsLoading { get; set; } = false;

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading article...";

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public bool ShowLoadingText { get; set; } = true;

    /// <summary>
    /// Gets or sets the not found title
    /// </summary>
    [Parameter]
    public string NotFoundTitle { get; set; } = "Article Not Found";

    /// <summary>
    /// Gets or sets the not found text
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "The requested article could not be found.";

    /// <summary>
    /// Gets or sets whether to show a back link when not found
    /// </summary>
    [Parameter]
    public bool ShowBackLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the back link URL
    /// </summary>
    [Parameter]
    public string BackLinkUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the back link text
    /// </summary>
    [Parameter]
    public string BackLinkText { get; set; } = "Back to Home";

    #endregion

    #region Hero

    /// <summary>
    /// Gets or sets whether to show the hero section
    /// </summary>
    [Parameter]
    public bool ShowHero { get; set; } = true;

    /// <summary>
    /// Gets or sets if image in hero section should be displayed as a background image. 
    /// </summary>
    [Parameter]
    public bool UseHeroBackgroundImage { get; set; }

    /// <summary>
    /// Gets or sets an overlay if UseHeroBackgroundImage is set to true
    /// </summary>
    [Parameter]
    public bool HasOverlay { get; set; }

    /// <summary>
    /// Gets or sets hero image. If not set, it will use Item?.FeaturedImageUrl property value. 
    /// </summary>
    [Parameter]
    public string? HeroImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the hero theme: "Light", "Dark", "System". Defaults "System".
    /// </summary>
    [Parameter]
    public ThemeMode HeroTheme { get; set; } = ThemeMode.System;

    /// <summary>
    /// Gets or sets the hero alignment: "left", "center", "right". Defaults to "left" or if justified is used, otherwise uses the specified alignment.
    /// </summary>
    [Parameter]
    public Alignment HeroAlignment { get; set; } = Alignment.Left;

    /// <summary>
    /// Gets or sets the image position when not using background: "left", "right". Defaults to "right" or if image is on the left or justified, otherwise uses the specified alignment.
    /// </summary>
    [Parameter]
    public Alignment HeroImagePosition { get; set; } = Alignment.Right;

    /// <summary>
    /// Gets or sets hero layout variant: "hero", "jumbotron", "minimal". Defaults to "hero".
    /// </summary>
    [Parameter]
    public HeroVariant HeroVariant { get; set; } = HeroVariant.Hero;

    /// <summary>
    /// Gets or sets whether to show hero buttons
    /// </summary>
    [Parameter]
    public bool ShowHeroButtons { get; set; } = false;

    /// <summary>
    /// Gets or sets the primary button text
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the primary button URL
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the secondary button text
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the secondary button URL
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets background pattern
    /// </summary>
    [Parameter]
    public BackgroundPatternType? HeroBackgroundPattern { get; set; } = BackgroundPatternType.Grid;

    #endregion

    #region Breadcrumbs

    /// <summary>
    /// Gets or sets whether to show breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the home link in breadcrumbs
    /// </summary>
    [Parameter]
    public bool ShowBreadcrumbHome { get; set; } = true;

    /// <summary>
    /// Gets or sets the home link text in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeText { get; set; } = "Home";

    /// <summary>
    /// Gets or sets the home link URL in breadcrumbs
    /// </summary>
    [Parameter]
    public string BreadcrumbHomeUrl { get; set; } = "/";

    #endregion

    #region Directory

    /// <summary>
    /// Gets or sets the directory URL formatter
    /// </summary>
    [Parameter]
    public string? DirectoryName { get; set; }

    /// <summary>
    /// Gets or sets the directory URL formatter
    /// </summary>
    [Parameter]
    public Func<DirectoryItem, string>? DirectoryUrlFormatter { get; set; }

    #endregion

    #region Categories

    /// <summary>
    /// Gets or sets whether to show categories in sidebar
    /// </summary>
    [Parameter]
    public bool ShowCategoriesSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets the categories section title
    /// </summary>
    [Parameter]
    public string CategoriesSectionTitle { get; set; } = "Categories";

    /// <summary>
    /// Gets or sets the category URL formatter
    /// </summary>
    [Parameter]
    public Func<string, string>? CategoryUrlFormatter { get; set; }

    #endregion

    #region Tags

    /// <summary>
    /// Gets or sets whether to show tags in sidebar
    /// </summary>
    [Parameter]
    public bool ShowTagsSidebar { get; set; } = true;

    /// <summary>
    /// Gets or sets the tags section title
    /// </summary>
    [Parameter]
    public string TagsSectionTitle { get; set; } = "Tags";

    /// <summary>
    /// Gets or sets the tag URL formatter
    /// </summary>
    [Parameter]
    public Func<string, string>? TagUrlFormatter { get; set; }

    #endregion

    [Inject]
    protected IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the navigation manager
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
}
