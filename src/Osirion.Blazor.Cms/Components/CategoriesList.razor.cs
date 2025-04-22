using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Components;

public partial class CategoriesList : OsirionComponentBase
{
    [Parameter]
    public IReadOnlyList<ContentCategory>? Categories { get; set; }

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public string LoadingText { get; set; } = "Loading categories...";

    [Parameter]
    public string NoContentText { get; set; } = "No categories available.";

    [Parameter]
    public Func<ContentCategory, string>? CategoryUrlFormatter { get; set; }

    private string GetCategoriesListClass()
    {
        return $"osirion-categories-list-container {CssClass}".Trim();
    }

    private string GetCategoryUrl(ContentCategory category)
    {
        return CategoryUrlFormatter?.Invoke(category) ?? $"/category/{category.Slug}";
    }
}
