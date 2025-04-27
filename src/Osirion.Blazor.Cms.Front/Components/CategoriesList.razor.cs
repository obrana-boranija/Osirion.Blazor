using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

public partial class CategoriesList(IContentProviderManager contentProviderManager)
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string LoadingText { get; set; } = "Loading categories...";

    [Parameter]
    public string NoContentText { get; set; } = "No categories available.";

    [Parameter]
    public Func<ContentCategory, string>? CategoryUrlFormatter { get; set; }

    [Parameter]
    public string? ActiveCategory { get; set; }

    [Parameter]
    public bool ShowCount { get; set; } = true;

    [Parameter]
    public bool SortByCount { get; set; } = true;

    [Parameter]
    public int? MaxCategories { get; set; }

    private IReadOnlyList<ContentCategory>? Categories { get; set; }
    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

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
            if (provider != null)
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
            Categories = Array.Empty<ContentCategory>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetCategoriesListClass()
    {
        return $"osirion-categories-list-container {CssClass}".Trim();
    }

    private string GetCategoryUrl(ContentCategory category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.Slug}";
    }

    private string GetCategoryLinkClass(ContentCategory category)
    {
        var isActive = !string.IsNullOrEmpty(ActiveCategory) &&
                     (category.Slug.Equals(ActiveCategory, StringComparison.OrdinalIgnoreCase) ||
                      category.Name.Equals(ActiveCategory, StringComparison.OrdinalIgnoreCase));

        return isActive ? "osirion-category-link osirion-active" : "osirion-category-link";
    }
}
