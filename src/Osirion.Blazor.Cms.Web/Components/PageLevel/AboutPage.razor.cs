using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// A whole about page from one content item (<c>about.md</c> by default): the title as the page's <c>h1</c>, the
/// description under it, the Markdown body, and whatever <see cref="ChildContent"/> adds below, such as
/// <c>OsirionAuthorCard</c>s for the people behind the site. It writes AboutPage structured data with the site's
/// organization as its main entity, and answers 404 when the content is missing. It renders without script.
/// </summary>
public partial class AboutPage
{
    private static readonly SchemaType[] DefaultSchemaTypes = [SchemaType.AboutPage];

    /// <summary>
    /// Initializes the page with the defaults of an about page: the content item with the slug <c>about</c> and a
    /// plain heading instead of a hero.
    /// </summary>
    public AboutPage()
    {
        ItemSlug = "about";
        ShowHero = false;
    }

    /// <summary>
    /// Gets or sets content shown after the body, for example author cards, a timeline or links to the project.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (!IsLoading && Item is null)
        {
            SignalNotFound();
        }
    }
}
