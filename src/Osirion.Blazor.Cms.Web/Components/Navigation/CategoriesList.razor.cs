using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the CategoriesList type.</summary>
public partial class CategoriesList(IContentProviderManager contentProviderManager)
{
    /// <summary>Gets or sets the heading above the list, rendered as an <c>h3</c>. No heading when <c>null</c>.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the text shown while the categories load. Only an interactive render mode can show it: under
    /// static SSR the response is sent with the finished list.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading categories...";

    /// <summary>
    /// Gets or sets the text shown when there are no categories to list, when no default content provider is
    /// registered or when loading failed.
    /// </summary>
    [Parameter]
    public string NoContentText { get; set; } = "No categories available.";

    /// <summary>
    /// Gets or sets a function that builds each link from its category (<c>Name</c>, <c>Slug</c> and <c>Count</c>).
    /// Without it, links go to <c>/category/{Slug}</c>.
    /// </summary>
    [Parameter]
    public Func<ContentCategory, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the slug or name of the current category, compared ignoring case. Its link gets the
    /// <c>osirion-active</c> class; <c>aria-current</c> is not written.
    /// </summary>
    [Parameter]
    public string? ActiveCategory { get; set; }

    /// <summary>Gets or sets whether each category's item count is shown after its name.</summary>
    [Parameter]
    public bool ShowCount { get; set; } = true;

    /// <summary>Gets or sets how categories are ordered: <c>true</c> by item count, highest first; <c>false</c> by name.</summary>
    [Parameter]
    public bool SortByCount { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of categories to show, taken from the start after sorting. All categories when
    /// <c>null</c>.
    /// </summary>
    [Parameter]
    public int? MaxCategories { get; set; }

    [Inject]
    private ILogger<CategoriesList> Logger { get; set; } = default!;

    private IReadOnlyList<ContentCategory>? Categories { get; set; }
    private bool IsLoading { get; set; } = true;


    /// <summary>Performs the OnParametersSet operation asynchronously.</summary>
    protected override async Task OnParametersSetAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        IsLoading = true;
        try
        {
            var provider = contentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                var allCategories = await provider.GetCategoriesAsync();

                // Apply sorting
                if (SortByCount)
                {
                    allCategories = allCategories.OrderByDescending(c => c.Count).ToList();
                }
                else
                {
                    allCategories = allCategories.OrderBy(c => c.Name).ToList();
                }

                // Apply limit if specified
                Categories = MaxCategories.HasValue
                    ? allCategories.Take(MaxCategories.Value).ToList()
                    : allCategories;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load the category list");
            Categories = Array.Empty<ContentCategory>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetCategoriesListClass()
    {
        return $"osirion-categories-list-container {Class}".Trim();
    }

    private string GetCategoryUrl(ContentCategory category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.Slug}";
    }

    private string GetCategoryLinkClass(ContentCategory category)
    {
        var isActive = !string.IsNullOrWhiteSpace(ActiveCategory) &&
                     (category.Slug.Equals(ActiveCategory, StringComparison.OrdinalIgnoreCase) ||
                      category.Name.Equals(ActiveCategory, StringComparison.OrdinalIgnoreCase));

        return isActive ? "osirion-category-link osirion-active" : "osirion-category-link";
    }
}
