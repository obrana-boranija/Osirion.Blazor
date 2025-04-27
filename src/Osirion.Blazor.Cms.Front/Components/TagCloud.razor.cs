using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Components;

public partial class TagCloud(IContentProviderManager contentProviderManager)
{
    [Parameter]
    public int? MaxTags { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string LoadingText { get; set; } = "Loading tags...";

    [Parameter]
    public string NoContentText { get; set; } = "No tags available.";

    [Parameter]
    public string? ActiveTag { get; set; }

    [Parameter]
    public Func<ContentTag, string>? TagUrlFormatter { get; set; }

    [Parameter]
    public bool ShowCount { get; set; } = true;

    [Parameter]
    public bool SortByCount { get; set; } = true;

    private IReadOnlyList<ContentTag>? Tags { get; set; }
    private bool IsLoading { get; set; } = true;

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
        IsLoading = true;
        try
        {
            var provider = contentProviderManager.GetDefaultProvider();
            if (provider != null)
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

    private string GetTagCloudClass()
    {
        return $"osirion-tags-container osirion-tag-cloud {CssClass}".Trim();
    }

    private string GetTagLinkClass(ContentTag tag)
    {
        bool isActive = !string.IsNullOrEmpty(ActiveTag) &&
                       (tag.Slug.Equals(ActiveTag, StringComparison.OrdinalIgnoreCase) ||
                        tag.Name.Equals(ActiveTag, StringComparison.OrdinalIgnoreCase));

        return isActive ? "osirion-tag-link osirion-tag-active" : "osirion-tag-link";
    }

    private string GetTagUrl(ContentTag tag)
    {
        return TagUrlFormatter?.Invoke(tag) ?? $"/tag/{tag.Slug}";
    }
}
