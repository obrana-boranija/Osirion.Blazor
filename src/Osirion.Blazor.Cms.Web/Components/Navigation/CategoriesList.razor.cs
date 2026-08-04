using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>Defines the CategoriesList type.</summary>
public partial class CategoriesList(IContentProviderManager contentProviderManager)
{
    /// <summary>Gets or sets the list title.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets the loading message.</summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading categories...";

    /// <summary>Gets or sets the empty-state message.</summary>
    [Parameter]
    public string NoContentText { get; set; } = "No categories available.";

    /// <summary>Gets or sets a formatter for category URLs.</summary>
    [Parameter]
    public Func<ContentCategory, string>? CategoryUrlFormatter { get; set; }

    /// <summary>Gets or sets the active category.</summary>
    [Parameter]
    public string? ActiveCategory { get; set; }

    /// <summary>Gets or sets a value indicating whether category counts are shown.</summary>
    [Parameter]
    public bool ShowCount { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether categories are sorted by count.</summary>
    [Parameter]
    public bool SortByCount { get; set; } = true;

    /// <summary>Gets or sets the maximum number of categories to display.</summary>
    [Parameter]
    public int? MaxCategories { get; set; }

    private IReadOnlyList<ContentCategory>? Categories { get; set; }
    private bool IsLoading { get; set; } = true;

    /// <summary>Initializes the component state and required services.</summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

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
        catch (Exception)
        {
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
