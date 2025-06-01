using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Web.Components;

public abstract partial class OsirionContentListPageBase : OsirionContentPageBase
{
    #region Content

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public IReadOnlyList<ContentItem>? Items { get; set; }

    #endregion

    /// <summary>
    /// Initializes the component and loads content asynchronously based on the provided parameters.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await LoadContentAsync();
    }

    /// <summary>
    /// Loads the content asynchronously based on the provided parameters.
    /// </summary>
    protected async Task LoadContentAsync()
    {
        if (Items is null)
        {
            IsLoading = true;
            try
            {
                Items = await ContentProviderManager
                    .GetContentByQueryAsync(Query ?? new ContentQuery
                    {
                        Directory = DirectoryName,
                        Locale = Locale,
                    });
            }
            catch (Exception)
            {
                Items = null;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
