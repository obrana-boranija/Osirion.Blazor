using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Example.Components.Pages;
public partial class CmsDemo(IContentProviderManager ContentProviderManager)
{
    [Parameter]
    public string? Slug { get; set; }

    [Parameter]
    public string? CategorySlug { get; set; }

    [Parameter]
    public string? TagSlug { get; set; }

    [Parameter]
    public string? locale { get; set; }

    private string Directory => "blog";
    private string Locale => locale ?? "en";

    private string PageTitle =>
        !string.IsNullOrEmpty(CategorySlug) ? $"Category: {CategorySlug}" :
        !string.IsNullOrEmpty(TagSlug) ? $"Tag: {TagSlug}" :
        "Blog";

    private string _contentPath = "blog/sample-post.md";
    private string _providerId = "";
    private ContentItem? _contentItem;
    private Dictionary<string, string> _providers = new();
    private string? exMessage = null;

    /// <summary>
    /// Initialize component when parameters are set
    /// </summary>
    // protected override async Task OnInitializedAsync()
    // {
    //     // Get the available providers
    //     foreach (var provider in ContentProviderManager.GetAllProviders())
    //     {
    //         _providers.Add(provider.DisplayName, provider.ProviderId);
    //     }

    //     // Set default provider
    //     var defaultProvider = ContentProviderManager.GetDefaultProvider();
    //     if (defaultProvider != null)
    //     {
    //         _providerId = defaultProvider.ProviderId;
    //     }
    //     else if (_providers.Count > 0)
    //     {
    //         // If no default, use the first provider
    //         _providerId = _providers.Values.First();
    //     }

    //     _contentPath = string.IsNullOrEmpty(Slug) ? _contentPath : Slug;

    //     await base.OnInitializedAsync();
    // }

    private IReadOnlyList<ContentItem>? blogPosts = Array.Empty<ContentItem>();
    private IReadOnlyList<DirectoryItem>? directories = Array.Empty<DirectoryItem>();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Get the provider
            var provider = ContentProviderManager.GetDefaultProvider();

            // Create a query
            var query = new ContentQuery
            {
                //Directory = "blog", // Filter by directory
                SortBy = SortField.Date,
                SortDirection = SortDirection.Descending,
                Take = 10 // Latest 10 blog posts
            };

            // Execute query
            blogPosts = await provider.GetItemsByQueryAsync(query);
            var localDir = await provider.GetDirectoriesAsync("en");

            directories = localDir.FirstOrDefault()?.Children;

            _contentItem = blogPosts?.FirstOrDefault();
        }
        catch (ContentProviderException ex)
        {
            exMessage = ex.Message;
        }
    }

    /// <summary>
    /// Load the content when the button is clicked
    /// </summary>
    private void LoadContent()
    {
        // Component will automatically refresh with new parameters
        StateHasChanged();
    }
}
