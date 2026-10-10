using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace Osirion.Blazor.Cms.Admin.Tests.Integration;

/// <summary>
/// A public page of the host site. It reads the cascading authentication state that the admin registration adds,
/// to prove public pages keep rendering in a host that never configured authentication.
/// </summary>
[Route(Route)]
public sealed class PublicTestPage : ComponentBase
{
    public const string Route = "/public-page";
    public const string Text = "public razor page";

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private bool _isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is not null)
        {
            _isAuthenticated = (await AuthenticationState).User.Identity?.IsAuthenticated == true;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddContent(1, $"{Text}, authenticated: {_isAuthenticated}");
        builder.CloseElement();
    }
}
