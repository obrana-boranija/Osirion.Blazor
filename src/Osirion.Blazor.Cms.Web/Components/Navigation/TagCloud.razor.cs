using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// TagCloud component for displaying a cloud of tags
/// </summary>
/// <param name="contentProviderManager"></param>
public partial class TagCloud(IContentProviderManager contentProviderManager)
{
    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public int? MaxTags { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading tags...";

    /// <summary>
    /// No content text to display when there are no tags available.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No tags available.";

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public string? ActiveTag { get; set; }

    /// <summary>
    /// /// A custom URL formatter for tags. If provided, it will be used to generate the URL for each tag.
    /// </summary>
    [Parameter]
    public Func<ContentTag, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public bool ShowCount { get; set; } = true;

    /// <summary>
    /// Whether to sort tags by count
    /// </summary>
    [Parameter]
    public bool SortByCount { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentTag>? Tags { get; set; }

    private bool IsLoading { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await LoadTagsAsync();
    }


    protected override async Task OnParametersSetAsync()
    {
        await LoadTagsAsync();
    }

    private async Task LoadTagsAsync()
    {
        if (Tags is null || Tags.Any() == false)
        {
            IsLoading = true;
            try
            {
                var provider = contentProviderManager.GetDefaultProvider();
                if (provider is not null)
                {
                    var allTags = await provider.GetTagsAsync();

                    if (SortByCount)
                    {
                        allTags = allTags.OrderByDescending(t => t.Count).ToList();
                    }
                    else
                    {
                        allTags = allTags.OrderBy(t => t.Name).ToList();
                    }

                    Tags = MaxTags.HasValue
                        ? allTags.Take(MaxTags.Value).ToList()
                        : allTags;
                }
            }
            catch
            {
                Tags = Array.Empty<ContentTag>();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private string GetTagCloudClass()
    {
        return $"osirion-tags-container osirion-tag-cloud {Class}".Trim();
    }

    private string GetTagLinkClass(ContentTag tag)
    {
        bool isActive = !string.IsNullOrWhiteSpace(ActiveTag) &&
                       (tag.Slug.Equals(ActiveTag, StringComparison.OrdinalIgnoreCase) ||
                        tag.Name.Equals(ActiveTag, StringComparison.OrdinalIgnoreCase));

        return isActive ? "osirion-tag-link osirion-tag-active" : "osirion-tag-link";
    }

    private string GetTagUrl(ContentTag tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? $"/tag/{tag.Slug}";
    }
}
