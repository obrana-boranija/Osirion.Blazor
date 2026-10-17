using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
    /// Gets or sets the number of loaded tags to show, taken from the start after sorting. All tags when
    /// <c>null</c>. Tags passed in <see cref="Tags"/> are shown as given.
    /// </summary>
    [Parameter]
    public int? MaxTags { get; set; }

    /// <summary>Gets or sets the heading above the tags, rendered as an <c>h3</c>. No heading when <c>null</c>.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the text shown while the tags load. Only an interactive render mode can show it: under static
    /// SSR the response is sent with the finished list.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading tags...";

    /// <summary>
    /// Gets or sets the text shown when there are no tags to show, when no default content provider is registered
    /// or when loading failed.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No tags available.";

    /// <summary>
    /// Gets or sets the slug or name of the current tag, compared ignoring case. Its link gets the
    /// <c>osirion-tag-active</c> class; <c>aria-current</c> is not written.
    /// </summary>
    [Parameter]
    public string? ActiveTag { get; set; }

    /// <summary>
    /// Gets or sets a function that builds each link from its tag (<c>Name</c>, <c>Slug</c> and <c>Count</c>).
    /// Without it, links go to <c>/tag/{Slug}</c>.
    /// </summary>
    [Parameter]
    public Func<ContentTag, string>? TagUrlFormatter { get; set; }

    /// <summary>Gets or sets whether each tag's item count is shown after its name.</summary>
    [Parameter]
    public bool ShowCount { get; set; } = true;

    /// <summary>
    /// Gets or sets how loaded tags are ordered: <c>true</c> by item count, highest first; <c>false</c> by name.
    /// Tags passed in <see cref="Tags"/> keep their order.
    /// </summary>
    [Parameter]
    public bool SortByCount { get; set; } = true;

    /// <summary>
    /// Gets or sets the tags to show. When <c>null</c> or empty, the component loads the tags of every item from
    /// the default content provider.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentTag>? Tags { get; set; }

    [Inject]
    private ILogger<TagCloud> Logger { get; set; } = default!;

    private bool IsLoading { get; set; } = true;


    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load the tag cloud");
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
