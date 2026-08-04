using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Admin.Components.Pages;

    /// <summary>Defines the public member type.</summary>
public partial class Settings : IDisposable
{
    [Inject]
    private IAuthenticationService AuthService { get; set; } = default!;

    /// <summary>Gets or sets the Theme value.</summary>
    [Parameter]
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
            NavigationManager.NavigateTo("/osirion/login");
        }
        catch (Exception ex)
        {
            AdminState.SetErrorMessage($"Error signing out: {ex.Message}");
        }
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        // Unsubscribe from state changes
        AdminState.StateChanged -= StateHasChanged;
    }
}
