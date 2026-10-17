using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Handlers;

namespace Osirion.Blazor.Components;

/// <summary>
/// A site-wide announcement above the page, which visitors can close. Closing posts a small form that
/// <see cref="AnnouncementExtensions.MapOsirionAnnouncements"/> answers with a cookie, so it works without script; the
/// bar then stays hidden until the cookie expires or the announcement gets a new <see cref="Id"/>.
/// </summary>
/// <remarks>
/// The cookie is read from the request, so the bar renders under static server rendering and while prerendering.
/// Without an HTTP request (an interactive render), it renders nothing rather than show a dismissed announcement
/// again; place it in a statically rendered layout.
/// </remarks>
public partial class OsirionAnnouncementBar
{
    [CascadingParameter]
    private HttpContext? CascadedHttpContext { get; set; }

    [Inject]
    private IServiceProvider Services { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    /// <summary>
    /// Gets or sets the announcement's ID: 1 to 40 lowercase letters, digits or hyphens, such as <c>release-4-1</c>.
    /// A dismissal is remembered per ID, so a new announcement needs a new one.
    /// </summary>
    [Parameter, EditorRequired]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the announcement, a normal Razor fragment that may contain links. It is encoded like any Razor output.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the bar has a close button.
    /// </summary>
    [Parameter]
    public bool Dismissible { get; set; } = true;

    /// <summary>
    /// Gets or sets how many days a dismissal lasts, from 1 to 365. Any other value throws an
    /// <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Parameter]
    public int RememberDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets the accessible name of the close button.
    /// </summary>
    [Parameter]
    public string DismissText { get; set; } = "Dismiss announcement";

    /// <summary>
    /// Gets or sets the accessible name of the bar, which is a region landmark.
    /// </summary>
    [Parameter]
    public string RegionLabel { get; set; } = "Announcement";

    /// <summary>
    /// Gets or sets where the close form posts.
    /// </summary>
    [Parameter]
    public string DismissEndpoint { get; set; } = AnnouncementExtensions.DefaultDismissPattern;

    private bool IsVisible { get; set; }

    private string ReturnUrl { get; set; } = "/";

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (!AnnouncementHandler.IsValidId(Id))
        {
            throw new ArgumentException(
                $"'{Id}' is not a valid announcement ID: use 1 to 40 lowercase letters, digits or hyphens.", nameof(Id));
        }

        if (RememberDays is < 1 or > 365)
        {
            throw new ArgumentOutOfRangeException(nameof(RememberDays), RememberDays, "RememberDays must be from 1 to 365.");
        }

        var httpContext = CascadedHttpContext ?? Services.GetService<IHttpContextAccessor>()?.HttpContext;
        IsVisible = httpContext is not null && !AnnouncementHandler.IsDismissed(httpContext, Id);
        ReturnUrl = new Uri(Navigation.Uri, UriKind.Absolute).PathAndQuery;
    }

    private string GetBarClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-announcement" : $"osirion-announcement {Class}";
}
