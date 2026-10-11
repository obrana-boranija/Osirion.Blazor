using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// A whole contact page: the title as the page's <c>h1</c>, an optional Markdown introduction from the content item
/// with the slug <c>contact</c>, and an <c>OsirionContactForm</c> with the contact details beside it. The page works
/// without that content item, from <see cref="Title"/> and <see cref="Description"/>; with it, the text is edited in
/// Markdown and the page writes ContactPage structured data. It renders without script.
/// </summary>
public partial class ContactPage
{
    private static readonly SchemaType[] DefaultSchemaTypes = [SchemaType.ContactPage];

    /// <summary>
    /// Initializes the page with the defaults of a contact page: the content item with the slug <c>contact</c> and a
    /// plain heading instead of a hero.
    /// </summary>
    public ContactPage()
    {
        ItemSlug = "contact";
        ShowHero = false;
    }

    /// <summary>
    /// Gets or sets the title used when there is no content item. Defaults to <c>Contact</c>.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Contact";

    /// <summary>
    /// Gets or sets the sentence under the title, also the meta description, used when there is no content item.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the form's id, which also names the form for static server-side rendering. Defaults to
    /// <c>contact-page-form</c>.
    /// </summary>
    [Parameter]
    public string FormId { get; set; } = "contact-page-form";

    /// <summary>Gets or sets the heading of the form. Defaults to <c>Write to us</c>.</summary>
    [Parameter]
    public string FormTitle { get; set; } = "Write to us";

    /// <summary>Gets or sets the text under the form's heading.</summary>
    [Parameter]
    public string FormDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address, phone, email and website shown beside the form. Nothing is shown beside it when
    /// <see langword="null"/>.
    /// </summary>
    [Parameter]
    public ContactInformation? ContactInfo { get; set; }

    /// <summary>
    /// Gets or sets whether the form sends its messages with the library's email service (configured with
    /// <c>AddOsirionEmailServices</c>). Defaults to <see langword="false"/>; handle <see cref="OnSubmit"/> instead.
    /// </summary>
    [Parameter]
    public bool UseBuiltInEmailService { get; set; }

    /// <summary>Gets or sets the callback that receives a valid submission.</summary>
    [Parameter]
    public EventCallback<ContactFormModel> OnSubmit { get; set; }

    /// <summary>
    /// Gets or sets a form that replaces the built-in one, for an <c>OsirionContactForm</c> configured beyond these
    /// parameters or a form of your own.
    /// </summary>
    [Parameter]
    public RenderFragment? FormContent { get; set; }

    /// <summary>Gets or sets content shown after the form, for example office hours or a map link.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string PageTitleText => Item?.Title ?? Title;
}
