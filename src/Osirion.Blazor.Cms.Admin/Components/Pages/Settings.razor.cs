using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Features.Authentication.Services;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

    /// <summary>Defines the public member type.</summary>
public partial class Settings : IDisposable
{
    private const string SignOutFailedMessage = "Signing out failed. Please try again.";

    [Inject]
    private IAuthenticationService AuthService { get; set; } = default!;

    [Inject]
    private IGitHubTokenStore TokenStore { get; set; } = default!;

    [Inject]
    private ILogger<Settings> Logger { get; set; } = default!;

    /// <summary>Gets or sets the theme chosen on this page.</summary>
    /// <remarks>
    /// It is not a component parameter: a parameter of this name would clash with the <c>Theme</c> parameter of
    /// <see cref="Osirion.Blazor.Components.OsirionComponentBase"/>, and Blazor refuses to render a component with
    /// two parameters of the same name.
    /// </remarks>
    public new string Theme { get; set; } = "light";

    /// <summary>Gets or sets the ThemeChanged value.</summary>
    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        // Subscribe to state changes
        AdminState.StateChanged += StateHasChanged;
    }

    /// <summary>Performs the OnParametersSet operation.</summary>
    protected override void OnParametersSet()
    {
        // Set default theme
        if (string.IsNullOrWhiteSpace(Theme))
        {
            Theme = "light";
        }
    }

    /// <summary>Performs the OnAfterRender operation asynchronously.</summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ThemeChanged.HasDelegate && !firstRender)
        {
            await ThemeChanged.InvokeAsync(Theme);
        }
    }

    private async Task SignOut()
    {
        try
        {
            await AuthService.SignOutAsync();
            await TokenStore.RemoveTokenAsync();
            NavigationManager.NavigateTo("/osirion/login");
        }
        catch (Exception ex)
        {
            // The exception text can carry browser or storage details, so it is logged and never shown
            Logger.LogError(ex, "Signing out of GitHub failed");
            AdminState.SetErrorMessage(SignOutFailedMessage);
        }
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        // Unsubscribe from state changes
        AdminState.StateChanged -= StateHasChanged;
    }
}
